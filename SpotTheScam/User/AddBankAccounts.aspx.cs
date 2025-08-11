using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography; // Needed for Guid.NewGuid for uniqueSuffix
using System.Text; // Needed for Guid.NewGuid for uniqueSuffix

namespace SpotTheScam.User
{
    public partial class AddBankAccounts : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Ensure user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/User/UserLogin.aspx");
                    return;
                }
            }
        }

        protected void btnAddAccount_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                // Validation controls will display errors.
                ShowAlert("Please correct the errors in the form.", "error");
                return;
            }

            if (Session["UserId"] == null)
            {
                ShowAlert("User ID not found in session. Please log in again.", "danger");
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            int currentUserId = Convert.ToInt32(Session["UserId"]);

            string bankName = txtBankName.Text.Trim();
            string accountType = txtAccountType.Text.Trim();
            string accountNumber = txtAccountNumber.Text.Trim();
            string accountNickname = txtAccountNickname.Text.Trim();
            decimal balance = Convert.ToDecimal(txtBalance.Text.Trim());

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check for duplicate account number for this user
                    string checkDuplicateQuery = "SELECT COUNT(*) FROM BankAccounts WHERE AccountNumber = @AccountNumber AND UserId = @UserId";
                    SqlCommand checkCmd = new SqlCommand(checkDuplicateQuery, conn);
                    checkCmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                    checkCmd.Parameters.AddWithValue("@UserId", currentUserId);
                    int existingAccounts = (int)checkCmd.ExecuteScalar();

                    if (existingAccounts > 0)
                    {
                        ShowAlert("An account with this number already exists for your user. Please use a different account number.", "error");
                        return;
                    }

                    string query = @"INSERT INTO BankAccounts (UserId, BankName, AccountType, AccountNumber, AccountNickname, Balance)
                                   VALUES (@UserId, @BankName, @AccountType, @AccountNumber, @AccountNickname, @Balance)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@UserId", currentUserId);
                    cmd.Parameters.AddWithValue("@BankName", bankName);
                    cmd.Parameters.AddWithValue("@AccountType", accountType);
                    cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                    cmd.Parameters.AddWithValue("@AccountNickname", accountNickname);
                    cmd.Parameters.AddWithValue("@Balance", balance);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Bank account added successfully! Redirecting...", "success");
                        ClearForm();
                        // Redirect back to UserBankAccounts page after a short delay
                        ScriptManager.RegisterStartupScript(this, GetType(), "Redirect", "setTimeout(function(){ window.location.href = 'UserBankAccounts.aspx'; }, 2000);", true);
                    }
                    else
                    {
                        ShowAlert("Failed to add bank account. Please try again.", "error");
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation
                {
                    ShowAlert("An account with this number already exists for your user. Please use a different account number.", "error");
                }
                else
                {
                    ShowAlert("A database error occurred while adding the account: " + ex.Message, "error");
                    System.Diagnostics.Debug.WriteLine("SQL Error adding account: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("An unexpected error occurred while adding the account: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine("General Error adding account: " + ex.Message);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserBankAccounts.aspx");
        }

        private void ClearForm()
        {
            txtBankName.Text = string.Empty;
            txtAccountType.Text = "Savings"; // Default to Savings
            txtAccountNumber.Text = string.Empty;
            txtAccountNickname.Text = string.Empty;
            txtBalance.Text = string.Empty;
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = $"alert alert-{type}";
            AlertMessage.Text = message;
            AlertPanel.Visible = true;

            if (type == "success")
            {
                AlertIcon.Attributes["class"] = "alert-icon fas fa-check-circle";
            }
            else
            {
                AlertIcon.Attributes["class"] = "alert-icon fas fa-times-circle";
            }

            // Hide alert after 5 seconds
            string script = @"
                setTimeout(function() {
                    var alertPanel = document.getElementById('" + AlertPanel.ClientID + @"');
                    if (alertPanel) {
                        alertPanel.style.display = 'none';
                    }
                }, 5000);
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideAlert", script, true);
        }

        // Event handler for the Seed Mock Accounts button
        protected void btnSeedMockAccounts_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                ShowAlert("Please log in to seed data.", "error");
                return;
            }

            int currentUserId = Convert.ToInt32(Session["UserId"]);
            try
            {
                DataSeeder.SeedMockBankAccounts(currentUserId);
                ShowAlert("Mock bank accounts and transactions seeded successfully! Redirecting...", "success");
                ScriptManager.RegisterStartupScript(this, GetType(), "Redirect", "setTimeout(function(){ window.location.href = 'UserBankAccounts.aspx'; }, 2000);", true);
            }
            catch (Exception ex)
            {
                ShowAlert("Error seeding mock data: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine("Error seeding mock data: " + ex.Message);
            }
        }
    }

    // DataSeeder static class (now includes transaction seeding)
    public static class DataSeeder
    {
        public static void SeedMockBankAccounts(int userId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 4);
                    DateTime dateAdded = DateTime.Now;

                    // To store the IDs of newly created accounts
                    int dbsAccountId = 0;
                    int ocbcAccountId = 0;
                    int uobAccountId = 0;

                    // Mock Account 1: DBS Savings
                    string query1 = @"INSERT INTO BankAccounts (UserId, BankName, AccountType, AccountNumber, AccountNickname, Balance, DateAdded)
                                      OUTPUT INSERTED.AccountId
                                      VALUES (@UserId, @BankName1, @AccountType1, @AccountNumber1, @AccountNickname1, @Balance1, @DateAdded)";
                    using (SqlCommand cmd1 = new SqlCommand(query1, conn, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@UserId", userId);
                        cmd1.Parameters.AddWithValue("@BankName1", "DBS Bank (Mock)");
                        cmd1.Parameters.AddWithValue("@AccountType1", "Savings");
                        cmd1.Parameters.AddWithValue("@AccountNumber1", "001-12345-" + uniqueSuffix);
                        cmd1.Parameters.AddWithValue("@AccountNickname1", "Main Savings " + uniqueSuffix);
                        cmd1.Parameters.AddWithValue("@Balance1", 5234.56m); // Initial balance for transactions
                        cmd1.Parameters.AddWithValue("@DateAdded", dateAdded);
                        dbsAccountId = (int)cmd1.ExecuteScalar();
                    }

                    // Mock Account 2: OCBC Checking
                    string query2 = @"INSERT INTO BankAccounts (UserId, BankName, AccountType, AccountNumber, AccountNickname, Balance, DateAdded)
                                      OUTPUT INSERTED.AccountId
                                      VALUES (@UserId, @BankName2, @AccountType2, @AccountNumber2, @AccountNickname2, @Balance2, @DateAdded)";
                    using (SqlCommand cmd2 = new SqlCommand(query2, conn, transaction))
                    {
                        cmd2.Parameters.AddWithValue("@UserId", userId);
                        cmd2.Parameters.AddWithValue("@BankName2", "OCBC Bank (Mock)");
                        cmd2.Parameters.AddWithValue("@AccountType2", "Checking");
                        cmd2.Parameters.AddWithValue("@AccountNumber2", "501-98765-" + uniqueSuffix);
                        cmd2.Parameters.AddWithValue("@AccountNickname2", "Daily Spending " + uniqueSuffix);
                        cmd2.Parameters.AddWithValue("@Balance2", 1500.75m); // Initial balance for transactions
                        cmd2.Parameters.AddWithValue("@DateAdded", dateAdded);
                        ocbcAccountId = (int)cmd2.ExecuteScalar();
                    }

                    // Mock Account 3: UOB Credit Card
                    string query3 = @"INSERT INTO BankAccounts (UserId, BankName, AccountType, AccountNumber, AccountNickname, Balance, DateAdded)
                                      OUTPUT INSERTED.AccountId
                                      VALUES (@UserId, @BankName3, @AccountType3, @AccountNumber3, @AccountNickname3, @Balance3, @DateAdded)";
                    using (SqlCommand cmd3 = new SqlCommand(query3, conn, transaction))
                    {
                        cmd3.Parameters.AddWithValue("@UserId", userId);
                        cmd3.Parameters.AddWithValue("@BankName3", "UOB Bank (Mock)");
                        cmd3.Parameters.AddWithValue("@AccountType3", "Credit Card");
                        cmd3.Parameters.AddWithValue("@AccountNumber3", "4567-8901-2345-" + uniqueSuffix);
                        cmd3.Parameters.AddWithValue("@AccountNickname3", "Travel Card " + uniqueSuffix);
                        cmd3.Parameters.AddWithValue("@Balance3", -300.00m); // Initial balance for transactions (negative)
                        cmd3.Parameters.AddWithValue("@DateAdded", dateAdded);
                        uobAccountId = (int)cmd3.ExecuteScalar();
                    }

                    // --- NEW: Seed Mock Transactions ---
                    DateTime today = DateTime.Now.Date;

                    // Transactions for DBS Savings (dbsAccountId)
                    InsertMockTransaction(conn, transaction, dbsAccountId, userId, today.AddDays(-10), TimeSpan.Parse("10:30"), "Online Shopping", "Debit", 85.50m, 5149.06m, "Amazon SG");
                    InsertMockTransaction(conn, transaction, dbsAccountId, userId, today.AddDays(-7), TimeSpan.Parse("14:00"), "Salary Deposit", "Credit", 2500.00m, 7649.06m, "Employer Inc.");
                    InsertMockTransaction(conn, transaction, dbsAccountId, userId, today.AddDays(-5), TimeSpan.Parse("09:15"), "ATM Withdrawal", "Debit", 200.00m, 7449.06m, "ATM");
                    InsertMockTransaction(conn, transaction, dbsAccountId, userId, today.AddDays(-3), TimeSpan.Parse("18:45"), "Restaurant Bill", "Debit", 65.45m, 7383.61m, "Food Palace");
                    InsertMockTransaction(conn, transaction, dbsAccountId, userId, today.AddDays(-2), TimeSpan.Parse("11:00"), "Subscription Service", "Debit", 15.00m, 7368.61m, "Streaming Plus");
                    InsertMockTransaction(conn, transaction, dbsAccountId, userId, today.AddDays(-1), TimeSpan.Parse("19:45"), "Scam Transaction (Simulated Phishing)", "Debit", 1500.00m, 5868.61m, "Unknown Vendor 123");
                    InsertMockTransaction(conn, transaction, dbsAccountId, userId, today, TimeSpan.Parse("09:00"), "Online Transfer Received", "Credit", 100.00m, 5968.61m, "Family Member");

                    // Transactions for OCBC Checking (ocbcAccountId)
                    InsertMockTransaction(conn, transaction, ocbcAccountId, userId, today.AddDays(-8), TimeSpan.Parse("11:00"), "Utility Bill Payment", "Debit", 120.00m, 1380.75m, "SP Services");
                    InsertMockTransaction(conn, transaction, ocbcAccountId, userId, today.AddDays(-4), TimeSpan.Parse("16:20"), "Grocery Shopping", "Debit", 75.20m, 1305.55m, "FairPrice Supermarket");
                    InsertMockTransaction(conn, transaction, ocbcAccountId, userId, today.AddDays(-1), TimeSpan.Parse("10:00"), "Mobile Top-up", "Debit", 20.00m, 1285.55m, "Singtel");

                    // Transactions for UOB Credit Card (uobAccountId)
                    InsertMockTransaction(conn, transaction, uobAccountId, userId, today.AddDays(-15), TimeSpan.Parse("08:00"), "Hotel Booking", "Debit", 500.00m, -800.00m, "Luxury Stays Inc.");
                    InsertMockTransaction(conn, transaction, uobAccountId, userId, today.AddDays(-9), TimeSpan.Parse("20:00"), "Flight Tickets", "Debit", 700.00m, -1500.00m, "FlyAway Airlines");
                    InsertMockTransaction(conn, transaction, uobAccountId, userId, today.AddDays(-3), TimeSpan.Parse("10:00"), "Credit Card Payment", "Credit", 1200.00m, -300.00m, "OCBC Checking (Payment)");
                    InsertMockTransaction(conn, transaction, uobAccountId, userId, today.AddDays(-1), TimeSpan.Parse("14:00"), "Scam Attempt (Unauthorized Charge)", "Debit", 50.00m, -350.00m, "Suspicious Merchant");

                    transaction.Commit();
                    System.Diagnostics.Debug.WriteLine("Mock bank accounts and transactions seeded successfully for UserID: " + userId);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine("Error seeding mock data: " + ex.Message);
                    throw; // Re-throw to indicate failure
                }
            }
        }

        // Helper method to insert a single transaction
        private static void InsertMockTransaction(SqlConnection conn, SqlTransaction transaction, int accountId, int userId, DateTime date, TimeSpan time, string description, string type, decimal amount, decimal balanceAfter, string senderRecipient)
        {
            bool isFlagged = false;
            string severity = null;
            string flagReason = "";

            // 1. Load user restrictions (if any)
            int? allowedStartHour = null;
            int? allowedEndHour = null;
            decimal? maxDailyTransfer = null;

            string restrictionQuery = @"
        SELECT AllowedStartHour, AllowedEndHour, MaxDailyTransfer
        FROM FamilyGroupMemberRestrictions
        WHERE UserId = @UserId";

            using (SqlCommand restrictionCmd = new SqlCommand(restrictionQuery, conn, transaction))
            {
                restrictionCmd.Parameters.AddWithValue("@UserId", userId);
                using (SqlDataReader reader = restrictionCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0)) allowedStartHour = reader.GetInt32(0);
                        if (!reader.IsDBNull(1)) allowedEndHour = reader.GetInt32(1);
                        if (!reader.IsDBNull(2)) maxDailyTransfer = reader.GetDecimal(2);
                    }
                }
            }

            // 2. Apply restriction checks
            if (allowedStartHour.HasValue && allowedEndHour.HasValue)
            {
                if (time < TimeSpan.FromHours(allowedStartHour.Value) || time > TimeSpan.FromHours(allowedEndHour.Value))
                {
                    isFlagged = true;
                    severity = "yellow";
                    flagReason += "Outside allowed transaction time window. ";
                }
            }

            if (maxDailyTransfer.HasValue && type == "Debit")
            {
                string dailyTotalQuery = @"
            SELECT ISNULL(SUM(Amount), 0)
            FROM BankTransactions
            WHERE UserId = @UserId AND TransactionType = 'Debit' AND CAST(TransactionDate AS DATE) = @TransactionDate";

                using (SqlCommand totalCmd = new SqlCommand(dailyTotalQuery, conn, transaction))
                {
                    totalCmd.Parameters.AddWithValue("@UserId", userId);
                    totalCmd.Parameters.AddWithValue("@TransactionDate", date);
                    decimal currentTotal = (decimal)totalCmd.ExecuteScalar();

                    if (currentTotal + amount > maxDailyTransfer.Value)
                    {
                        isFlagged = true;
                        severity = "red";
                        flagReason += $"Daily transfer limit exceeded (Limit: {maxDailyTransfer.Value}). ";
                    }
                }
            }

            // 3. Fallback flagging (if no restriction)
            if (amount > 500)
            {
                isFlagged = true;
                if (severity == null)
                    severity = "red";
                flagReason += "Large transaction amount. ";
            }
            if (!string.IsNullOrEmpty(senderRecipient) &&
                (senderRecipient.ToLower().Contains("unknown") || senderRecipient.ToLower().Contains("suspicious")))
            {
                isFlagged = true;
                if (severity == null)
                    severity = "yellow";
                flagReason += "Suspicious recipient name. ";
            }


            // 4. Insert transaction
            string query = @"
        INSERT INTO BankTransactions 
        (AccountId, UserId, TransactionDate, TransactionTime, Description, TransactionType, Amount, BalanceAfterTransaction, SenderRecipient, IsFlagged, Severity, Notes)
        VALUES 
        (@AccountId, @UserId, @TransactionDate, @TransactionTime, @Description, @TransactionType, @Amount, @BalanceAfterTransaction, @SenderRecipient, @IsFlagged, @Severity, @Notes)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@AccountId", accountId);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@TransactionDate", date);
                cmd.Parameters.AddWithValue("@TransactionTime", time);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@TransactionType", type);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@BalanceAfterTransaction", balanceAfter);
                cmd.Parameters.AddWithValue("@SenderRecipient", senderRecipient ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsFlagged", isFlagged);
                cmd.Parameters.AddWithValue("@Severity", (object)severity ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrWhiteSpace(flagReason) ? (object)DBNull.Value : flagReason.Trim());
                cmd.ExecuteNonQuery();
            }
        }

    }
}