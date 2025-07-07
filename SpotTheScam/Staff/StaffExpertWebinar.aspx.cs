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
        // Get connection string from web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set minimum date to today
                SessionDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
                LoadSessions();
            }
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

                // Check if the time slot already exists
                if (IsTimeSlotExists(sessionDate, startTime, endTime))
                {
                    ShowAlert("This time slot already exists.", "error");
                    return;
                }

                // Get current staff member ID (default to 1 for now)
                int staffId = GetCurrentStaffId();

                // Insert new session into database using the correct table and column names
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO ExpertSessions 
                                   (SessionDate, StartTime, EndTime, SessionType, SessionTitle, 
                                    SessionDescription, ExpertName, ExpertTitle, MaxParticipants, 
                                    CurrentParticipants, PointsCost, Status, SessionTopic, CreatedBy) 
                                   VALUES 
                                   (@SessionDate, @StartTime, @EndTime, 'Individual', 'Expert Consultation', 
                                    'One-on-one expert consultation session', 'Financial Security Expert', 
                                    'Senior Security Consultant', 1, 0, 50, 'Available', 'Security Consultation', @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionDate", sessionDate.Date);
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);
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
            catch (Exception ex)
            {
                ShowAlert("Error adding time slot: " + ex.Message, "error");
            }
        }

        protected void SessionsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteSession")
            {
                try
                {
                    int sessionId = Convert.ToInt32(e.CommandArgument);

                    // Check if session is booked before deleting
                    if (IsSessionBooked(sessionId))
                    {
                        ShowAlert("Cannot delete a session that has been booked.", "error");
                        return;
                    }

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM ExpertSessions WHERE Id = @SessionId";
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
                }
            }
        }

        private void LoadSessions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Check if the ExpertSessions table exists first
                    string checkTableQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                                             WHERE TABLE_NAME = 'ExpertSessions'";

                    using (SqlCommand checkCmd = new SqlCommand(checkTableQuery, conn))
                    {
                        conn.Open();
                        int tableExists = (int)checkCmd.ExecuteScalar();

                        if (tableExists == 0)
                        {
                            ShowAlert("Database tables not found. Please set up the database first.", "error");
                            return;
                        }
                    }

                    string query = @"SELECT 
                                        es.Id,
                                        es.SessionDate,
                                        es.StartTime,
                                        es.EndTime,
                                        CASE 
                                            WHEN vcb.BookingId IS NOT NULL THEN 'Booked'
                                            ELSE 'Available'
                                        END as Status,
                                        ISNULL(vcb.CustomerName, '') as CustomerName,
                                        ISNULL(vcb.CustomerPhone, '') as CustomerPhone,
                                        ISNULL(vcb.ScamConcerns, '') as ScamConcerns
                                    FROM ExpertSessions es
                                    LEFT JOIN VideoCallBookings vcb ON es.Id = vcb.SessionId AND vcb.BookingStatus = 'Confirmed'
                                    WHERE es.SessionDate >= CAST(GETDATE() AS DATE)
                                    ORDER BY es.SessionDate, es.StartTime";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (conn.State != ConnectionState.Open)
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
            }
        }

        private bool IsTimeSlotExists(DateTime sessionDate, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT COUNT(*) FROM ExpertSessions 
                                   WHERE SessionDate = @SessionDate 
                                   AND ((StartTime <= @StartTime AND EndTime > @StartTime) 
                                        OR (StartTime < @EndTime AND EndTime >= @EndTime)
                                        OR (StartTime >= @StartTime AND EndTime <= @EndTime))";

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
            catch
            {
                return false; // If there's an error, assume no conflict
            }
        }

        private bool IsSessionBooked(int sessionId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = @SessionId AND BookingStatus = 'Confirmed'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false; // If there's an error, allow deletion
            }
        }

        private int GetCurrentStaffId()
        {
            // Check if staff is logged in and return their ID
            if (Session["StaffId"] != null)
            {
                return Convert.ToInt32(Session["StaffId"]);
            }
            else if (Session["StaffName"] != null)
            {
                // If only StaffName is stored, you might need to look up the ID
                // For now, return 1 as default
                return 1;
            }
            return 1; // Default staff ID - you should implement proper authentication
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