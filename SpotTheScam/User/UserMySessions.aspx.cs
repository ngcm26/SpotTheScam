using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class UserMySessions : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                Response.Redirect("UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadUserSessions();
            }
        }

        private void LoadUserSessions()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine($"=== LoadUserSessions for UserId: {userId} ===");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First, check if SessionLink column exists
                    string checkColumnQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'VideoCallBookings' 
                        AND COLUMN_NAME = 'SessionLink'";

                    bool sessionLinkExists = false;
                    using (SqlCommand checkCmd = new SqlCommand(checkColumnQuery, conn))
                    {
                        int columnExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        sessionLinkExists = columnExists > 0;
                        System.Diagnostics.Debug.WriteLine($"SessionLink column exists: {sessionLinkExists}");
                    }

                    // Build query based on whether SessionLink column exists
                    string query;
                    if (sessionLinkExists)
                    {
                        query = @"
                            SELECT 
                                es.Id,
                                es.SessionTitle,
                                es.SessionDescription,
                                es.SessionDate,
                                CONVERT(varchar, es.StartTime, 108) as StartTime,
                                CONVERT(varchar, es.EndTime, 108) as EndTime,
                                es.ExpertName,
                                ISNULL(es.SessionTopic, 'General') as SessionTopic,
                                ISNULL(vcb.PointsUsed, 0) as PointsUsed,
                                vcb.BookingDate as RegistrationDate,
                                ISNULL(vcb.SessionLink, '') as SessionLink
                            FROM VideoCallBookings vcb
                            INNER JOIN ExpertSessions es ON vcb.SessionId = es.Id
                            WHERE vcb.UserId = @UserId AND vcb.BookingStatus = 'Confirmed'
                            ORDER BY es.SessionDate DESC, es.StartTime DESC";
                    }
                    else
                    {
                        query = @"
                            SELECT 
                                es.Id,
                                es.SessionTitle,
                                es.SessionDescription,
                                es.SessionDate,
                                CONVERT(varchar, es.StartTime, 108) as StartTime,
                                CONVERT(varchar, es.EndTime, 108) as EndTime,
                                es.ExpertName,
                                ISNULL(es.SessionTopic, 'General') as SessionTopic,
                                ISNULL(vcb.PointsUsed, 0) as PointsUsed,
                                vcb.BookingDate as RegistrationDate,
                                '' as SessionLink
                            FROM VideoCallBookings vcb
                            INNER JOIN ExpertSessions es ON vcb.SessionId = es.Id
                            WHERE vcb.UserId = @UserId AND vcb.BookingStatus = 'Confirmed'
                            ORDER BY es.SessionDate DESC, es.StartTime DESC";
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@UserId", userId);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        System.Diagnostics.Debug.WriteLine($"Found {dt.Rows.Count} user sessions");

                        if (dt.Rows.Count > 0)
                        {
                            // Generate session links for sessions that don't have them
                            foreach (DataRow row in dt.Rows)
                            {
                                if (string.IsNullOrEmpty(row["SessionLink"].ToString()))
                                {
                                    int sessionId = Convert.ToInt32(row["Id"]);
                                    string sessionLink = GenerateSessionLink(sessionId, userId);

                                    // Only update database if SessionLink column exists
                                    if (sessionLinkExists)
                                    {
                                        UpdateSessionLink(sessionId, userId, sessionLink);
                                    }

                                    row["SessionLink"] = sessionLink;
                                    System.Diagnostics.Debug.WriteLine($"Generated session link for session {sessionId}");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"Session {row["Id"]} already has link: {row["SessionLink"]}");
                                }
                            }

                            rptSessions.DataSource = dt;
                            rptSessions.DataBind();
                            pnlNoSessions.Visible = false;
                        }
                        else
                        {
                            rptSessions.DataSource = null;
                            rptSessions.DataBind();
                            pnlNoSessions.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading your sessions: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error loading user sessions: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void UpdateSessionLink(int sessionId, int userId, string sessionLink)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if SessionLink column exists before updating
                    string checkColumnQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'VideoCallBookings' 
                        AND COLUMN_NAME = 'SessionLink'";

                    using (SqlCommand checkCmd = new SqlCommand(checkColumnQuery, conn))
                    {
                        int columnExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (columnExists == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("SessionLink column does not exist - skipping update");
                            return;
                        }
                    }

                    string updateQuery = @"
                        UPDATE VideoCallBookings 
                        SET SessionLink = @SessionLink 
                        WHERE SessionId = @SessionId AND UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionLink", sessionLink);
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int rowsUpdated = cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Updated {rowsUpdated} rows with session link");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating session link: {ex.Message}");
                // Don't throw the exception - just log it, so the page still works
            }
        }

        private string GenerateSessionLink(int sessionId, int userId)
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
            string token = GenerateSecureToken(sessionId, userId);
            return $"{baseUrl}/User/JoinSession.aspx?sessionId={sessionId}&userId={userId}&token={token}";
        }

        private string GenerateSecureToken(int sessionId, int userId)
        {
            // Simple token generation - you might want to make this more secure
            string data = $"{sessionId}-{userId}-{DateTime.Now.Ticks}";
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data))
                   .Replace("=", "").Replace("+", "-").Replace("/", "_").Substring(0, 16);
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = type == "success" ? "alert alert-success" :
                                 type == "warning" ? "alert alert-warning" : "alert alert-error";
            AlertMessage.Text = message;
            AlertPanel.Visible = true;
        }
    }
}