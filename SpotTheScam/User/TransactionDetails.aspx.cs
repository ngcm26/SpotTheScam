using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace SpotTheScam.User
{
    public partial class TransactionDetails : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/User/UserLogin.aspx");
                    return;
                }

                int transactionId;
                if (!int.TryParse(Request.QueryString["tid"], out transactionId))
                {
                    ShowAlert("Invalid transaction ID.", "danger");
                    return;
                }

                LoadTransactionDetails(transactionId);
            }
        }

        private void LoadTransactionDetails(int transactionId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        bt.IsFlagged, bt.Severity, bt.TransactionDate, bt.TransactionTime, bt.Description, bt.TransactionType, 
                        bt.Amount, bt.SenderRecipient, bt.BalanceAfterTransaction, ba.BankName, ba.AccountNickname
                    FROM BankTransactions bt
                    INNER JOIN BankAccounts ba ON bt.AccountId = ba.AccountId
                    WHERE bt.TransactionId = @TransactionId AND bt.UserId = @UserId
                ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                cmd.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    bool isFlagged = reader["IsFlagged"] != DBNull.Value && Convert.ToBoolean(reader["IsFlagged"]);
                    string severity = reader["Severity"]?.ToString();
                    ltFlaggedStatus.Text = isFlagged
                        ? $"<i class='fas fa-exclamation-circle flag-icon' style='color:{(severity == "red" ? "#e53e3e" : "#ecc94b")};'></i> Flagged"
                        : "<i class='fas fa-check-circle flag-icon' style='color:#38a169;'></i> Not Flagged";
                    ltSeverity.Text = isFlagged ? severity : "-";
                    ltDate.Text = Convert.ToDateTime(reader["TransactionDate"]).ToString("d");
                    ltTime.Text = TimeSpan.Parse(reader["TransactionTime"].ToString()).ToString(@"hh\:mm");
                    ltDescription.Text = reader["Description"]?.ToString();
                    ltType.Text = reader["TransactionType"]?.ToString();
                    ltAmount.Text = Convert.ToDecimal(reader["Amount"]).ToString("C2");
                    ltSenderRecipient.Text = reader["SenderRecipient"]?.ToString();
                    ltBalanceAfter.Text = reader["BalanceAfterTransaction"] != DBNull.Value ? Convert.ToDecimal(reader["BalanceAfterTransaction"]).ToString("C2") : "-";
                    ltBankName.Text = reader["BankName"]?.ToString();
                    ltAccountNickname.Text = reader["AccountNickname"]?.ToString();
                }
                else
                {
                    ShowAlert("Transaction not found or you do not have access.", "danger");
                }
                reader.Close();
            }
        }

        protected void btnAlertFamily_Click(object sender, EventArgs e)
        {
            // Simulate alerting trusted family member (in-app notification logic can be added here)
            ShowAlert("Trusted family member has been alerted about this transaction.", "success");
        }

        protected void btnContactBank_Click(object sender, EventArgs e)
        {
            // Simulate contacting bank (could open a modal, send email, etc.)
            ShowAlert("Bank contact request has been initiated. Please follow up with your bank.", "success");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserTransactionLogs.aspx");
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = type == "success" ? "alert alert-success" : "alert alert-danger";
            AlertMessage.Text = message;
            AlertPanel.Visible = true;
        }
    }
}
