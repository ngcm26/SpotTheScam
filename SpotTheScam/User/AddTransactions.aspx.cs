using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace SpotTheScam.User
{
    public partial class AddTransactions : Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/User/UserLogin.aspx"); return; }
            if (!IsPostBack)
            {
                BindAccounts();
                BindRecent(); // empty on first load
            }
        }

        protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindRecent();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            pnlMsg.Visible = false;

            if (string.IsNullOrEmpty(ddlAccount.SelectedValue))
            {
                Show("Please choose an account.", false);
                return;
            }
            decimal amount;
            if (!decimal.TryParse(txtAmount.Text, out amount) || amount <= 0m)
            {
                Show("Enter a valid amount greater than 0.", false);
                return;
            }
            string desc = (txtDescription.Text ?? "").Trim();
            if (desc.Length == 0)
            {
                Show("Description is required.", false);
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            int accountId = Convert.ToInt32(ddlAccount.SelectedValue);
            string type = ddlType.SelectedValue;
            string recipient = (txtRecipient.Text ?? "").Trim();
            if (recipient.Length == 0)
                recipient = (type == "Withdrawal" ? "Manual Withdrawal" : "Manual Deposit");

            try
            {
                using (var conn = new SqlConnection(cs))
                {
                    conn.Open();
                    var tx = conn.BeginTransaction();
                    try
                    {
                        // Verify account ownership & get current balance
                        decimal currentBal = 0m;
                        using (var check = new SqlCommand(
                            "SELECT Balance FROM BankAccounts WHERE AccountId=@a AND UserId=@u", conn, tx))
                        {
                            check.Parameters.AddWithValue("@a", accountId);
                            check.Parameters.AddWithValue("@u", userId);
                            object r = check.ExecuteScalar();
                            if (r == null)
                            {
                                tx.Rollback();
                                Show("Account not found.", false);
                                return;
                            }
                            currentBal = Convert.ToDecimal(r);
                        }

                        if (type == "Withdrawal" && currentBal < amount)
                        {
                            tx.Rollback();
                            Show("Insufficient funds for this withdrawal.", false);
                            return;
                        }

                        // --- Enforcement pre-check (Primary restrictions) ---
                        bool hold = false;          // if true → insert as held and don't update balance
                        string reviewStatus = null; // initial ReviewStatus
                        if (type == "Withdrawal")
                        {
                            // Load the most recent restriction row for this user (any active group where user is Primary)
                            decimal? single = null, daily = null;
                            int? startHr = null, endHr = null;

                            using (var cmdR = new SqlCommand(@"
SELECT TOP (1) r.SingleTransactionLimit, r.MaxDailyTransfer, r.AllowedStartHour, r.AllowedEndHour
FROM FamilyGroupMemberRestrictions r
JOIN FamilyGroupMembers m ON m.GroupId = r.GroupId AND m.UserId = r.PrimaryUserId
WHERE r.PrimaryUserId=@uid AND m.GroupRole='Primary' AND m.Status='Active'
ORDER BY r.UpdatedAt DESC;", conn, tx))
                            {
                                cmdR.Parameters.AddWithValue("@uid", userId);
                                using (var rd = cmdR.ExecuteReader())
                                {
                                    if (rd.Read())
                                    {
                                        single = rd["SingleTransactionLimit"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(rd["SingleTransactionLimit"]);
                                        daily = rd["MaxDailyTransfer"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(rd["MaxDailyTransfer"]);
                                        startHr = rd["AllowedStartHour"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["AllowedStartHour"]);
                                        endHr = rd["AllowedEndHour"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["AllowedEndHour"]);
                                    }
                                }
                            }

                            bool red = false, yellow = false;

                            // Single-transaction check (Red)
                            if (single.HasValue && amount > single.Value) red = true;

                            // Time-window check (Red) — if both hours are set
                            if (startHr.HasValue && endHr.HasValue)
                            {
                                int h = DateTime.Now.Hour;
                                bool inWindow = (startHr.Value <= endHr.Value)
                                    ? (h >= startHr.Value && h <= endHr.Value)
                                    : (h >= startHr.Value || h <= endHr.Value);
                                if (!inWindow) red = true;
                            }

                            // Daily total check (Yellow) — only count non-held withdrawals
                            if (daily.HasValue)
                            {
                                decimal dayTotal = 0m;
                                using (var cmdSum = new SqlCommand(@"
                                        SELECT ISNULL(SUM(Amount),0)
                                        FROM BankTransactions
                                        WHERE UserId=@uid
                                          AND TransactionDate = CAST(GETDATE() AS date)
                                          AND TransactionType='Withdrawal'
                                          AND IsHeld = 0;", conn, tx))
                                {
                                    cmdSum.Parameters.AddWithValue("@uid", userId);
                                    dayTotal = Convert.ToDecimal(cmdSum.ExecuteScalar());
                                }
                                if (dayTotal + amount > daily.Value) yellow = true;
                            }

                            // Whitelist downgrade: if recipient is whitelisted, downgrade Red→Yellow
                            if (red)
                            {
                                bool isWhitelisted = false;
                                using (var cmdW = new SqlCommand(@"
                                        SELECT 1
                                        FROM FamilyGroupMemberWhitelistedRecipients w
                                        WHERE w.UserId=@uid
                                          AND w.RecipientName=@name;", conn, tx))
                                {
                                    cmdW.Parameters.AddWithValue("@uid", userId);
                                    cmdW.Parameters.AddWithValue("@name", recipient);
                                    isWhitelisted = cmdW.ExecuteScalar() != null;
                                }
                                if (isWhitelisted) { red = false; yellow = true; }
                            }

                            hold = red;
                            reviewStatus = hold ? "Pending" : (yellow ? "Auto" : null);
                        }

                        // Compute new balance if not held
                        decimal newBal = (type == "Withdrawal") ? currentBal - amount : currentBal + amount;
                        decimal balToStore = (hold ? currentBal : newBal); // for held txns, don't reflect change yet

                        // Insert transaction (note: IsHeld + ReviewStatus)
                        using (var cmd = new SqlCommand(@"
                                INSERT INTO BankTransactions
                                  (AccountId,UserId,TransactionDate,TransactionTime,TransactionType,
                                   Amount,Description,SenderRecipient,BalanceAfterTransaction, IsHeld, ReviewStatus)
                                VALUES
                                  (@a,@u,CAST(GETDATE() AS date),CONVERT(time,GETDATE()),@t,
                                   @amt,@d,@r,@bal,@held,@rs);", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@a", accountId);
                            cmd.Parameters.AddWithValue("@u", userId);
                            cmd.Parameters.AddWithValue("@t", type);
                            cmd.Parameters.AddWithValue("@amt", amount);
                            cmd.Parameters.AddWithValue("@d", desc);
                            cmd.Parameters.AddWithValue("@r", recipient);
                            cmd.Parameters.AddWithValue("@bal", balToStore);
                            cmd.Parameters.AddWithValue("@held", hold ? 1 : 0);
                            if (string.IsNullOrEmpty(reviewStatus))
                                cmd.Parameters.AddWithValue("@rs", DBNull.Value);
                            else
                                cmd.Parameters.AddWithValue("@rs", reviewStatus);
                            cmd.ExecuteNonQuery();
                        }

                        // Only apply to account balance when not held
                        if (!hold)
                        {
                            using (var upd = new SqlCommand(
                                "UPDATE BankAccounts SET Balance=@b WHERE AccountId=@a", conn, tx))
                            {
                                upd.Parameters.AddWithValue("@b", newBal);
                                upd.Parameters.AddWithValue("@a", accountId);
                                upd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();

                        if (hold)
                            Show("Transfer submitted but HELD for guardian approval (violates rules).", true);
                        else
                            Show("Transaction saved.", true);

                        txtAmount.Text = "";
                        txtDescription.Text = "";
                        txtRecipient.Text = "";
                        BindRecent();
                    }
                    catch (Exception ex1)
                    {
                        try { tx.Rollback(); } catch { }
                        Show("Save failed: " + ex1.Message, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Show("Unexpected error: " + ex.Message, false);
            }
        }


        // ---------- helpers ----------

        private void BindAccounts()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(
                "SELECT AccountId, AccountNickname + ' — ' + BankName AS Label FROM BankAccounts WHERE UserId=@u ORDER BY CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                ddlAccount.DataSource = dt;
                ddlAccount.DataTextField = "Label";
                ddlAccount.DataValueField = "AccountId";
                ddlAccount.DataBind();

                ddlAccount.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- choose --", ""));
            }
        }

        private void BindRecent()
        {
            if (string.IsNullOrEmpty(ddlAccount.SelectedValue))
            {
                rptRecent.DataSource = null;
                rptRecent.DataBind();
                return;
            }

            int accountId = Convert.ToInt32(ddlAccount.SelectedValue);
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT TOP 8 TransactionDate, TransactionType, Amount, Description, BalanceAfterTransaction
                FROM BankTransactions
                WHERE AccountId=@a
                ORDER BY TransactionId DESC", conn))
            {
                cmd.Parameters.AddWithValue("@a", accountId);
                conn.Open();
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                rptRecent.DataSource = dt;
                rptRecent.DataBind();
            }
        }

        private void Show(string msg, bool ok)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = ok ? "alert alert-ok" : "alert alert-bad";
            lblMsg.Text = msg;
        }
    }
}
