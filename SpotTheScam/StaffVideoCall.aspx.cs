using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam
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

                // Update session status to completed
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE ExpertWebinarSessions 
                                   SET Status = 'Completed', IsActive = 0 
                                   WHERE Id = @SessionId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SessionId", sessionId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Return to join form
                VideoCallInterface.Attributes["class"] = "video-container hidden";
                JoinForm.Attributes["class"] = "join-form";

                ShowAlert("Call ended successfully. Session marked as completed.", "success");
                LoadActiveWaitingSessions();
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
                    string query = @"SELECT Id, SessionDate, StartTime, EndTime, Status, 
                                           CustomerName, CustomerPhone, ScamConcerns, 
                                           ExpertName, JoinedAt
                                    FROM ExpertWebinarSessions 
                                    WHERE (Id = @Input OR CustomerPhone = @Input) 
                                    AND Status = 'Booked' AND IsActive = 1";

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
                        // Try to find any booked session (not necessarily active)
                        reader.Close();

                        string fallbackQuery = @"SELECT Id, SessionDate, StartTime, EndTime, Status, 
                                               CustomerName, CustomerPhone, ScamConcerns
                                               FROM ExpertWebinarSessions 
                                               WHERE (Id = @Input OR CustomerPhone = @Input) 
                                               AND Status = 'Booked'";

                        SqlCommand fallbackCmd = new SqlCommand(fallbackQuery, conn);
                        fallbackCmd.Parameters.AddWithValue("@Input", input);

                        SqlDataReader fallbackReader = fallbackCmd.ExecuteReader();

                        if (fallbackReader.Read())
                        {
                            ShowAlert($"Found session for {fallbackReader["CustomerName"]}, but customer hasn't joined yet. They need to join first using their phone number.", "error");
                        }
                        else
                        {
                            ShowAlert("No active session found with that ID or phone number. Please check and try again.", "error");
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
                    string query = @"SELECT Id, CustomerName, CustomerPhone, ScamConcerns, JoinedAt
                                    FROM ExpertWebinarSessions 
                                    WHERE Status = 'Booked' AND IsActive = 1 
                                    ORDER BY JoinedAt ASC";

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