using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.Staff
{
    public partial class StaffVideoCall : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadActiveWaitingSessions();

                // Check if sessionId is provided in URL
                string sessionId = Request.QueryString["sessionId"];
                if (!string.IsNullOrEmpty(sessionId))
                {
                    JoinSessionById(sessionId);
                }
            }

            // Handle refresh postback
            string eventTarget = Request["__EVENTTARGET"];
            if (eventTarget == "RefreshSessions")
            {
                LoadActiveWaitingSessions();
            }
        }

        protected void JoinSessionButton_Click(object sender, EventArgs e)
        {
            string input = SessionIdInput.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                ShowAlert("Please enter a Session ID or phone number", "error");
                return;
            }

            JoinSessionById(input);
        }

        protected void QuickJoinButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string sessionId = btn.CommandArgument;
            JoinSessionById(sessionId);
        }

        protected void EndCallButton_Click(object sender, EventArgs e)
        {
            try
            {
                string sessionId = SessionIdLabel.Text;

                // Update booking status to completed
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE VideoCallBookings 
                                   SET BookingStatus = 'Completed' 
                                   WHERE SessionId = @SessionId AND BookingStatus = 'Confirmed'";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SessionId", sessionId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Return to join form
                        VideoCallInterface.Attributes["class"] = "video-container hidden";
                        JoinForm.Attributes["class"] = "join-form";

                        ShowAlert("Call ended successfully. Session marked as completed.", "success");
                        LoadActiveWaitingSessions();
                    }
                    else
                    {
                        ShowAlert("No active booking found to end.", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error ending call: " + ex.Message, "error");
            }
        }

        private void JoinSessionById(string input)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Try to find session by ID or phone number
                    string query = @"SELECT 
                                        es.Id, 
                                        es.SessionDate, 
                                        es.StartTime, 
                                        es.EndTime, 
                                        vcb.CustomerName, 
                                        vcb.CustomerPhone, 
                                        vcb.ScamConcerns,
                                        vcb.BookingDate,
                                        vcb.BookingStatus,
                                        es.ExpertName
                                    FROM ExpertSessions es
                                    INNER JOIN VideoCallBookings vcb ON es.Id = vcb.SessionId
                                    WHERE (es.Id = @Input OR vcb.CustomerPhone = @Input) 
                                    AND vcb.BookingStatus = 'Confirmed'
                                    AND es.SessionDate = CAST(GETDATE() AS DATE)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Input", input);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Found active session
                        LoadVideoCallInterface(reader);
                        ShowAlert("Successfully joined session! You can now help the customer learn about scam prevention.", "success");
                    }
                    else
                    {
                        // Try to find any booked session (not necessarily today)
                        reader.Close();

                        string fallbackQuery = @"SELECT 
                                               es.Id, 
                                               es.SessionDate, 
                                               vcb.CustomerName, 
                                               vcb.CustomerPhone, 
                                               vcb.ScamConcerns,
                                               vcb.BookingStatus
                                               FROM ExpertSessions es
                                               INNER JOIN VideoCallBookings vcb ON es.Id = vcb.SessionId
                                               WHERE (es.Id = @Input OR vcb.CustomerPhone = @Input) 
                                               AND vcb.BookingStatus = 'Confirmed'
                                               ORDER BY es.SessionDate DESC";

                        SqlCommand fallbackCmd = new SqlCommand(fallbackQuery, conn);
                        fallbackCmd.Parameters.AddWithValue("@Input", input);

                        SqlDataReader fallbackReader = fallbackCmd.ExecuteReader();

                        if (fallbackReader.Read())
                        {
                            DateTime sessionDate = Convert.ToDateTime(fallbackReader["SessionDate"]);
                            if (sessionDate.Date == DateTime.Today)
                            {
                                ShowAlert($"Found session for {fallbackReader["CustomerName"]}, but they may not have joined yet. Please try again in a moment.", "error");
                            }
                            else
                            {
                                ShowAlert($"Found session for {fallbackReader["CustomerName"]} on {sessionDate:dd/MM/yyyy}, but it's not scheduled for today.", "error");
                            }
                        }
                        else
                        {
                            ShowAlert("No session found with that ID or phone number. Please check and try again.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error joining session: " + ex.Message, "error");
            }
        }

        private void LoadVideoCallInterface(SqlDataReader reader)
        {
            // Populate session details
            SessionIdLabel.Text = reader["Id"].ToString();
            CustomerNameLabel.Text = reader["CustomerName"].ToString();
            CustomerPhoneLabel.Text = reader["CustomerPhone"].ToString();
            SessionDateLabel.Text = Convert.ToDateTime(reader["SessionDate"]).ToString("dd/MM/yyyy");
            SessionTimeLabel.Text = ((TimeSpan)reader["StartTime"]).ToString(@"hh\:mm");
            ScamConcernsLabel.Text = reader["ScamConcerns"].ToString();

            // Show video interface, hide join form
            VideoCallInterface.Attributes["class"] = "video-container";
            JoinForm.Attributes["class"] = "join-form hidden";
        }

        private void LoadActiveWaitingSessions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Get sessions that are booked for today and customers are waiting
                    string query = @"SELECT 
                                        es.Id, 
                                        vcb.CustomerName, 
                                        vcb.CustomerPhone, 
                                        vcb.ScamConcerns, 
                                        vcb.BookingDate as JoinedAt
                                    FROM ExpertSessions es
                                    INNER JOIN VideoCallBookings vcb ON es.Id = vcb.SessionId
                                    WHERE vcb.BookingStatus = 'Confirmed' 
                                    AND es.SessionDate = CAST(GETDATE() AS DATE)
                                    AND es.StartTime <= CAST(GETDATE() AS TIME)
                                    AND es.EndTime >= CAST(GETDATE() AS TIME)
                                    ORDER BY vcb.BookingDate ASC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        ActiveSessionsRepeater.DataSource = dt;
                        ActiveSessionsRepeater.DataBind();
                        NoActiveSessionsPanel.Visible = false;
                    }
                    else
                    {
                        ActiveSessionsRepeater.DataSource = null;
                        ActiveSessionsRepeater.DataBind();
                        NoActiveSessionsPanel.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading active sessions: " + ex.Message, "error");
                // Still show the no sessions panel if there's an error
                NoActiveSessionsPanel.Visible = true;
            }
        }

        private void ShowAlert(string message, string type)
        {
            string cssClass = type == "success" ? "alert alert-success" : "alert alert-error";
            AlertPanel.CssClass = cssClass;
            AlertMessage.Text = message;
            AlertPanel.Visible = true;

            // Hide alert after 8 seconds
            string script = @"
                setTimeout(function() {
                    var alertPanel = document.getElementById('" + AlertPanel.ClientID + @"');
                    if (alertPanel) alertPanel.style.display = 'none';
                }, 8000);
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideAlert", script, true);
        }
    }
}