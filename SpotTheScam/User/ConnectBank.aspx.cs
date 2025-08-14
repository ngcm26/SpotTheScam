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
            if (!IsPostBack) BindAccounts();
        }

        protected void btnSeed_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            try
            {
                using (var conn = new SqlConnection(cs))
                {
                    conn.Open();
                    // If user already has accounts, don’t duplicate
                    int existing = 0;
                    using (var check = new SqlCommand("SELECT COUNT(*) FROM BankAccounts WHERE UserId=@u", conn))
                    {
                        check.Parameters.AddWithValue("@u", userId);
                        existing = Convert.ToInt32(check.ExecuteScalar());
                    }
                    if (existing > 0)
                    {
                        Show("You already have bank accounts. Use 'Remove My Mock Data' if you want to reseed.", "warn");
                        BindAccounts();
                        return;
                    }

                    var tx = conn.BeginTransaction();
                    try
                    {
                        int acc1 = InsertAccount(conn, tx, userId, "UOB", "****1288", "UOB Savings");
                        int acc2 = InsertAccount(conn, tx, userId, "OCBC", "****4421", "OCBC Everyday");

                        SeedTransactionsForAccount(conn, tx, acc1, userId, 2750m);
                        SeedTransactionsForAccount(conn, tx, acc2, userId, 3850m);

                        tx.Commit();
                        Show("Mock accounts and transactions created.", "ok");
                    }
                    catch (Exception ex1)
                    {
                        tx.Rollback();
                        Show("Seeding failed: " + ex1.Message, "warn");
                    }
                }
            }
            catch (Exception ex)
            {
                Show("Unexpected error: " + ex.Message, "warn");
            }
            BindAccounts();
        }

        protected void btnWipe_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            try
            {
                using (var conn = new SqlConnection(cs))
                {
                    conn.Open();
                    var tx = conn.BeginTransaction();
                    try
                    {
                        using (var cmd1 = new SqlCommand(@"
                            DELETE bt FROM BankTransactions bt
                              JOIN BankAccounts ba ON bt.AccountId = ba.AccountId
                            WHERE ba.UserId=@u;
                            DELETE FROM BankAccounts WHERE UserId=@u;", conn, tx))
                        {
                            cmd1.Parameters.AddWithValue("@u", userId);
                            cmd1.ExecuteNonQuery();
                        }
                        tx.Commit();
                        Show("Your mock data has been removed.", "ok");
                    }
                    catch (Exception ex1)
                    {
                        tx.Rollback();
                        Show("Wipe failed: " + ex1.Message, "warn");
                    }
                }
            }
            catch (Exception ex)
            {
                Show("Unexpected error: " + ex.Message, "warn");
            }
            BindAccounts();
        }

        // --- helpers ---

        private int InsertAccount(SqlConnection conn, SqlTransaction tx, int userId, string bank, string masked, string nick)
        {
            using (var cmd = new SqlCommand(@"
                INSERT INTO BankAccounts(UserId,BankName,AccountNumberMasked,AccountNickname,Balance)
                VALUES(@u,@b,@m,@n,0); SELECT SCOPE_IDENTITY();", conn, tx))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@b", bank);
                cmd.Parameters.AddWithValue("@m", masked);
                cmd.Parameters.AddWithValue("@n", nick);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void SeedTransactionsForAccount(SqlConnection conn, SqlTransaction tx, int accountId, int userId, decimal startingBalance)
        {
            // A small, fixed, realistic set (last 20 days)
            var rows = new[]
            {
                new SeedRow(-32.50m,  "Grocery Store",      "FairPrice"),
                new SeedRow(-12.90m,  "Coffee",             "Starbucks"),
                new SeedRow(-85.00m,  "Pharmacy",           "Watsons"),
                new SeedRow( 500.00m, "Salary Credit",      "Employer Pte Ltd"),
                new SeedRow(-120.00m, "Utilities Bill",     "SP Services"),
                new SeedRow(-18.40m,  "Transport",          "Grab"),
                new SeedRow(-220.00m, "Online Shopping",    "Lazada"),
                new SeedRow(-9.90m,   "Streaming Service",  "Netflix"),
                new SeedRow(-45.20m,  "Dining",             "Hawker Centre"),
                new SeedRow( 150.00m, "Transfer From John", "John Tan")
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
                  VALUES
                  (@a,@u,@d,CONVERT(time, @dt),@t,ABS(@amt),@desc,@rcp,@bal);", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@a", accountId);
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@d", d0.AddDays(i));
                    cmd.Parameters.AddWithValue("@dt", d0.AddDays(i).AddHours(10 + (i % 6)));
                    cmd.Parameters.AddWithValue("@t", type);
                    cmd.Parameters.AddWithValue("@amt", r.Amount); // ABS() in SQL
                    cmd.Parameters.AddWithValue("@desc", r.Description);
                    cmd.Parameters.AddWithValue("@rcp", r.Recipient);
                    cmd.Parameters.AddWithValue("@bal", running);
                    cmd.ExecuteNonQuery();
                }
            }

            using (var upd = new SqlCommand("UPDATE BankAccounts SET Balance=@b WHERE AccountId=@a", conn, tx))
            {
                upd.Parameters.AddWithValue("@b", running);
                upd.Parameters.AddWithValue("@a", accountId);
                upd.ExecuteNonQuery();
            }
        }

        private void BindAccounts()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT AccountNickname,BankName,AccountNumberMasked,Balance
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
            pnlMsg.CssClass = (kind == "ok") ? "alert alert-success" : "alert alert-warn";
            lblMsg.Text = msg;
        }

        private struct SeedRow
        {
            public decimal Amount;
            public string Description;
            public string Recipient;
            public SeedRow(decimal a, string d, string r) { Amount = a; Description = d; Recipient = r; }
        }
    }
}
