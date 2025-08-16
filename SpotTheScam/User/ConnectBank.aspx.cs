using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace SpotTheScam.User
{
    public partial class ConnectBank : Page
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/User/UserLogin.aspx"); return; }
            if (!IsPostBack)
            {
                ToggleAddMode();
                BindAccounts();
            }
        }

        private void ToggleAddMode()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            bool hasAny = false;

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT TOP 1 1 FROM BankAccounts WHERE UserId=@u", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                hasAny = cmd.ExecuteScalar() != null;
            }

            // If no accounts yet → show QuickStart button (which seeds),
            // otherwise show manual add form.
            pnlQuickStart.Visible = !hasAny;
            pnlAddForm.Visible = hasAny;
        }

        // Quick add = seed realistic accounts + transactions (first-time only)
        protected void btnQuickAdd_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            try
            {
                using (var conn = new SqlConnection(cs))
                {
                    conn.Open();

                    // prevent duplicate quick-add if user already has accounts
                    using (var check = new SqlCommand("SELECT COUNT(*) FROM BankAccounts WHERE UserId=@u", conn))
                    {
                        check.Parameters.AddWithValue("@u", userId);
                        if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                        { Show("You already have accounts added.", "warn"); ToggleAddMode(); BindAccounts(); return; }
                    }

                    var tx = conn.BeginTransaction();
                    try
                    {
                        int acc1 = InsertAccount(conn, tx, userId, "UOB", "****1288", "UOB Savings", 0m);
                        int acc2 = InsertAccount(conn, tx, userId, "OCBC", "****4421", "OCBC Everyday", 0m);

                        SeedTransactionsForAccount(conn, tx, acc1, userId, 2750m);
                        SeedTransactionsForAccount(conn, tx, acc2, userId, 3850m);

                        tx.Commit();
                        Show("Account added.", "ok");
                    }
                    catch (Exception ex1) { tx.Rollback(); Show("Could not add account. " + ex1.Message, "warn"); }
                }
            }
            catch (Exception ex) { Show("Unexpected error: " + ex.Message, "warn"); }

            ToggleAddMode();
            BindAccounts();
        }

        // Manual add (used once the user already has accounts)
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string bank = (ddlBank.SelectedValue ?? "").Trim();
            string nick = (txtNickname.Text ?? "").Trim();
            string last4 = (txtLast4.Text ?? "").Trim();
            string balStr = (txtStartBalance.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nick)) { Show("Nickname is required.", "warn"); return; }
            if (last4.Length != 4 || !int.TryParse(last4, out _)) { Show("Enter the last 4 digits (numbers only).", "warn"); return; }
            if (!decimal.TryParse(balStr, out decimal startBal) || startBal < 0m) { Show("Starting balance must be 0 or more.", "warn"); return; }

            try
            {
                using (var conn = new SqlConnection(cs))
                using (var cmd = new SqlCommand(@"
                    INSERT INTO BankAccounts(UserId,BankName,AccountNumberMasked,AccountNickname,Balance)
                    VALUES(@u,@b,@m,@n,@bal);", conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@b", bank);
                    cmd.Parameters.AddWithValue("@m", "****" + last4);
                    cmd.Parameters.AddWithValue("@n", nick);
                    cmd.Parameters.AddWithValue("@bal", startBal);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                txtNickname.Text = ""; txtLast4.Text = ""; txtStartBalance.Text = "";
                Show("Account added.", "ok");
                BindAccounts();
            }
            catch (Exception ex) { Show("Could not add account. " + ex.Message, "warn"); }
        }

        protected void rptAccounts_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "Delete", StringComparison.OrdinalIgnoreCase)) return;

            int accountId = Convert.ToInt32(e.CommandArgument);
            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                using (var conn = new SqlConnection(cs))
                {
                    conn.Open();

                    // only allow deleting your own account
                    using (var own = new SqlCommand("SELECT 1 FROM BankAccounts WHERE AccountId=@a AND UserId=@u", conn))
                    {
                        own.Parameters.AddWithValue("@a", accountId);
                        own.Parameters.AddWithValue("@u", userId);
                        if (own.ExecuteScalar() == null) { Show("Account not found.", "warn"); return; }
                    }

                    var tx = conn.BeginTransaction();
                    try
                    {
                        // 1) delete all transactions for this account (FK-safe)
                        using (var delTx = new SqlCommand("DELETE FROM BankTransactions WHERE AccountId=@a", conn, tx))
                        {
                            delTx.Parameters.AddWithValue("@a", accountId);
                            delTx.ExecuteNonQuery();
                        }

                        // 2) delete the account
                        using (var delAcc = new SqlCommand("DELETE FROM BankAccounts WHERE AccountId=@a", conn, tx))
                        {
                            delAcc.Parameters.AddWithValue("@a", accountId);
                            delAcc.ExecuteNonQuery();
                        }

                        tx.Commit();
                        Show("Account and its transactions deleted.", "ok");
                        BindAccounts();
                        ToggleAddMode(); // if none left, show the QuickStart add
                    }
                    catch (Exception ex1)
                    {
                        try { tx.Rollback(); } catch { }
                        Show("Delete failed. " + ex1.Message, "warn");
                    }
                }
            }
            catch (Exception ex)
            {
                Show("Unexpected error: " + ex.Message, "warn");
            }
        }


        // helpers
        private int InsertAccount(SqlConnection conn, SqlTransaction tx, int userId, string bank, string masked, string nick, decimal startBalance)
        {
            using (var cmd = new SqlCommand(@"
                INSERT INTO BankAccounts(UserId,BankName,AccountNumberMasked,AccountNickname,Balance)
                VALUES(@u,@b,@m,@n,@bal);
                SELECT SCOPE_IDENTITY();", conn, tx))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@b", bank);
                cmd.Parameters.AddWithValue("@m", masked);
                cmd.Parameters.AddWithValue("@n", nick);
                cmd.Parameters.AddWithValue("@bal", startBalance);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void SeedTransactionsForAccount(SqlConnection conn, SqlTransaction tx, int accountId, int userId, decimal startingBalance)
        {
            var rows = new[]
            {
                new SeedRow(-32.50m,"Grocery Store","FairPrice"),
                new SeedRow(-12.90m,"Coffee","Starbucks"),
                new SeedRow(-85.00m,"Pharmacy","Watsons"),
                new SeedRow( 500.00m,"Salary Credit","Employer Pte Ltd"),
                new SeedRow(-120.00m,"Utilities Bill","SP Services"),
                new SeedRow(-18.40m,"Transport","Grab"),
                new SeedRow(-220.00m,"Online Shopping","Lazada"),
                new SeedRow(-9.90m,"Streaming Service","Netflix"),
                new SeedRow(-45.20m,"Dining","Hawker Centre"),
                new SeedRow( 150.00m,"Transfer From John","John Tan")
            };

            decimal running = startingBalance;
            DateTime d0 = DateTime.Today.AddDays(-20);

            for (int i = 0; i < rows.Length; i++)
            {
                var r = rows[i];
                string type = r.Amount >= 0 ? "Deposit" : "Withdrawal";
                running += r.Amount;

                using (var cmd = new SqlCommand(@"
                  INSERT INTO BankTransactions
                  (AccountId,UserId,TransactionDate,TransactionTime,TransactionType,Amount,Description,SenderRecipient,BalanceAfterTransaction)
                  VALUES(@a,@u,@d,CONVERT(time,@dt),@t,ABS(@amt),@desc,@rcp,@bal);", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@a", accountId);
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@d", d0.AddDays(i));
                    cmd.Parameters.AddWithValue("@dt", d0.AddDays(i).AddHours(10 + (i % 6)));
                    cmd.Parameters.AddWithValue("@t", type);
                    cmd.Parameters.AddWithValue("@amt", r.Amount);
                    cmd.Parameters.AddWithValue("@desc", r.Description);
                    cmd.Parameters.AddWithValue("@rcp", r.Recipient);
                    cmd.Parameters.AddWithValue("@bal", running);
                    cmd.ExecuteNonQuery();
                }
            }

            using (var upd = new SqlCommand("UPDATE BankAccounts SET Balance=@b WHERE AccountId=@a", conn, tx))
            { upd.Parameters.AddWithValue("@b", running); upd.Parameters.AddWithValue("@a", accountId); upd.ExecuteNonQuery(); }
        }

        private void BindAccounts()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT AccountId, AccountNickname, BankName, AccountNumberMasked, Balance
                FROM BankAccounts WHERE UserId=@u ORDER BY CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                conn.Open();
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                rptAccounts.DataSource = dt;
                rptAccounts.DataBind();
            }
        }

        private void Show(string msg, string kind)
        {
            pnlMsg.Visible = true;
            lblMsg.Text = msg; // simple style via msg-card
        }

        private struct SeedRow
        {
            public decimal Amount; public string Description; public string Recipient;
            public SeedRow(decimal a, string d, string r) { Amount = a; Description = d; Recipient = r; }
        }
    }
}
