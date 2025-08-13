using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class AddManualTransaction : System.Web.UI.Page
    {
        // Get the database connection string from the Web.config file
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure the user is logged in before allowing any actions
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // When the page first loads, fill the dropdown with the user's bank accounts
                PopulateAccountsDropdown();
            }
        }

        /// <summary>
        /// Fetches the current user's bank accounts and populates the dropdown list.
        /// </summary>
        private void PopulateAccountsDropdown()
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Select the account nickname and ID to show the user a friendly name
                string query = "SELECT AccountId, AccountNickname FROM BankAccounts WHERE UserId = @UserId ORDER BY AccountNickname";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlMyAccounts.DataSource = dt;
                    ddlMyAccounts.DataTextField = "AccountNickname"; // The text the user sees
                    ddlMyAccounts.DataValueField = "AccountId";      // The ID we use in the backend
                    ddlMyAccounts.DataBind();
                }
            }
            // Add a default instructional item at the top
            ddlMyAccounts.Items.Insert(0, new ListItem("--- Select an Account ---", ""));
        }

        /// <summary>
        /// This is the main event handler that runs when the user submits the form.
        /// </summary>
        protected void btnSubmitTransaction_Click(object sender, EventArgs e)
        {
            // Validate that an account was selected
            if (string.IsNullOrEmpty(ddlMyAccounts.SelectedValue))
            {
                ShowMessage("Please select an account.", Color.Red);
                return;
            }

            // Get all the required values from the form
            int accountId = Convert.ToInt32(ddlMyAccounts.SelectedValue);
            decimal amount = Convert.ToDecimal(txtAmount.Text);
            string description = txtDescription.Text.Trim();
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            // Use a transaction to ensure all database operations succeed or fail together.
            // This prevents a scenario where the balance is updated but the transaction log fails to write.
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction sqlTran = conn.BeginTransaction();

                try
                {
                    // 1. SAVE THE TRANSACTION AND UPDATE THE BALANCE
                    // ===============================================
                    string insertTransactionQuery = @"
                        INSERT INTO BankTransactions (AccountId, UserId, TransactionDate, TransactionType, Amount, Description, SenderRecipient, BalanceAfterTransaction)
                        VALUES (@AccountId, @UserId, GETDATE(), 'Withdrawal', @Amount, @Description, @SenderRecipient, 
                               (SELECT Balance FROM BankAccounts WHERE AccountId = @AccountId) - @Amount);
                        SELECT SCOPE_IDENTITY();"; // SCOPE_IDENTITY() gets the ID of the row we just inserted.

                    int newTransactionId;
                    using (SqlCommand cmd = new SqlCommand(insertTransactionQuery, conn, sqlTran))
                    {
                        cmd.Parameters.AddWithValue("@AccountId", accountId);
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@SenderRecipient", "Manual Withdrawal"); // Set a default value for this simulation

                        // Execute the command and get the new TransactionId
                        newTransactionId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Now, update the actual balance in the BankAccounts table
                    string updateBalanceQuery = "UPDATE BankAccounts SET Balance = Balance - @Amount WHERE AccountId = @AccountId";
                    using (SqlCommand cmd = new SqlCommand(updateBalanceQuery, conn, sqlTran))
                    {
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@AccountId", accountId);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. CHECK FOR RULE VIOLATIONS AND CREATE ALERTS
                    // ================================================
                    CheckAndCreateAlerts(conn, sqlTran, currentUserId, newTransactionId, amount, description);

                    // 3. COMMIT THE TRANSACTION
                    // ===========================
                    // If everything was successful, commit the transaction to make the changes permanent.
                    sqlTran.Commit();
                    ShowMessage("Transaction successful!", Color.Green);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    // If any error occurred, roll back the entire transaction.
                    sqlTran.Rollback();
                    ShowMessage("An error occurred. The transaction has been cancelled. Details: " + ex.Message, Color.Red);
                }
            }
        }

        /// <summary>
        /// Checks if the new transaction violates any rules set by trusted family members.
        /// </summary>
        private void CheckAndCreateAlerts(SqlConnection conn, SqlTransaction tran, int userId, int transactionId, decimal amount, string description)
        {
            // First, find which family group the user belongs to.
            string getFamilyGroupQuery = "SELECT FamilyGroupId FROM FamilyMembers WHERE UserId = @UserId AND Role = 'Elderly'";
            int familyGroupId = 0;

            using (SqlCommand cmd = new SqlCommand(getFamilyGroupQuery, conn, tran))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    familyGroupId = Convert.ToInt32(result);
                }
            }

            // If the user is not in a family group as an 'Elderly' member, there are no rules to check.
            if (familyGroupId == 0)
            {
                return;
            }

            // Next, get the spending limit set for this user.
            string getLimitQuery = "SELECT SingleTransactionLimit FROM FamilyMemberSettings WHERE UserId = @UserId AND FamilyGroupId = @FamilyGroupId";
            decimal? singleTransactionLimit = null; // Use nullable decimal for the limit

            using (SqlCommand cmd = new SqlCommand(getLimitQuery, conn, tran))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FamilyGroupId", familyGroupId);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    singleTransactionLimit = Convert.ToDecimal(result);
                }
            }

            // Now, check if the transaction amount exceeds the limit.
            if (singleTransactionLimit.HasValue && amount > singleTransactionLimit.Value)
            {
                // The rule has been broken! Create an alert.
                string alertMessage = $"Alert: A transaction of ${amount:F2} for '{description}' exceeded the set limit of ${singleTransactionLimit.Value:F2}.";
                CreateAlert(conn, tran, userId, familyGroupId, transactionId, "Single Transaction Limit Exceeded", alertMessage);
            }
        }

        /// <summary>
        /// Inserts a new record into the Alerts table.
        /// </summary>
        private void CreateAlert(SqlConnection conn, SqlTransaction tran, int userId, int familyGroupId, int transactionId, string alertType, string alertMessage)
        {
            string insertAlertQuery = @"
                INSERT INTO Alerts (UserId, FamilyGroupId, TransactionId, AlertType, AlertMessage, AlertDate, IsRead)
                VALUES (@UserId, @FamilyGroupId, @TransactionId, @AlertType, @AlertMessage, GETDATE(), 0)";

            using (SqlCommand cmd = new SqlCommand(insertAlertQuery, conn, tran))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FamilyGroupId", familyGroupId);
                cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                cmd.Parameters.AddWithValue("@AlertType", alertType);
                cmd.Parameters.AddWithValue("@AlertMessage", alertMessage);
                cmd.ExecuteNonQuery();
            }
        }

        private void ShowMessage(string message, Color color)
        {
            lblMessage.Text = message;
            // Use Bootstrap alert classes for styling
            pnlMessage.CssClass = (color == Color.Green) ? "alert alert-success" : "alert alert-danger";
            pnlMessage.Visible = true;
        }


        private void ClearForm()
        {
            txtAmount.Text = string.Empty;
            txtDescription.Text = string.Empty;
            ddlMyAccounts.SelectedIndex = 0;
        }
    }
}