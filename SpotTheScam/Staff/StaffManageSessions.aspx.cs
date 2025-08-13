using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

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
                if (Session["StaffName"] == null && Session["StaffUsername"] == null && Session["Username"] == null)
                {
                    Response.Redirect("~/User/UserLogin.aspx");
                    return;
                }

                // Check for success/error messages from redirect
                string action = Request.QueryString["action"];
                if (!string.IsNullOrEmpty(action))
                {
                    switch (action.ToLower())
                    {
                        case "created":
                            ShowAlert("Session created successfully!", "success");
                            break;
                        case "updated":
                            ShowAlert("Session updated successfully!", "success");
                            break;
                        case "deleted":
                            ShowAlert("Session deleted successfully!", "success");
                            break;
                        case "cancelled":
                            ShowAlert("Registrations cancelled successfully!", "success");
                            break;
                    }
                }

                // Check for force delete parameter
                string forceDeleteParam = Request.QueryString["forceDelete"];
                if (!string.IsNullOrEmpty(forceDeleteParam) && int.TryParse(forceDeleteParam, out int sessionToDelete))
                {
                    ForceDeleteSession(sessionToDelete);
                    Response.Redirect("StaffManageSessions.aspx?action=deleted");
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

                    // Load all available sessions with proper registration count
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
                            (SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = es.Id) + 
                            ISNULL((SELECT COUNT(*) FROM WebinarRegistrations WHERE SessionId = es.Id AND IsActive = 1), 0) as RegistrationCount
                        FROM ExpertSessions es
                        WHERE es.Status = 'Available'
                        ORDER BY es.SessionDate ASC, es.StartTime ASC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        System.Diagnostics.Debug.WriteLine($"Available sessions loaded: {dt.Rows.Count}");

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

                    // First, verify the session exists
                    string verifyQuery = "SELECT COUNT(*) FROM ExpertSessions WHERE Id = @SessionId";
                    using (SqlCommand verifyCmd = new SqlCommand(verifyQuery, conn))
                    {
                        verifyCmd.Parameters.AddWithValue("@SessionId", sessionId);
                        int sessionExists = Convert.ToInt32(verifyCmd.ExecuteScalar());

                        if (sessionExists == 0)
                        {
                            ShowAlert("Session not found or already deleted.", "error");
                            LoadSessions(); // Refresh the list
                            return;
                        }
                    }

                    // Check for existing registrations in VideoCallBookings
                    string checkVideoBookingsQuery = "SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = @SessionId";
                    int videoBookingCount = 0;

                    using (SqlCommand checkVidCmd = new SqlCommand(checkVideoBookingsQuery, conn))
                    {
                        checkVidCmd.Parameters.AddWithValue("@SessionId", sessionId);
                        videoBookingCount = Convert.ToInt32(checkVidCmd.ExecuteScalar());
                        System.Diagnostics.Debug.WriteLine($"Found {videoBookingCount} video call bookings for session {sessionId}");
                    }

                    // Check for existing registrations in WebinarRegistrations
                    int webinarRegistrationCount = 0;
                    try
                    {
                        string checkWebinarQuery = "SELECT COUNT(*) FROM WebinarRegistrations WHERE SessionId = @SessionId AND IsActive = 1";
                        using (SqlCommand checkWebCmd = new SqlCommand(checkWebinarQuery, conn))
                        {
                            checkWebCmd.Parameters.AddWithValue("@SessionId", sessionId);
                            webinarRegistrationCount = Convert.ToInt32(checkWebCmd.ExecuteScalar());
                            System.Diagnostics.Debug.WriteLine($"Found {webinarRegistrationCount} webinar registrations for session {sessionId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // WebinarRegistrations table might not exist - that's okay
                        System.Diagnostics.Debug.WriteLine($"WebinarRegistrations table check failed (table might not exist): {ex.Message}");
                    }

                    int totalRegistrations = videoBookingCount + webinarRegistrationCount;

                    if (totalRegistrations > 0)
                    {
                        // Show detailed confirmation message with options
                        string alertScript = $@"
                            if (confirm('⚠️ This session has {totalRegistrations} active registration(s).\n\n' +
                                      'Video Call Bookings: {videoBookingCount}\n' +
                                      'Webinar Registrations: {webinarRegistrationCount}\n\n' +
                                      'If you delete this session:\n' +
                                      '• All participant registrations will be cancelled\n' +
                                      '• Participants will be notified\n' +
                                      '• Points will be refunded if applicable\n\n' +
                                      'Are you sure you want to proceed?')) {{
                                window.location.href = 'StaffManageSessions.aspx?forceDelete={sessionId}';
                            }}
                        ";
                        ClientScript.RegisterStartupScript(this.GetType(), "ConfirmCascadeDelete", alertScript, true);
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
                            System.Diagnostics.Debug.WriteLine($"Session {sessionId} deleted successfully");
                            // Use redirect to prevent refresh duplication
                            Response.Redirect("StaffManageSessions.aspx?action=deleted", false);
                            Context.ApplicationInstance.CompleteRequest();
                        }
                        else
                        {
                            ShowAlert("Failed to delete session. It may have already been deleted.", "error");
                            LoadSessions(); // Refresh to show current state
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
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            int totalCancelledRegistrations = 0;
                            int totalPointsRefunded = 0;

                            // Cancel VideoCallBookings
                            string getVideoBookingsQuery = @"
                                SELECT UserId, ISNULL(PointsUsed, 0) as PointsUsed, CustomerName
                                FROM VideoCallBookings 
                                WHERE SessionId = @SessionId";

                            var videoBookings = new List<(int UserId, int PointsUsed, string CustomerName)>();

                            using (SqlCommand getCmd = new SqlCommand(getVideoBookingsQuery, conn, transaction))
                            {
                                getCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                using (SqlDataReader reader = getCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        videoBookings.Add((
                                            Convert.ToInt32(reader["UserId"]),
                                            Convert.ToInt32(reader["PointsUsed"]),
                                            reader["CustomerName"].ToString()
                                        ));
                                    }
                                }
                            }

                            // Delete VideoCallBookings and refund points
                            if (videoBookings.Count > 0)
                            {
                                string deleteVideoBookingsQuery = "DELETE FROM VideoCallBookings WHERE SessionId = @SessionId";
                                using (SqlCommand deleteVideoCmd = new SqlCommand(deleteVideoBookingsQuery, conn, transaction))
                                {
                                    deleteVideoCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                    int deletedVideoBookings = deleteVideoCmd.ExecuteNonQuery();
                                    totalCancelledRegistrations += deletedVideoBookings;
                                    System.Diagnostics.Debug.WriteLine($"Deleted {deletedVideoBookings} video call bookings");
                                }

                                // Refund points for video bookings
                                foreach (var booking in videoBookings)
                                {
                                    if (booking.PointsUsed > 0)
                                    {
                                        string refundQuery = @"
                                            INSERT INTO PointsTransactions (UserId, TransactionType, Points, Description, TransactionDate)
                                            VALUES (@UserId, 'Refund', @Points, @Description, @TransactionDate)";

                                        using (SqlCommand refundCmd = new SqlCommand(refundQuery, conn, transaction))
                                        {
                                            refundCmd.Parameters.AddWithValue("@UserId", booking.UserId);
                                            refundCmd.Parameters.AddWithValue("@Points", booking.PointsUsed);
                                            refundCmd.Parameters.AddWithValue("@Description", $"Session {sessionId} cancelled - Refund for {booking.CustomerName}");
                                            refundCmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                                            refundCmd.ExecuteNonQuery();

                                            totalPointsRefunded += booking.PointsUsed;
                                            System.Diagnostics.Debug.WriteLine($"Refunded {booking.PointsUsed} points to user {booking.UserId}");
                                        }
                                    }
                                }
                            }

                            // Cancel WebinarRegistrations (if table exists)
                            try
                            {
                                string getWebinarQuery = @"
                                    SELECT UserId, ISNULL(PointsUsed, 0) as PointsUsed, FirstName, LastName
                                    FROM WebinarRegistrations 
                                    WHERE SessionId = @SessionId AND IsActive = 1";

                                var webinarRegistrations = new List<(int UserId, int PointsUsed, string Name)>();

                                using (SqlCommand getWebCmd = new SqlCommand(getWebinarQuery, conn, transaction))
                                {
                                    getWebCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                    using (SqlDataReader reader = getWebCmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string fullName = $"{reader["FirstName"]} {reader["LastName"]}".Trim();
                                            webinarRegistrations.Add((
                                                Convert.ToInt32(reader["UserId"]),
                                                Convert.ToInt32(reader["PointsUsed"]),
                                                fullName
                                            ));
                                        }
                                    }
                                }

                                // Deactivate webinar registrations instead of deleting
                                if (webinarRegistrations.Count > 0)
                                {
                                    string deactivateWebinarQuery = "UPDATE WebinarRegistrations SET IsActive = 0 WHERE SessionId = @SessionId AND IsActive = 1";
                                    using (SqlCommand deactivateWebCmd = new SqlCommand(deactivateWebinarQuery, conn, transaction))
                                    {
                                        deactivateWebCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                        int deactivatedWebinarRegs = deactivateWebCmd.ExecuteNonQuery();
                                        totalCancelledRegistrations += deactivatedWebinarRegs;
                                        System.Diagnostics.Debug.WriteLine($"Deactivated {deactivatedWebinarRegs} webinar registrations");
                                    }

                                    // Refund points for webinar registrations
                                    foreach (var registration in webinarRegistrations)
                                    {
                                        if (registration.PointsUsed > 0)
                                        {
                                            string refundQuery = @"
                                                INSERT INTO PointsTransactions (UserId, TransactionType, Points, Description, TransactionDate)
                                                VALUES (@UserId, 'Refund', @Points, @Description, @TransactionDate)";

                                            using (SqlCommand refundCmd = new SqlCommand(refundQuery, conn, transaction))
                                            {
                                                refundCmd.Parameters.AddWithValue("@UserId", registration.UserId);
                                                refundCmd.Parameters.AddWithValue("@Points", registration.PointsUsed);
                                                refundCmd.Parameters.AddWithValue("@Description", $"Session {sessionId} cancelled - Refund for {registration.Name}");
                                                refundCmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                                                refundCmd.ExecuteNonQuery();

                                                totalPointsRefunded += registration.PointsUsed;
                                                System.Diagnostics.Debug.WriteLine($"Refunded {registration.PointsUsed} points to user {registration.UserId}");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception webEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"WebinarRegistrations handling failed (table might not exist): {webEx.Message}");
                                // Continue - this is not critical if the table doesn't exist
                            }

                            // Reset current participants count to 0
                            string updateSessionQuery = "UPDATE ExpertSessions SET CurrentParticipants = 0 WHERE Id = @SessionId";
                            using (SqlCommand updateCmd = new SqlCommand(updateSessionQuery, conn, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                updateCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            string successMessage = $"Successfully canceled {totalCancelledRegistrations} registration(s).";
                            if (totalPointsRefunded > 0)
                            {
                                successMessage += $" Refunded {totalPointsRefunded} total points.";
                            }
                            successMessage += " The session is now available for new registrations.";

                            System.Diagnostics.Debug.WriteLine($"Canceled {totalCancelledRegistrations} registrations for session {sessionId}");

                            // Use redirect to prevent refresh duplication
                            Response.Redirect("StaffManageSessions.aspx?action=cancelled", false);
                            Context.ApplicationInstance.CompleteRequest();
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
                            int totalCancelledRegistrations = 0;
                            int totalPointsRefunded = 0;

                            // Step 1: Handle VideoCallBookings registrations
                            string getVideoBookingsQuery = @"
                                SELECT UserId, ISNULL(PointsUsed, 0) as PointsUsed, CustomerName
                                FROM VideoCallBookings 
                                WHERE SessionId = @SessionId";

                            var videoBookings = new List<(int UserId, int PointsUsed, string CustomerName)>();

                            using (SqlCommand getCmd = new SqlCommand(getVideoBookingsQuery, conn, transaction))
                            {
                                getCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                using (SqlDataReader reader = getCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        videoBookings.Add((
                                            Convert.ToInt32(reader["UserId"]),
                                            Convert.ToInt32(reader["PointsUsed"]),
                                            reader["CustomerName"].ToString()
                                        ));
                                    }
                                }
                            }

                            // Delete VideoCallBookings and refund points
                            if (videoBookings.Count > 0)
                            {
                                string deleteVideoBookingsQuery = "DELETE FROM VideoCallBookings WHERE SessionId = @SessionId";
                                using (SqlCommand deleteVideoCmd = new SqlCommand(deleteVideoBookingsQuery, conn, transaction))
                                {
                                    deleteVideoCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                    int deletedVideoBookings = deleteVideoCmd.ExecuteNonQuery();
                                    totalCancelledRegistrations += deletedVideoBookings;
                                    System.Diagnostics.Debug.WriteLine($"Deleted {deletedVideoBookings} video call bookings");
                                }

                                // Refund points for video bookings
                                foreach (var booking in videoBookings)
                                {
                                    if (booking.PointsUsed > 0)
                                    {
                                        string refundQuery = @"
                                            INSERT INTO PointsTransactions (UserId, TransactionType, Points, Description, TransactionDate)
                                            VALUES (@UserId, 'Refund', @Points, @Description, @TransactionDate)";

                                        using (SqlCommand refundCmd = new SqlCommand(refundQuery, conn, transaction))
                                        {
                                            refundCmd.Parameters.AddWithValue("@UserId", booking.UserId);
                                            refundCmd.Parameters.AddWithValue("@Points", booking.PointsUsed);
                                            refundCmd.Parameters.AddWithValue("@Description", $"Session {sessionId} cancelled - Refund for {booking.CustomerName}");
                                            refundCmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                                            refundCmd.ExecuteNonQuery();

                                            totalPointsRefunded += booking.PointsUsed;
                                            System.Diagnostics.Debug.WriteLine($"Refunded {booking.PointsUsed} points to user {booking.UserId}");
                                        }
                                    }
                                }
                            }

                            // Step 2: Handle WebinarRegistrations (if table exists)
                            try
                            {
                                string getWebinarQuery = @"
                                    SELECT UserId, ISNULL(PointsUsed, 0) as PointsUsed, FirstName, LastName
                                    FROM WebinarRegistrations 
                                    WHERE SessionId = @SessionId AND IsActive = 1";

                                var webinarRegistrations = new List<(int UserId, int PointsUsed, string Name)>();

                                using (SqlCommand getWebCmd = new SqlCommand(getWebinarQuery, conn, transaction))
                                {
                                    getWebCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                    using (SqlDataReader reader = getWebCmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string fullName = $"{reader["FirstName"]} {reader["LastName"]}".Trim();
                                            webinarRegistrations.Add((
                                                Convert.ToInt32(reader["UserId"]),
                                                Convert.ToInt32(reader["PointsUsed"]),
                                                fullName
                                            ));
                                        }
                                    }
                                }

                                // Delete webinar registrations and refund points
                                if (webinarRegistrations.Count > 0)
                                {
                                    string deleteWebinarQuery = "DELETE FROM WebinarRegistrations WHERE SessionId = @SessionId";
                                    using (SqlCommand deleteWebCmd = new SqlCommand(deleteWebinarQuery, conn, transaction))
                                    {
                                        deleteWebCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                        int deletedWebinarRegs = deleteWebCmd.ExecuteNonQuery();
                                        totalCancelledRegistrations += deletedWebinarRegs;
                                        System.Diagnostics.Debug.WriteLine($"Deleted {deletedWebinarRegs} webinar registrations");
                                    }

                                    // Refund points for webinar registrations
                                    foreach (var registration in webinarRegistrations)
                                    {
                                        if (registration.PointsUsed > 0)
                                        {
                                            string refundQuery = @"
                                                INSERT INTO PointsTransactions (UserId, TransactionType, Points, Description, TransactionDate)
                                                VALUES (@UserId, 'Refund', @Points, @Description, @TransactionDate)";

                                            using (SqlCommand refundCmd = new SqlCommand(refundQuery, conn, transaction))
                                            {
                                                refundCmd.Parameters.AddWithValue("@UserId", registration.UserId);
                                                refundCmd.Parameters.AddWithValue("@Points", registration.PointsUsed);
                                                refundCmd.Parameters.AddWithValue("@Description", $"Session {sessionId} cancelled - Refund for {registration.Name}");
                                                refundCmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                                                refundCmd.ExecuteNonQuery();

                                                totalPointsRefunded += registration.PointsUsed;
                                                System.Diagnostics.Debug.WriteLine($"Refunded {registration.PointsUsed} points to user {registration.UserId}");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception webEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"WebinarRegistrations handling failed (table might not exist): {webEx.Message}");
                                // Continue - this is not critical if the table doesn't exist
                            }

                            // Step 3: Finally delete the session
                            string deleteSessionQuery = "DELETE FROM ExpertSessions WHERE Id = @SessionId";
                            using (SqlCommand deleteSessionCmd = new SqlCommand(deleteSessionQuery, conn, transaction))
                            {
                                deleteSessionCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                int sessionDeleted = deleteSessionCmd.ExecuteNonQuery();

                                if (sessionDeleted > 0)
                                {
                                    transaction.Commit();

                                    System.Diagnostics.Debug.WriteLine($"✅ Force deleted session {sessionId} with {totalCancelledRegistrations} registrations and {totalPointsRefunded} points refunded");

                                    // Don't redirect here as this is called from Page_Load redirect
                                    // The redirect will happen in Page_Load after this method completes
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ShowAlert("Failed to delete session. It may have already been deleted.", "error");
                                    LoadSessions(); // Refresh to show current state
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
                            string successMessage = isEdit ? "updated" : "created";
                            System.Diagnostics.Debug.WriteLine($"Session save successful: {successMessage}");

                            // Use POST-Redirect-GET pattern to prevent duplicate submissions
                            Response.Redirect($"StaffManageSessions.aspx?action={successMessage}", false);
                            Context.ApplicationInstance.CompleteRequest();
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
            if (Session["StaffID"] == null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string username = Session["StaffName"]?.ToString() ??
                                        Session["StaffUsername"]?.ToString() ??
                                        Session["Username"]?.ToString();

                        if (!string.IsNullOrEmpty(username))
                        {
                            string query = "SELECT Id FROM Staff WHERE Username = @Username OR Email = @Username";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Username", username);
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
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting staff ID: {ex.Message}");
                }
            }
            else
            {
                return Convert.ToInt32(Session["StaffID"]);
            }

            return 1; // Default fallback
        }
    }
}