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
                        // Verify the account belongs to current user & get balance
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

                        decimal newBal = (type == "Withdrawal") ? currentBal - amount : currentBal + amount;

                        // Insert transaction (store positive amount)
                        using (var cmd = new SqlCommand(@"
                            INSERT INTO BankTransactions
                              (AccountId,UserId,TransactionDate,TransactionTime,TransactionType,
                               Amount,Description,SenderRecipient,BalanceAfterTransaction)
                            VALUES
                              (@a,@u,CAST(GETDATE() AS date),CONVERT(time,GETDATE()),@t,
                               @amt,@d,@r,@bal);", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@a", accountId);
                            cmd.Parameters.AddWithValue("@u", userId);
                            cmd.Parameters.AddWithValue("@t", type);
                            cmd.Parameters.AddWithValue("@amt", amount);
                            cmd.Parameters.AddWithValue("@d", desc);
                            cmd.Parameters.AddWithValue("@r", recipient);
                            cmd.Parameters.AddWithValue("@bal", newBal);
                            cmd.ExecuteNonQuery();
                        }

                        // Update account balance
                        using (var upd = new SqlCommand(
                            "UPDATE BankAccounts SET Balance=@b WHERE AccountId=@a", conn, tx))
                        {
                            upd.Parameters.AddWithValue("@b", newBal);
                            upd.Parameters.AddWithValue("@a", accountId);
                            upd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        Show("Transaction saved.", true);
                        txtAmount.Text = "";
                        txtDescription.Text = "";
                        txtRecipient.Text = "";
                        BindRecent();
                    }
                    catch (Exception ex1)
                    {
                        tx.Rollback();
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
