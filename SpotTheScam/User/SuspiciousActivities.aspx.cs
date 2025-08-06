using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Collections.Generic;

namespace SpotTheScam.User
{
    public partial class SuspiciousActivities : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadSuspiciousTransactions();
        }

        private void LoadSuspiciousTransactions()
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TransactionId, TransactionDate, TransactionTime, Description, TransactionType, Amount, SenderRecipient, Severity
                    FROM BankTransactions
                    WHERE UserId = @UserId AND IsFlagged = 1
                        AND (ReviewStatus IS NULL OR ReviewStatus = 'Pending')
                    ORDER BY TransactionDate DESC, TransactionTime DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", currentUserId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvSuspicious.DataSource = dt;
                gvSuspicious.DataBind();
            }
        }

        protected void btnBulkSafe_Click(object sender, EventArgs e)
        {
            BulkUpdateTransactions("Safe");
        }

        protected void btnBulkAlert_Click(object sender, EventArgs e)
        {
            BulkUpdateTransactions("Alerted");
        }

        private void BulkUpdateTransactions(string status)
        {
            int updated = 0;
            foreach (GridViewRow row in gvSuspicious.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkRow");
                if (chk != null && chk.Checked)
                {
                    int txnId = Convert.ToInt32(gvSuspicious.DataKeys[row.RowIndex].Value);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        if (status == "Safe")
                        {
                            string sql = "UPDATE BankTransactions SET IsFlagged = 0, Severity = NULL, ReviewStatus = 'Safe' WHERE TransactionId = @TransactionId";
                            using (SqlCommand cmd = new SqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@TransactionId", txnId);
                                updated += cmd.ExecuteNonQuery();
                            }
                        }
                        else if (status == "Alerted")
                        {
                            string sql = "UPDATE BankTransactions SET ReviewStatus = 'Alerted' WHERE TransactionId = @TransactionId";
                            using (SqlCommand cmd = new SqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@TransactionId", txnId);
                                updated += cmd.ExecuteNonQuery();
                            }
                            // Send alert email after update
                            SendAlertEmailToTrustedContacts(txnId);
                        }
                    }
                }
            }
            ShowAlert((status == "Safe" ? "Selected transactions marked as safe." : "Alert sent for selected transactions."), "success");
            LoadSuspiciousTransactions();
        }


        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = "alert " + (type == "success" ? "alert-success" : "alert-error");
            AlertMessage.Text = message;
            AlertPanel.Visible = true;

            string script = @"
                setTimeout(function() {
                    var alertPanel = document.getElementById('" + AlertPanel.ClientID + @"');
                    if (alertPanel) {
                        alertPanel.style.display = 'none';
                    }
                }, 4000);
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideAlert", script, true);
        }

        private void SendAlertEmailToTrustedContacts(int transactionId)
        {
            // Get the transaction, user, and group
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. Find the UserId from the transaction
                int userId = -1, groupId = -1;
                string description = "", date = "", amount = "";
                using (var cmd = new SqlCommand(
                    @"SELECT bt.UserId, bt.Description, bt.Amount, bt.TransactionDate, fgm.GroupId
              FROM BankTransactions bt
              INNER JOIN FamilyGroupMembers fgm ON bt.UserId = fgm.UserId
              WHERE bt.TransactionId = @TransactionId", conn))
                {
                    cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = Convert.ToInt32(reader["UserId"]);
                            groupId = Convert.ToInt32(reader["GroupId"]);
                            description = reader["Description"].ToString();
                            date = Convert.ToDateTime(reader["TransactionDate"]).ToShortDateString();
                            amount = Convert.ToDecimal(reader["Amount"]).ToString("C2");
                        }
                    }
                }

                if (groupId == -1) return; // Safety

                // 2. Get emails of all trusted contacts and admins in the group (except the elderly user themself)
                List<string> emails = new List<string>();
                using (var cmd = new SqlCommand(
                    @"SELECT u.Email
              FROM FamilyGroupMembers fgm
              INNER JOIN Users u ON fgm.UserId = u.Id
              WHERE fgm.GroupId = @GroupId 
                AND (fgm.Role = 'trusted' OR fgm.Role = 'admin')
                AND u.Id <> @UserId", conn))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            emails.Add(reader["Email"].ToString());
                        }
                    }
                }

                if (emails.Count == 0) return; // No trusted contacts

                // 3. Send email (example using Gmail SMTP — adjust as needed)
                var fromAddress = new MailAddress("wongdeyu123@gmail.com", "SpotTheScam");
                const string fromPassword = "svib lwpm spwm lztp"; // Use your real app password

                string subject = "ALERT: Suspicious Transaction Detected";
                string body = $"A suspicious transaction (ID: {transactionId}) was flagged:\n\n" +
                    $"Date: {date}\nDescription: {description}\nAmount: {amount}\n\n" +
                    $"Please review in SpotTheScam and take action if necessary.";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
                };

                foreach (var toEmail in emails)
                {
                    using (var message = new MailMessage(fromAddress, new MailAddress(toEmail))
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        try
                        {
                            smtp.Send(message);
                        }
                        catch (Exception ex)
                        {
                            // Log error or handle appropriately
                        }
                    }
                }
            }
        }

    }
}
