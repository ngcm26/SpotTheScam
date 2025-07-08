using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.Staff
{
    public partial class StaffExpertWebinar : System.Web.UI.Page
    {
        private string connectionString;

        public StaffExpertWebinar()
        {
            // Use the existing connection string name from web.config
            var connStr = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"];

            if (connStr != null)
            {
                connectionString = connStr.ConnectionString;
            }
            else
            {
                throw new ConfigurationErrorsException("Connection string 'SpotTheScamConnectionString' not found in web.config");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // For now, we'll skip staff authentication for testing
            // You can enable this later when you have staff login functionality
            /*
            if (!IsStaffLoggedIn())
            {
                Response.Redirect("../StaffLogin.aspx");
                return;
            }
            */

            if (!IsPostBack)
            {
                // Set minimum date to today
                SessionDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
                LoadSessions();
            }
        }

        private bool IsStaffLoggedIn()
        {
            return Session["StaffId"] != null &&
                   Session["StaffId"].ToString() != "0" &&
                   !string.IsNullOrEmpty(Session["StaffId"].ToString());
        }

        protected void AddTimeSlotButton_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime sessionDate = DateTime.Parse(SessionDate.Text);
                TimeSpan startTime = TimeSpan.Parse(StartTime.Text);
                TimeSpan endTime = TimeSpan.Parse(EndTime.Text);

                // Validate that end time is after start time
                if (endTime <= startTime)
                {
                    ShowAlert("End time must be after start time.", "error");
                    return;
                }

                // Validate that the date is not in the past
                if (sessionDate.Date < DateTime.Now.Date)
                {
                    ShowAlert("Cannot create sessions for past dates.", "error");
                    return;
                }

                // Validate session duration (minimum 30 minutes, maximum 4 hours)
                double durationHours = (endTime - startTime).TotalHours;
                if (durationHours < 0.5)
                {
                    ShowAlert("Session must be at least 30 minutes long.", "error");
                    return;
                }
                if (durationHours > 4)
                {
                    ShowAlert("Session cannot be longer than 4 hours.", "error");
                    return;
                }

                // Check if the time slot already exists or overlaps
                if (IsTimeSlotConflict(sessionDate, startTime, endTime))
                {
                    ShowAlert("This time slot conflicts with an existing session.", "error");
                    return;
                }

                // Get current staff member ID
                int staffId = GetCurrentStaffId();

                // Insert new session into WebinarSessions table (not ExpertSessions)
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        INSERT INTO WebinarSessions 
                        (Title, Description, SessionDate, StartTime, EndTime, MaxParticipants, 
                         PointsRequired, SessionType, ExpertId, IsActive, CreatedDate, CreatedBy) 
                        VALUES 
                        (@Title, @Description, @SessionDate, @StartTime, @EndTime, @MaxParticipants,
                         @PointsRequired, @SessionType, @ExpertId, 1, @CreatedDate, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Default values for expert consultation session
                        cmd.Parameters.AddWithValue("@Title", "Expert Security Consultation");
                        cmd.Parameters.AddWithValue("@Description", "One-on-one expert consultation session for personalized security advice and scam prevention strategies.");
                        cmd.Parameters.AddWithValue("@SessionDate", sessionDate.Date);
                        cmd.Parameters.AddWithValue("@StartTime", startTime.ToString(@"hh\:mm"));
                        cmd.Parameters.AddWithValue("@EndTime", endTime.ToString(@"hh\:mm"));
                        cmd.Parameters.AddWithValue("@MaxParticipants", 1); // One-on-one session
                        cmd.Parameters.AddWithValue("@PointsRequired", 100); // Premium session
                        cmd.Parameters.AddWithValue("@SessionType", "VIP Premium");
                        cmd.Parameters.AddWithValue("@ExpertId", GetDefaultExpertId()); // Get or create default expert
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CreatedBy", staffId);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            ShowAlert("Time slot added successfully!", "success");
                            ClearForm();
                            LoadSessions();
                        }
                        else
                        {
                            ShowAlert("Failed to add time slot. Please try again.", "error");
                        }
                    }
                }
            }
            catch (FormatException)
            {
                ShowAlert("Please enter valid date and time values.", "error");
            }
            catch (Exception ex)
            {
                ShowAlert("Error adding time slot: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error adding time slot: {ex.Message}");
            }
        }

        private int GetDefaultExpertId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // First, try to get an existing expert
                    string checkQuery = "SELECT TOP 1 ExpertId FROM Experts WHERE IsActive = 1";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        conn.Open();
                        object result = checkCmd.ExecuteScalar();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                    }

                    // If no expert exists, create a default one
                    string insertQuery = @"
                        INSERT INTO Experts (ExpertName, ExpertTitle, ExpertImage, Bio, Specialization, IsActive)
                        VALUES (@ExpertName, @ExpertTitle, @ExpertImage, @Bio, @Specialization, 1);
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@ExpertName", "Financial Security Expert");
                        insertCmd.Parameters.AddWithValue("@ExpertTitle", "Senior Security Consultant, 15+ years experience");
                        insertCmd.Parameters.AddWithValue("@ExpertImage", "/Images/default-expert.jpg");
                        insertCmd.Parameters.AddWithValue("@Bio", "Experienced financial security consultant specializing in scam prevention and digital safety.");
                        insertCmd.Parameters.AddWithValue("@Specialization", "Financial Security & Scam Prevention");

                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        object newId = insertCmd.ExecuteScalar();
                        return Convert.ToInt32(newId);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting/creating expert: {ex.Message}");
                return 1; // Default fallback
            }
        }

        protected void SessionsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteSession")
            {
                try
                {
                    int sessionId = Convert.ToInt32(e.CommandArgument);

                    // Check if session has registrations before deleting
                    if (HasRegistrations(sessionId))
                    {
                        ShowAlert("Cannot delete a session that has active registrations.", "error");
                        return;
                    }

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE WebinarSessions SET IsActive = 0 WHERE SessionId = @SessionId";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@SessionId", sessionId);
                            conn.Open();
                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                ShowAlert("Time slot deleted successfully!", "success");
                                LoadSessions();
                            }
                            else
                            {
                                ShowAlert("Failed to delete time slot.", "error");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting time slot: " + ex.Message, "error");
                    System.Diagnostics.Debug.WriteLine($"Error deleting session: {ex.Message}");
                }
            }
        }

        private void LoadSessions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Updated query to work with WebinarSessions table
                    string query = @"
                        SELECT 
                            ws.SessionId as Id,
                            ws.SessionDate,
                            ws.StartTime,
                            ws.EndTime,
                            CASE 
                                WHEN COUNT(wr.RegistrationId) >= ws.MaxParticipants THEN 'Booked'
                                WHEN COUNT(wr.RegistrationId) > 0 THEN 'Partially Booked'
                                ELSE 'Available'
                            END as Status,
                            ISNULL(
                                STUFF((
                                    SELECT ', ' + wr2.FirstName + ' ' + wr2.LastName
                                    FROM WebinarRegistrations wr2 
                                    WHERE wr2.SessionId = ws.SessionId AND wr2.IsActive = 1
                                    FOR XML PATH('')
                                ), 1, 2, '')
                            , '') as CustomerName,
                            ISNULL(
                                STUFF((
                                    SELECT ', ' + wr3.Phone
                                    FROM WebinarRegistrations wr3 
                                    WHERE wr3.SessionId = ws.SessionId AND wr3.IsActive = 1
                                    FOR XML PATH('')
                                ), 1, 2, '')
                            , '') as CustomerPhone,
                            ISNULL(
                                STUFF((
                                    SELECT ', ' + wr4.SecurityConcerns
                                    FROM WebinarRegistrations wr4 
                                    WHERE wr4.SessionId = ws.SessionId AND wr4.IsActive = 1 AND wr4.SecurityConcerns IS NOT NULL AND wr4.SecurityConcerns != ''
                                    FOR XML PATH('')
                                ), 1, 2, '')
                            , '') as ScamConcerns
                        FROM WebinarSessions ws
                        LEFT JOIN WebinarRegistrations wr ON ws.SessionId = wr.SessionId AND wr.IsActive = 1
                        WHERE ws.IsActive = 1 AND ws.SessionDate >= CAST(GETDATE() AS DATE)
                        GROUP BY ws.SessionId, ws.SessionDate, ws.StartTime, ws.EndTime, ws.MaxParticipants
                        ORDER BY ws.SessionDate, ws.StartTime";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        SessionsGridView.DataSource = dt;
                        SessionsGridView.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading sessions: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error loading sessions: {ex.Message}");
            }
        }

        private bool IsTimeSlotConflict(DateTime sessionDate, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT COUNT(*) FROM WebinarSessions 
                        WHERE IsActive = 1 
                        AND SessionDate = @SessionDate 
                        AND (
                            (CAST(StartTime AS TIME) <= @StartTime AND CAST(EndTime AS TIME) > @StartTime) 
                            OR (CAST(StartTime AS TIME) < @EndTime AND CAST(EndTime AS TIME) >= @EndTime)
                            OR (CAST(StartTime AS TIME) >= @StartTime AND CAST(EndTime AS TIME) <= @EndTime)
                        )";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionDate", sessionDate.Date);
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);

                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking time slot conflict: {ex.Message}");
                return false; // If there's an error, assume no conflict
            }
        }

        private bool HasRegistrations(int sessionId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM WebinarRegistrations WHERE SessionId = @SessionId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking registrations: {ex.Message}");
                return true; // If there's an error, prevent deletion
            }
        }

        private int GetCurrentStaffId()
        {
            // Check if staff is logged in and return their ID
            if (Session["StaffId"] != null)
            {
                return Convert.ToInt32(Session["StaffId"]);
            }

            // For now, return a default staff ID for testing
            // You can change this later when you have proper staff authentication
            return 1; // Default test staff ID
        }

        private void ClearForm()
        {
            SessionDate.Text = string.Empty;
            StartTime.Text = string.Empty;
            EndTime.Text = string.Empty;
        }

        private void ShowAlert(string message, string type)
        {
            string cssClass = type == "success" ? "alert alert-success" : "alert alert-error";
            AlertPanel.CssClass = cssClass;
            AlertMessage.Text = message;
            AlertPanel.Visible = true;

            // Hide alert after 5 seconds using JavaScript
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
    }
}