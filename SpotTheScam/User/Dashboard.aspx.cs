using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace SpotTheScam.User
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                string userRole = (Session["UserRole"] ?? "Elderly").ToString();

                lblTotalBalance.Text = GetTotalBalance(userId, userRole).ToString("N2");

                rptRecentTransactions.DataSource = GetRecentTransactions(userId, userRole);
                rptRecentTransactions.DataBind();

                var alerts = GetAlerts(userId, userRole);
                rptAlerts.DataSource = alerts;
                rptAlerts.DataBind();
                lblAlertCount.Text = alerts.Count.ToString();
            }
        }

        private decimal GetTotalBalance(int userId, string userRole)
        {
            decimal total = 0;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (userRole == "Elderly")
                {
                    cmd.CommandText = "SELECT ISNULL(SUM(Balance),0) FROM BankAccounts WHERE UserId = @UserId";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                }
                else if (userRole == "Trusted")
                {
                    // Get all elderly user IDs under this trusted contact
                    cmd.CommandText = @"
                        SELECT ISNULL(SUM(ba.Balance),0)
                        FROM FamilyGroupMembers t
                        JOIN FamilyGroupMembers e ON t.GroupId = e.GroupId
                        JOIN BankAccounts ba ON ba.UserId = e.UserId
                        WHERE t.UserId = @UserId AND t.Role = 'Trusted' AND e.Role = 'Elderly'
                    ";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                }
                else
                {
                    // Admin: show all balances
                    cmd.CommandText = "SELECT ISNULL(SUM(Balance),0) FROM BankAccounts";
                }

                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                    total = Convert.ToDecimal(result);
            }
            return total;
        }

        private DataTable GetRecentTransactions(int userId, string userRole)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (userRole == "Elderly")
                {
                    cmd.CommandText = @"
                        SELECT TOP 5 TransactionDate, Description, Amount, IsFlagged, Severity
                        FROM BankTransactions
                        WHERE UserId = @UserId
                        ORDER BY TransactionDate DESC
                    ";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                }
                else if (userRole == "Trusted")
                {
                    // Get all elderly user IDs under this trusted contact
                    cmd.CommandText = @"
                        SELECT TOP 5 bt.TransactionDate, bt.Description, bt.Amount, bt.IsFlagged, bt.Severity
                        FROM FamilyGroupMembers t
                        JOIN FamilyGroupMembers e ON t.GroupId = e.GroupId
                        JOIN BankTransactions bt ON bt.UserId = e.UserId
                        WHERE t.UserId = @UserId AND t.Role = 'Trusted' AND e.Role = 'Elderly'
                        ORDER BY bt.TransactionDate DESC
                    ";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                }
                else
                {
                    // Admin: show all recent
                    cmd.CommandText = @"
                        SELECT TOP 5 TransactionDate, Description, Amount, IsFlagged, Severity
                        FROM BankTransactions
                        ORDER BY TransactionDate DESC
                    ";
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private List<AlertViewModel> GetAlerts(int userId, string userRole)
        {
            var alerts = new List<AlertViewModel>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (userRole == "Elderly")
                {
                    cmd.CommandText = @"
                        SELECT TOP 5 AlertId, AlertType, AlertMessage, AlertDate, TransactionId
                        FROM Alerts
                        WHERE UserId = @UserId AND IsRead = 0
                        ORDER BY AlertDate DESC
                    ";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                }
                else if (userRole == "Trusted")
                {
                    // Trusted: show unread alerts for all elderly users under their care
                    cmd.CommandText = @"
                        SELECT TOP 5 a.AlertId, a.AlertType, a.AlertMessage, a.AlertDate, a.TransactionId
                        FROM FamilyGroupMembers t
                        JOIN FamilyGroupMembers e ON t.GroupId = e.GroupId
                        JOIN Alerts a ON a.UserId = e.UserId
                        WHERE t.UserId = @UserId AND t.Role = 'Trusted' AND e.Role = 'Elderly' AND a.IsRead = 0
                        ORDER BY a.AlertDate DESC
                    ";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                }
                else
                {
                    cmd.CommandText = @"
                        SELECT TOP 5 AlertId, AlertType, AlertMessage, AlertDate, TransactionId
                        FROM Alerts
                        WHERE IsRead = 0
                        ORDER BY AlertDate DESC
                    ";
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        alerts.Add(new AlertViewModel
                        {
                            AlertId = Convert.ToInt32(reader["AlertId"]),
                            AlertType = reader["AlertType"].ToString(),
                            AlertMessage = reader["AlertMessage"].ToString(),
                            AlertDate = Convert.ToDateTime(reader["AlertDate"]),
                            TransactionId = Convert.ToInt32(reader["TransactionId"])
                        });
                    }
                }
            }
            return alerts;
        }

        // Helper for binding
        public class AlertViewModel
        {
            public int AlertId { get; set; }
            public string AlertType { get; set; }
            public string AlertMessage { get; set; }
            public DateTime AlertDate { get; set; }
            public int TransactionId { get; set; }
        }
    }
}
