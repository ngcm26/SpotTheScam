using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace SpotTheScam.User
{
    public partial class WebinarRegistrationSuccess : System.Web.UI.Page
    {
        private string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
        private int currentUserPoints = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                if (Session["Username"] == null || string.IsNullOrEmpty(Session["Username"].ToString()))
                {
                    Response.Redirect("UserLogin.aspx");
                    return;
                }

                LoadUserCurrentPoints();
                LoadSessionDetails();
                LoadSessionLink();
                SetBackToHomeLink();
                UpdatePointsDisplay();
            }
        }

        private void LoadUserCurrentPoints()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from Username
                    string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";
                    int userId = 0;

                    using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                        object userIdResult = cmd.ExecuteScalar();
                        if (userIdResult != null)
                        {
                            userId = Convert.ToInt32(userIdResult);
                            Session["UserID"] = userId;
                        }
                        else
                        {
                            currentUserPoints = 0;
                            return;
                        }
                    }

                    // Get current total points from PointsTransactions table
                    string getPointsQuery = @"
                        SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                        FROM PointsTransactions 
                        WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(getPointsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        currentUserPoints = result != null ? Convert.ToInt32(result) : 0;
                        Session["CurrentPoints"] = currentUserPoints;

                        System.Diagnostics.Debug.WriteLine($"✅ Success page - Current points loaded: {currentUserPoints}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading points on success page: {ex.Message}");
                currentUserPoints = 0;
            }
        }

        private void LoadSessionLink()
        {
            try
            {
                string sessionIdStr = Request.QueryString["sessionId"];

                if (!string.IsNullOrEmpty(sessionIdStr) && int.TryParse(sessionIdStr, out int sessionId))
                {
                    int userId = Convert.ToInt32(Session["UserID"]);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Get the session link from VideoCallBookings
                        string linkQuery = @"
                            SELECT SessionLink 
                            FROM VideoCallBookings 
                            WHERE SessionId = @SessionId AND UserId = @UserId 
                            AND BookingStatus = 'Confirmed'";

                        using (SqlCommand cmd = new SqlCommand(linkQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@SessionId", sessionId);
                            cmd.Parameters.AddWithValue("@UserId", userId);

                            object linkResult = cmd.ExecuteScalar();
                            if (linkResult != null && !string.IsNullOrEmpty(linkResult.ToString()))
                            {
                                lblSessionLink.Text = linkResult.ToString();
                                System.Diagnostics.Debug.WriteLine($"✅ Session link loaded: {linkResult}");
                            }
                            else
                            {
                                // Generate a new session link if one doesn't exist
                                string newSessionLink = GenerateSessionLink(sessionId, userId);
                                UpdateSessionLink(sessionId, userId, newSessionLink);
                                lblSessionLink.Text = newSessionLink;
                                System.Diagnostics.Debug.WriteLine($"✅ New session link generated: {newSessionLink}");
                            }
                        }
                    }
                }
                else
                {
                    // Fallback - generate a basic link
                    lblSessionLink.Text = "Session link will be provided in My Sessions page";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading session link: {ex.Message}");
                lblSessionLink.Text = "Session link will be available in My Sessions page";
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

        private void UpdateSessionLink(int sessionId, int userId, string sessionLink)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string updateQuery = @"
                        UPDATE VideoCallBookings 
                        SET SessionLink = @SessionLink 
                        WHERE SessionId = @SessionId AND UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionLink", sessionLink);
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error updating session link: {ex.Message}");
            }
        }

        private void UpdatePointsDisplay()
        {
            // Set the label directly on the server side
            lblCurrentPoints.Text = currentUserPoints.ToString();

            // Also update with JavaScript as backup
            string script = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    var pointsBadge = document.querySelector('.points-badge span');
                    if (pointsBadge) {{
                        pointsBadge.innerHTML = 'Current Points: {currentUserPoints} ⭐';
                    }}
                    console.log('Success page - Points updated to: {currentUserPoints}');
                }});
            ";
            ClientScript.RegisterStartupScript(this.GetType(), "UpdatePointsDisplay", script, true);
        }

        private void LoadSessionDetails()
        {
            try
            {
                string sessionIdStr = Request.QueryString["sessionId"];
                string firstName = Request.QueryString["firstName"];
                string lastName = Request.QueryString["lastName"];

                // Load session details based on ID from database
                if (!string.IsNullOrEmpty(sessionIdStr) && int.TryParse(sessionIdStr, out int sessionId))
                {
                    LoadSessionFromDatabase(sessionId);
                }
                else
                {
                    SetDefaultSessionDetails();
                }

                // Personalize if user info available
                if (!string.IsNullOrEmpty(firstName))
                {
                    // Could personalize the success message here if needed
                }
            }
            catch (Exception ex)
            {
                // Handle error gracefully
                System.Diagnostics.Debug.WriteLine($"Error loading session details: {ex.Message}");
                SetDefaultSessionDetails();
            }
        }

        private void LoadSessionFromDatabase(int sessionId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT SessionTitle, SessionDate, StartTime, EndTime, ExpertName, ExpertTitle
                        FROM ExpertSessions 
                        WHERE Id = @SessionId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ltSessionName.Text = reader["SessionTitle"].ToString();

                                DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                                string startTime = reader["StartTime"].ToString();
                                string endTime = reader["EndTime"].ToString();

                                ltDateTime.Text = $"{sessionDate:MMMM dd, yyyy} at {startTime} - {endTime}";
                                ltExpertName.Text = reader["ExpertName"].ToString();

                                System.Diagnostics.Debug.WriteLine($"✅ Loaded session details for session {sessionId}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"❌ Session {sessionId} not found, using fallback");
                                LoadSessionById(sessionId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading session from database: {ex.Message}");
                LoadSessionById(sessionId);
            }
        }

        private void LoadSessionById(int sessionId)
        {
            switch (sessionId)
            {
                case 1:
                    ltSessionName.Text = "Protecting Your Online Banking";
                    ltDateTime.Text = DateTime.Today.AddDays(7).ToString("MMMM dd, yyyy") + " at 2:00 PM - 3:00 PM";
                    ltExpertName.Text = "Dr Harvey Blue";
                    break;
                case 2:
                    ltSessionName.Text = "Small Group: Latest Phone Scam Tactics";
                    ltDateTime.Text = DateTime.Today.AddDays(9).ToString("MMMM dd, yyyy") + " at 10:00 AM - 11:30 AM";
                    ltExpertName.Text = "Officer James Wilson";
                    break;
                case 3:
                    ltSessionName.Text = "VIP One-on-One Safety Consultation";
                    ltDateTime.Text = DateTime.Today.AddDays(12).ToString("MMMM dd, yyyy") + " at 3:00 PM - 4:00 PM";
                    ltExpertName.Text = "Maria Rodriguez";
                    break;
                default:
                    SetDefaultSessionDetails();
                    break;
            }
        }

        private void SetDefaultSessionDetails()
        {
            ltSessionName.Text = "Protecting Your Online Banking";
            ltDateTime.Text = DateTime.Today.AddDays(7).ToString("MMMM dd, yyyy") + " at 2:00 PM - 3:00 PM";
            ltExpertName.Text = "Dr Harvey Blue";
        }

        private void SetBackToHomeLink()
        {
            // Set the navigation URLs
            lnkBackToHome.NavigateUrl = "~/User/UserHome.aspx";
            lnkMySession.NavigateUrl = "~/User/UserMySessions.aspx";
        }
    }
}