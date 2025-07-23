using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace SpotTheScam
{
    public partial class JoinSession : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
        private int sessionId;
        private int userId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Get parameters
            if (!int.TryParse(Request.QueryString["sessionId"], out sessionId) ||
                !int.TryParse(Request.QueryString["userId"], out userId))
            {
                ShowError("Invalid session parameters.");
                return;
            }

            // Verify this user matches the logged in user
            int loggedInUserId = Convert.ToInt32(Session["UserId"]);
            if (userId != loggedInUserId)
            {
                ShowError("Access denied. This session link is not for your account.");
                return;
            }

            if (!IsPostBack)
            {
                LoadSessionInfo();
                ValidateSessionAccess();
            }
        }

        private void LoadSessionInfo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            es.SessionTitle,
                            es.SessionDescription,
                            es.SessionDate,
                            CONVERT(varchar, es.StartTime, 108) as StartTime,
                            CONVERT(varchar, es.EndTime, 108) as EndTime,
                            es.ExpertName,
                            es.SessionTopic
                        FROM ExpertSessions es
                        WHERE es.Id = @SessionId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblSessionTitle.Text = reader["SessionTitle"].ToString();

                                DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                                string startTime = reader["StartTime"].ToString();
                                string endTime = reader["EndTime"].ToString();
                                string expertName = reader["ExpertName"].ToString();
                                string topic = reader["SessionTopic"].ToString();
                                string description = reader["SessionDescription"].ToString();

                                string sessionDetails = $@"
                                    <div class='detail-item'>
                                        <span class='detail-icon'>📅</span>
                                        <strong>Date:</strong> {sessionDate:dddd, dd MMMM yyyy}
                                    </div>
                                    <div class='detail-item'>
                                        <span class='detail-icon'>🕐</span>
                                        <strong>Time:</strong> {startTime} - {endTime}
                                    </div>
                                    <div class='detail-item'>
                                        <span class='detail-icon'>👨‍💼</span>
                                        <strong>Expert:</strong> {expertName}
                                    </div>
                                    <div class='detail-item'>
                                        <span class='detail-icon'>📚</span>
                                        <strong>Topic:</strong> {topic}
                                    </div>
                                    <div style='margin-top: 15px;'>
                                        <strong>Description:</strong><br/>
                                        {description}
                                    </div>";

                                lblSessionDetails.Text = sessionDetails;
                            }
                            else
                            {
                                ShowError("Session not found.");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading session information: " + ex.Message);
            }
        }

        private void ValidateSessionAccess()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if user is registered for this session using VideoCallBookings table
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM VideoCallBookings vcb
                        INNER JOIN ExpertSessions es ON vcb.SessionId = es.Id
                        WHERE vcb.SessionId = @SessionId 
                        AND vcb.UserId = @UserId 
                        AND vcb.BookingStatus = 'Confirmed'";

                    using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        int registrationCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (registrationCount == 0)
                        {
                            ShowError("You are not registered for this session or your registration is not confirmed.");
                            return;
                        }
                    }

                    // Check timing - allow joining 10 minutes before session starts
                    string timeQuery = @"
                        SELECT SessionDate, StartTime, EndTime 
                        FROM ExpertSessions 
                        WHERE Id = @SessionId";

                    using (SqlCommand timeCmd = new SqlCommand(timeQuery, conn))
                    {
                        timeCmd.Parameters.AddWithValue("@SessionId", sessionId);
                        using (SqlDataReader reader = timeCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                                TimeSpan startTime = (TimeSpan)reader["StartTime"];
                                TimeSpan endTime = (TimeSpan)reader["EndTime"];

                                DateTime sessionStart = sessionDate.Add(startTime);
                                DateTime sessionEnd = sessionDate.Add(endTime);
                                DateTime joinAllowedTime = sessionStart.AddMinutes(-10);
                                DateTime now = DateTime.Now;

                                if (now < joinAllowedTime)
                                {
                                    TimeSpan waitTime = joinAllowedTime - now;
                                    string waitMessage;

                                    if (waitTime.TotalDays >= 1)
                                    {
                                        int days = (int)waitTime.TotalDays;
                                        waitMessage = $"You can join this session in {days} day{(days > 1 ? "s" : "")} and {waitTime.Hours} hour{(waitTime.Hours != 1 ? "s" : "")}.";
                                    }
                                    else if (waitTime.TotalHours >= 1)
                                    {
                                        waitMessage = $"You can join this session in {waitTime.Hours} hour{(waitTime.Hours != 1 ? "s" : "")} and {waitTime.Minutes} minute{(waitTime.Minutes != 1 ? "s" : "")}.";
                                    }
                                    else
                                    {
                                        waitMessage = $"You can join this session in {waitTime.Minutes} minute{(waitTime.Minutes != 1 ? "s" : "")}.";
                                    }

                                    ShowAlert(waitMessage + " Please return at the designated time.", "warning");
                                    pnlJoinActions.Visible = false;
                                    return;
                                }
                                else if (now > sessionEnd)
                                {
                                    ShowAlert("This session has already ended.", "info");
                                    pnlJoinActions.Visible = false;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error validating session access: " + ex.Message);
            }
        }

        protected void btnJoinSession_Click(object sender, EventArgs e)
        {
            try
            {
                // CORRECTED: Redirect to the User VideoCall page for participants
                Response.Redirect($"/User/VideoCall.aspx?sessionId={sessionId}&userId={userId}");
            }
            catch (Exception ex)
            {
                ShowAlert("Error joining session: " + ex.Message, "error");
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            pnlError.Visible = true;
            pnlJoinActions.Visible = false;
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = "alert alert-" + type;
            AlertMessage.Text = message;
            AlertPanel.Visible = true;
        }
    }
}