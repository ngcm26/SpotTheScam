using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.Staff
{
    public partial class StaffManageSessions : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if staff is logged in
                if (Session["StaffName"] == null)
                {
                    Response.Redirect("StaffLogin.aspx");
                    return;
                }

                LoadSessions();
                SetMinimumDate();
            }
        }

        private void SetMinimumDate()
        {
            txtSessionDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private void LoadSessions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            es.Id,
                            es.SessionTitle,
                            es.SessionDescription,
                            es.SessionDate,
                            CONVERT(varchar, es.StartTime, 108) as StartTime,
                            CONVERT(varchar, es.EndTime, 108) as EndTime,
                            es.MaxParticipants,
                            ISNULL(es.PointsCost, 0) as PointsCost,
                            es.SessionType,
                            es.Status,
                            ISNULL(es.SessionTopic, 'General') as SessionTopic,
                            es.ExpertName,
                            ISNULL(es.ExpertTitle, '') as ExpertTitle,
                            ISNULL(es.CurrentParticipants, 0) as CurrentParticipants,
                            (es.MaxParticipants - ISNULL(es.CurrentParticipants, 0)) as AvailableSpots,
                            es.CreatedDate,
                            (SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = es.Id AND BookingStatus = 'Confirmed') as RegistrationCount
                        FROM ExpertSessions es
                        ORDER BY es.SessionDate DESC, es.StartTime DESC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        System.Diagnostics.Debug.WriteLine($"=== LoadSessions ===");
                        System.Diagnostics.Debug.WriteLine($"Loaded {dt.Rows.Count} sessions from database");

                        if (dt.Rows.Count > 0)
                        {
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
                ShowAlert("Error loading sessions: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error loading sessions: {ex.Message}");
            }
        }

        protected void rptSessions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                string commandName = e.CommandName.ToLower();
                string commandArg = e.CommandArgument?.ToString();

                System.Diagnostics.Debug.WriteLine($"=== ItemCommand Event Triggered ===");
                System.Diagnostics.Debug.WriteLine($"Command Name: {commandName}");
                System.Diagnostics.Debug.WriteLine($"Command Argument: {commandArg}");
                System.Diagnostics.Debug.WriteLine($"Source Type: {source.GetType().Name}");

                if (string.IsNullOrEmpty(commandArg))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Command argument is null or empty");
                    ShowAlert("Invalid session ID", "error");
                    return;
                }

                if (!int.TryParse(commandArg, out int sessionId))
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Cannot parse session ID: {commandArg}");
                    ShowAlert("Invalid session ID format", "error");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Parsed Session ID: {sessionId}");

                switch (commandName)
                {
                    case "edit":
                        System.Diagnostics.Debug.WriteLine($"Processing EDIT command for session {sessionId}");
                        EditSession(sessionId);
                        break;
                    case "delete":
                        System.Diagnostics.Debug.WriteLine($"Processing DELETE command for session {sessionId}");
                        DeleteSession(sessionId);
                        break;
                    case "cancelregistrations":
                        System.Diagnostics.Debug.WriteLine($"Processing CANCEL REGISTRATIONS command for session {sessionId}");
                        CancelAllRegistrations(sessionId);
                        break;
                    case "forcedelete":
                        System.Diagnostics.Debug.WriteLine($"Processing FORCE DELETE command for session {sessionId}");
                        ForceDeleteSession(sessionId);
                        break;
                    case "manageparticipants":
                        System.Diagnostics.Debug.WriteLine($"Processing MANAGE PARTICIPANTS command for session {sessionId}");
                        Response.Redirect($"StaffManageParticipants.aspx?sessionId={sessionId}");
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"ERROR: Unknown command: {commandName}");
                        ShowAlert($"Unknown command: {commandName}", "error");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR in ItemCommand ===");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                ShowAlert("Error processing command: " + ex.Message, "error");
            }
        }

        private void EditSession(int sessionId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== EditSession called with ID: {sessionId} ===");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT 
                        Id, SessionTitle, SessionDescription, SessionDate, 
                        StartTime, EndTime, SessionType, MaxParticipants, 
                        PointsCost, SessionTopic, ExpertName, ExpertTitle 
                        FROM ExpertSessions WHERE Id = @SessionId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine("Session found, populating form");

                                // Populate the form with existing data
                                hdnEditingSessionId.Value = sessionId.ToString();
                                txtTitle.Text = reader["SessionTitle"]?.ToString() ?? "";
                                txtDescription.Text = reader["SessionDescription"]?.ToString() ?? "";

                                if (reader["SessionDate"] != DBNull.Value)
                                {
                                    DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                                    txtSessionDate.Text = sessionDate.ToString("yyyy-MM-dd");
                                }

                                if (reader["StartTime"] != DBNull.Value)
                                {
                                    TimeSpan startTime = (TimeSpan)reader["StartTime"];
                                    txtStartTime.Text = startTime.ToString(@"hh\:mm");
                                }

                                if (reader["EndTime"] != DBNull.Value)
                                {
                                    TimeSpan endTime = (TimeSpan)reader["EndTime"];
                                    txtEndTime.Text = endTime.ToString(@"hh\:mm");
                                }

                                // Set dropdown values with safety checks
                                string sessionType = reader["SessionType"]?.ToString() ?? "";
                                if (ddlSessionType.Items.FindByValue(sessionType) != null)
                                    ddlSessionType.SelectedValue = sessionType;

                                txtMaxParticipants.Text = reader["MaxParticipants"]?.ToString() ?? "";
                                txtPointsCost.Text = reader["PointsCost"]?.ToString() ?? "0";

                                string sessionTopic = reader["SessionTopic"]?.ToString() ?? "General";
                                if (ddlTopic.Items.FindByValue(sessionTopic) != null)
                                    ddlTopic.SelectedValue = sessionTopic;

                                string expertName = reader["ExpertName"]?.ToString() ?? "";
                                if (ddlExpertName.Items.FindByValue(expertName) != null)
                                    ddlExpertName.SelectedValue = expertName;

                                txtExpertTitle.Text = reader["ExpertTitle"]?.ToString() ?? "";
                                btnSave.Text = "Update Session";

                                // Show the form
                                string showFormScript = @"
                                    setTimeout(function() {
                                        var form = document.getElementById('createForm');
                                        if (form) {
                                            form.style.display = 'block';
                                            form.scrollIntoView({ behavior: 'smooth' });
                                        }
                                    }, 100);
                                ";
                                ClientScript.RegisterStartupScript(this.GetType(), "ShowEditForm", showFormScript, true);

                                System.Diagnostics.Debug.WriteLine($"Edit form populated for session {sessionId}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Session not found");
                                ShowAlert("Session not found", "error");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading session for editing: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error editing session: {ex.Message}");
            }
        }

        private void DeleteSession(int sessionId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== DeleteSession called with ID: {sessionId} ===");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check for existing registrations in VideoCallBookings
                    string checkRegistrationsQuery = "SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = @SessionId AND BookingStatus = 'Confirmed'";
                    int registrationCount = 0;

                    using (SqlCommand checkRegCmd = new SqlCommand(checkRegistrationsQuery, conn))
                    {
                        checkRegCmd.Parameters.AddWithValue("@SessionId", sessionId);
                        registrationCount = Convert.ToInt32(checkRegCmd.ExecuteScalar());
                        System.Diagnostics.Debug.WriteLine($"Found {registrationCount} registrations for session {sessionId}");
                    }

                    if (registrationCount > 0)
                    {
                        // Show detailed message with options
                        string alertScript = $@"
                            if (confirm('This session has {registrationCount} active registration(s).\n\nChoose an option:\n• Click OK to cancel all registrations and delete the session\n• Click Cancel to keep the session and registrations')) {{
                                __doPostBack('{Page.ClientID}', 'forcedelete:{sessionId}');
                            }}
                        ";
                        ClientScript.RegisterStartupScript(this.GetType(), "ConfirmForceDelete", alertScript, true);
                        return;
                    }

                    // Safe to delete - no registrations
                    string deleteQuery = "DELETE FROM ExpertSessions WHERE Id = @SessionId";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowAlert("Session deleted successfully!", "success");
                            LoadSessions();
                            System.Diagnostics.Debug.WriteLine($"Session {sessionId} deleted successfully");
                        }
                        else
                        {
                            ShowAlert("Failed to delete session", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error deleting session: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error deleting session: {ex.Message}");
            }
        }

        private void CancelAllRegistrations(int sessionId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== CancelAllRegistrations called with ID: {sessionId} ===");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get count of registrations first for reporting
                    string countQuery = "SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = @SessionId AND BookingStatus = 'Confirmed'";
                    int registrationCount = 0;

                    using (SqlCommand countCmd = new SqlCommand(countQuery, conn))
                    {
                        countCmd.Parameters.AddWithValue("@SessionId", sessionId);
                        registrationCount = Convert.ToInt32(countCmd.ExecuteScalar());
                    }

                    if (registrationCount == 0)
                    {
                        ShowAlert("No registrations found for this session.", "error");
                        return;
                    }

                    // Delete all registrations for this session
                    string deleteRegsQuery = "DELETE FROM VideoCallBookings WHERE SessionId = @SessionId";
                    using (SqlCommand cmd = new SqlCommand(deleteRegsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        int deletedCount = cmd.ExecuteNonQuery();

                        if (deletedCount > 0)
                        {
                            // Reset current participants count to 0
                            string updateSessionQuery = "UPDATE ExpertSessions SET CurrentParticipants = 0 WHERE Id = @SessionId";
                            using (SqlCommand updateCmd = new SqlCommand(updateSessionQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                updateCmd.ExecuteNonQuery();
                            }

                            ShowAlert($"Successfully canceled {deletedCount} registration(s). The session is now available for new registrations.", "success");
                            LoadSessions();
                            System.Diagnostics.Debug.WriteLine($"Canceled {deletedCount} registrations for session {sessionId}");
                        }
                        else
                        {
                            ShowAlert("Failed to cancel registrations.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error canceling registrations: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error canceling registrations: {ex.Message}");
            }
        }

        private void ForceDeleteSession(int sessionId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== ForceDeleteSession called with ID: {sessionId} ===");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Get count for reporting
                            string countQuery = "SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = @SessionId AND BookingStatus = 'Confirmed'";
                            int registrationCount = 0;

                            using (SqlCommand countCmd = new SqlCommand(countQuery, conn, transaction))
                            {
                                countCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                registrationCount = Convert.ToInt32(countCmd.ExecuteScalar());
                            }

                            // Delete registrations first (child table)
                            string deleteRegsQuery = "DELETE FROM VideoCallBookings WHERE SessionId = @SessionId";
                            using (SqlCommand deleteRegsCmd = new SqlCommand(deleteRegsQuery, conn, transaction))
                            {
                                deleteRegsCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                deleteRegsCmd.ExecuteNonQuery();
                            }

                            // Then delete the session (parent table)
                            string deleteSessionQuery = "DELETE FROM ExpertSessions WHERE Id = @SessionId";
                            using (SqlCommand deleteSessionCmd = new SqlCommand(deleteSessionQuery, conn, transaction))
                            {
                                deleteSessionCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                int sessionDeleted = deleteSessionCmd.ExecuteNonQuery();

                                if (sessionDeleted > 0)
                                {
                                    transaction.Commit();
                                    ShowAlert($"Session deleted successfully! Also canceled {registrationCount} registration(s).", "success");
                                    LoadSessions();
                                    System.Diagnostics.Debug.WriteLine($"Force deleted session {sessionId} with {registrationCount} registrations");
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ShowAlert("Failed to delete session.", "error");
                                }
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error force deleting session: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error force deleting session: {ex.Message}");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                System.Diagnostics.Debug.WriteLine("=== btnSave_Click triggered ===");

                int staffId = GetCurrentStaffId();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;
                    bool isEdit = !string.IsNullOrEmpty(hdnEditingSessionId.Value);

                    if (isEdit)
                    {
                        query = @"
                           UPDATE ExpertSessions SET
                               SessionTitle = @SessionTitle,
                               SessionDescription = @SessionDescription,
                               SessionDate = @SessionDate,
                               StartTime = @StartTime,
                               EndTime = @EndTime,
                               SessionType = @SessionType,
                               MaxParticipants = @MaxParticipants,
                               PointsCost = @PointsCost,
                               SessionTopic = @SessionTopic,
                               ExpertName = @ExpertName,
                               ExpertTitle = @ExpertTitle,
                               Status = @Status
                           WHERE Id = @SessionId";
                    }
                    else
                    {
                        query = @"
                           INSERT INTO ExpertSessions (
                               SessionTitle, SessionDescription, SessionDate, StartTime, EndTime,
                               SessionType, MaxParticipants, CurrentParticipants, PointsCost,
                               Status, SessionTopic, ExpertName, ExpertTitle, CreatedBy, CreatedDate
                           ) VALUES (
                               @SessionTitle, @SessionDescription, @SessionDate, @StartTime, @EndTime,
                               @SessionType, @MaxParticipants, 0, @PointsCost,
                               @Status, @SessionTopic, @ExpertName, @ExpertTitle, @CreatedBy, @CreatedDate
                           )";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionTitle", txtTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@SessionDescription", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@SessionDate", Convert.ToDateTime(txtSessionDate.Text));
                        cmd.Parameters.AddWithValue("@StartTime", TimeSpan.Parse(txtStartTime.Text));
                        cmd.Parameters.AddWithValue("@EndTime", TimeSpan.Parse(txtEndTime.Text));
                        cmd.Parameters.AddWithValue("@SessionType", ddlSessionType.SelectedValue);
                        cmd.Parameters.AddWithValue("@MaxParticipants", Convert.ToInt32(txtMaxParticipants.Text));
                        cmd.Parameters.AddWithValue("@PointsCost", string.IsNullOrEmpty(txtPointsCost.Text) ? 0 : Convert.ToInt32(txtPointsCost.Text));
                        cmd.Parameters.AddWithValue("@SessionTopic", ddlTopic.SelectedValue);
                        cmd.Parameters.AddWithValue("@ExpertName", ddlExpertName.SelectedValue);
                        cmd.Parameters.AddWithValue("@ExpertTitle", txtExpertTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Status", "Available");

                        if (isEdit)
                        {
                            cmd.Parameters.AddWithValue("@SessionId", Convert.ToInt32(hdnEditingSessionId.Value));
                            System.Diagnostics.Debug.WriteLine($"Updating session {hdnEditingSessionId.Value}");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CreatedBy", staffId);
                            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                            System.Diagnostics.Debug.WriteLine("Creating new session");
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string successMessage = isEdit ? "Session updated successfully!" : "Session created successfully!";
                            ShowAlert(successMessage, "success");
                            ClearForm();
                            LoadSessions();
                            System.Diagnostics.Debug.WriteLine($"Session save successful: {successMessage}");
                        }
                        else
                        {
                            ShowAlert("Failed to save session. Please try again.", "error");
                            System.Diagnostics.Debug.WriteLine("Session save failed - no rows affected");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error saving session: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error saving session: {ex.Message}");
            }
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            // Handle the custom postback for force delete
            if (!string.IsNullOrEmpty(eventArgument) && eventArgument.StartsWith("forcedelete:"))
            {
                string sessionIdStr = eventArgument.Substring("forcedelete:".Length);
                if (int.TryParse(sessionIdStr, out int sessionId))
                {
                    System.Diagnostics.Debug.WriteLine($"Processing force delete via RaisePostBackEvent for session {sessionId}");
                    ForceDeleteSession(sessionId);
                    return;
                }
            }

            base.RaisePostBackEvent(sourceControl, eventArgument);
        }

        private void ClearForm()
        {
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtSessionDate.Text = "";
            txtStartTime.Text = "";
            txtEndTime.Text = "";
            ddlSessionType.SelectedIndex = 0;
            txtMaxParticipants.Text = "";
            txtPointsCost.Text = "0";
            ddlTopic.SelectedIndex = 0;
            ddlExpertName.SelectedIndex = 0;
            txtExpertTitle.Text = "";
            hdnEditingSessionId.Value = "";
            btnSave.Text = "Create Session";

            string script = "document.getElementById('createForm').style.display = 'none';";
            ClientScript.RegisterStartupScript(this.GetType(), "HideForm", script, true);
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = type == "success" ? "alert alert-success" : "alert alert-error";
            AlertMessage.Text = message;
            AlertPanel.Visible = true;
        }

        private int GetCurrentStaffId()
        {
            if (Session["StaffID"] != null)
            {
                return Convert.ToInt32(Session["StaffID"]);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id FROM Staff WHERE Username = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["StaffName"]);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            int staffId = Convert.ToInt32(result);
                            Session["StaffID"] = staffId;
                            return staffId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting staff ID: {ex.Message}");
            }

            return 1; // Default fallback
        }
    }
}