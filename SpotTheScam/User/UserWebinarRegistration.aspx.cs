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
    public partial class UserWebinarRegistration : System.Web.UI.Page
    {
        private string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        // Store points in ViewState instead of instance variable
        private int CurrentUserPoints
        {
            get
            {
                if (ViewState["CurrentUserPoints"] != null)
                    return Convert.ToInt32(ViewState["CurrentUserPoints"]);
                return 0;
            }
            set
            {
                ViewState["CurrentUserPoints"] = value;
                // Update the label immediately when points change
                if (lblCurrentPoints != null)
                {
                    lblCurrentPoints.Text = value.ToString();
                }
            }
        }

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

                // Check if sessionId is provided
                if (string.IsNullOrEmpty(Request.QueryString["sessionId"]))
                {
                    ShowErrorMessage("Please select a session from the listings page.");
                    AddGoBackButton();
                    return;
                }

                LoadUserCurrentPoints();
                LoadSessionDetails();
                ValidatePointsRequirement();
                UpdatePointsDisplay();
            }
        }

        private void LoadUserCurrentPoints()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== LoadUserCurrentPoints in UserWebinarRegistration ===");
                System.Diagnostics.Debug.WriteLine($"Session Username: {Session["Username"]}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from Username first
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
                            System.Diagnostics.Debug.WriteLine($"User found: {Session["Username"]}, UserID: {userId}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"User not found: {Session["Username"]}");
                            Response.Redirect("UserLogin.aspx");
                            return;
                        }
                    }

                    // Get current total points from PointsTransactions table (authoritative source)
                    string getPointsQuery = @"
                        SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                        FROM PointsTransactions 
                        WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(getPointsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        CurrentUserPoints = result != null ? Convert.ToInt32(result) : 0;
                        Session["CurrentPoints"] = CurrentUserPoints;

                        System.Diagnostics.Debug.WriteLine($"Current points loaded in UserWebinarRegistration: {CurrentUserPoints}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user points in UserWebinarRegistration: {ex.Message}");
                CurrentUserPoints = 0;
                Session["CurrentPoints"] = 0;
            }
        }

        private void LoadSessionDetails()
        {
            try
            {
                string sessionIdStr = Request.QueryString["sessionId"];
                System.Diagnostics.Debug.WriteLine($"🔍 Loading session details for sessionId: {sessionIdStr}");

                if (!int.TryParse(sessionIdStr, out int sessionId))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Invalid session ID format");
                    ShowErrorMessage("Invalid session ID format.");
                    AddGoBackButton();
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First check if the session exists at all
                    string checkQuery = "SELECT COUNT(*) FROM ExpertSessions WHERE Id = @SessionId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@SessionId", sessionId);
                        int sessionExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (sessionExists == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ Session {sessionId} does not exist in database");
                            ShowErrorMessage($"Session {sessionId} not found. Please select a session from the available listings.");
                            AddGoBackButton();
                            return;
                        }
                    }

                    string query = @"
                        SELECT es.Id, es.SessionTitle, es.SessionDescription, es.SessionDate, es.StartTime, es.EndTime,
                               es.ExpertName, es.ExpertTitle, es.SessionType, es.MaxParticipants, es.CurrentParticipants,
                               es.PointsCost, es.Status, es.CreatedDate
                        FROM ExpertSessions es
                        WHERE es.Id = @SessionId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Check if session is available
                                string status = reader["Status"].ToString();
                                if (status != "Available")
                                {
                                    System.Diagnostics.Debug.WriteLine($"❌ Session {sessionId} is not available (Status: {status})");
                                    ShowErrorMessage("This session is no longer available for registration.");
                                    AddGoBackButton();
                                    return;
                                }

                                // Load session details into the form
                                string expertName = reader["ExpertName"].ToString();

                                ltSessionTitle.Text = reader["SessionTitle"].ToString();
                                ltSessionDescription.Text = reader["SessionDescription"].ToString();
                                ltSessionDate.Text = Convert.ToDateTime(reader["SessionDate"]).ToString("MMMM dd, yyyy");
                                ltSessionTime.Text = reader["StartTime"].ToString() + " - " + reader["EndTime"].ToString();
                                ltExpertName.Text = expertName;
                                ltExpertTitle.Text = reader["ExpertTitle"].ToString();

                                // Set the correct expert image based on expert name
                                imgExpert.ImageUrl = GetExpertImageByName(expertName);

                                System.Diagnostics.Debug.WriteLine($"✅ Set expert image for {expertName}: {imgExpert.ImageUrl}");

                                int maxParticipants = Convert.ToInt32(reader["MaxParticipants"]);
                                int currentParticipants = Convert.ToInt32(reader["CurrentParticipants"]);
                                int availableSpots = maxParticipants - currentParticipants;

                                // Update participants display
                                ltParticipants.Text = $"Up to {maxParticipants} Participants";

                                int pointsCost = Convert.ToInt32(reader["PointsCost"]);

                                // Update session type display
                                if (pointsCost > 0)
                                {
                                    ltSessionType.Text = $"Premium Session - {pointsCost} Points Required";
                                }
                                else
                                {
                                    ltSessionType.Text = "Free Session";
                                }

                                // Store session info in ViewState for later use
                                ViewState["SessionId"] = sessionId;
                                ViewState["PointsRequired"] = pointsCost;
                                ViewState["AvailableSpots"] = availableSpots;

                                System.Diagnostics.Debug.WriteLine($"✅ Session details loaded successfully for session {sessionId}");
                                System.Diagnostics.Debug.WriteLine($"   - Points Required: {pointsCost}");
                                System.Diagnostics.Debug.WriteLine($"   - Available Spots: {availableSpots}");
                                System.Diagnostics.Debug.WriteLine($"   - User Has Points: {CurrentUserPoints}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"❌ Session {sessionId} not found in query results");
                                ShowErrorMessage("Session details could not be loaded.");
                                AddGoBackButton();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading session details: {ex.Message}");
                ShowErrorMessage("Error loading session details. Please try again.");
                AddGoBackButton();
            }
        }

        private void ValidatePointsRequirement()
        {
            try
            {
                if (ViewState["PointsRequired"] != null)
                {
                    int pointsRequired = Convert.ToInt32(ViewState["PointsRequired"]);
                    int availableSpots = Convert.ToInt32(ViewState["AvailableSpots"] ?? 0);

                    System.Diagnostics.Debug.WriteLine($"🔍 Validating: Points Required: {pointsRequired}, User Points: {CurrentUserPoints}, Available Spots: {availableSpots}");

                    // Check if session is full
                    if (availableSpots <= 0)
                    {
                        btnRegister.Enabled = false;
                        btnRegister.Text = "Session Full";
                        btnRegister.CssClass += " disabled";
                        ShowErrorMessage("This session is currently full. Please check back later or choose another session.");
                        return;
                    }

                    // Check if user has enough points
                    if (pointsRequired > 0 && CurrentUserPoints < pointsRequired)
                    {
                        btnRegister.Enabled = false;
                        btnRegister.Text = $"Need {pointsRequired - CurrentUserPoints} More Points";
                        btnRegister.CssClass += " disabled";
                        ShowErrorMessage($"You need {pointsRequired} points to register for this session. You currently have {CurrentUserPoints} points. Complete more quizzes to earn points!");
                    }
                    else
                    {
                        btnRegister.Enabled = true;
                        btnRegister.Text = pointsRequired > 0 ? $"Register for {pointsRequired} Points" : "Register for Free";
                        System.Diagnostics.Debug.WriteLine($"✅ User can register for this session");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating points requirement: {ex.Message}");
            }
        }

        private void UpdatePointsDisplay()
        {
            // Set the label text directly on server side first
            if (lblCurrentPoints != null)
            {
                lblCurrentPoints.Text = CurrentUserPoints.ToString();
            }

            // Update the points display dynamically on the client side as backup
            string script = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    console.log('Updating points display to: {CurrentUserPoints}');
                    
                    // Find and update the label control
                    var pointsLabel = document.getElementById('{lblCurrentPoints.ClientID}');
                    if (pointsLabel) {{
                        pointsLabel.textContent = '{CurrentUserPoints}';
                        console.log('Updated points label directly');
                    }}
                    
                    // Also update any other points displays as backup
                    var pointsElements = document.querySelectorAll('.current-points, .points-display');
                    for (var i = 0; i < pointsElements.length; i++) {{
                        pointsElements[i].textContent = 'Current Points: {CurrentUserPoints}';
                    }}
                }});
            ";
            ClientScript.RegisterStartupScript(this.GetType(), "UpdatePointsDisplay", script, true);
        }

        private void AddGoBackButton()
        {
            string script = @"
                document.addEventListener('DOMContentLoaded', function() {
                    var btn = document.createElement('button');
                    btn.innerHTML = '← Go Back to Session Listing';
                    btn.className = 'btn btn-primary mt-3';
                    btn.onclick = function() { window.location.href = 'UserWebinarSessionListing.aspx'; };
                    
                    var container = document.querySelector('.container') || document.body;
                    container.appendChild(btn);
                });";
            ClientScript.RegisterStartupScript(this.GetType(), "AddGoBackButton", script, true);
        }

        // Updated btnRegister_Click method in UserWebinarRegistration.aspx.cs
        // Replace your existing registration code with this more robust version:

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== REGISTRATION BUTTON CLICKED ===");

                if (ViewState["SessionId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ ViewState SessionId is null");
                    ShowErrorMessage("Session information not found. Please try again.");
                    return;
                }

                // Reload current points before validation
                LoadUserCurrentPoints();

                int sessionId = Convert.ToInt32(ViewState["SessionId"]);
                int pointsRequired = Convert.ToInt32(ViewState["PointsRequired"] ?? 0);
                int userId = Convert.ToInt32(Session["UserID"]);

                System.Diagnostics.Debug.WriteLine($"🔄 Processing registration:");
                System.Diagnostics.Debug.WriteLine($"   - SessionId: {sessionId}");
                System.Diagnostics.Debug.WriteLine($"   - UserId: {userId}");
                System.Diagnostics.Debug.WriteLine($"   - PointsRequired: {pointsRequired}");
                System.Diagnostics.Debug.WriteLine($"   - Current user points: {CurrentUserPoints}");

                // Validate form fields
                if (string.IsNullOrEmpty(txtFirstName.Text.Trim()) ||
                    string.IsNullOrEmpty(txtLastName.Text.Trim()) ||
                    string.IsNullOrEmpty(txtEmail.Text.Trim()) ||
                    string.IsNullOrEmpty(txtPhone.Text.Trim()))
                {
                    ShowErrorMessage("Please fill in all required fields.");
                    return;
                }

                // Final validation with reloaded points
                if (pointsRequired > 0 && CurrentUserPoints < pointsRequired)
                {
                    ShowErrorMessage($"Insufficient points. You need {pointsRequired} points but only have {CurrentUserPoints}.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("✅ Database connection opened");

                    // Check if user is already registered
                    string checkQuery = @"
                SELECT COUNT(*) 
                FROM VideoCallBookings 
                WHERE UserId = @UserId AND SessionId = @SessionId AND BookingStatus = 'Confirmed'";

                    using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);

                        int existingRegistration = Convert.ToInt32(cmd.ExecuteScalar());
                        if (existingRegistration > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("❌ User already registered");
                            ShowErrorMessage("You are already registered for this session.");
                            return;
                        }
                    }

                    // Check session availability
                    string availabilityQuery = @"
                SELECT (MaxParticipants - ISNULL(CurrentParticipants, 0)) as AvailableSpots 
                FROM ExpertSessions 
                WHERE Id = @SessionId AND Status = 'Available'";

                    using (SqlCommand cmd = new SqlCommand(availabilityQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        object result = cmd.ExecuteScalar();

                        if (result == null || Convert.ToInt32(result) <= 0)
                        {
                            System.Diagnostics.Debug.WriteLine("❌ Session no longer available");
                            ShowErrorMessage("This session is no longer available or is full.");
                            return;
                        }
                    }

                    // Get table structure to build dynamic query
                    string getColumnsQuery = @"
                SELECT COLUMN_NAME 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'VideoCallBookings' 
                AND COLUMN_NAME IN ('SessionId', 'UserId', 'CustomerName', 'CustomerPhone', 'CustomerEmail', 
                                   'ScamConcerns', 'BookingDate', 'BookingStatus', 'PointsUsed', 'BookingType', 
                                   'FirstName', 'LastName', 'SessionLink')";

                    List<string> availableColumns = new List<string>();
                    using (SqlCommand cmd = new SqlCommand(getColumnsQuery, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                availableColumns.Add(reader["COLUMN_NAME"].ToString());
                            }
                        }
                    }

                    // Build dynamic insert query based on available columns
                    List<string> columns = new List<string>();
                    List<string> parameters = new List<string>();

                    // Essential columns
                    if (availableColumns.Contains("SessionId")) { columns.Add("SessionId"); parameters.Add("@SessionId"); }
                    if (availableColumns.Contains("UserId")) { columns.Add("UserId"); parameters.Add("@UserId"); }
                    if (availableColumns.Contains("CustomerName")) { columns.Add("CustomerName"); parameters.Add("@CustomerName"); }
                    if (availableColumns.Contains("CustomerPhone")) { columns.Add("CustomerPhone"); parameters.Add("@CustomerPhone"); }
                    if (availableColumns.Contains("CustomerEmail")) { columns.Add("CustomerEmail"); parameters.Add("@CustomerEmail"); }
                    if (availableColumns.Contains("ScamConcerns")) { columns.Add("ScamConcerns"); parameters.Add("@ScamConcerns"); }
                    if (availableColumns.Contains("BookingDate")) { columns.Add("BookingDate"); parameters.Add("@BookingDate"); }
                    if (availableColumns.Contains("BookingStatus")) { columns.Add("BookingStatus"); parameters.Add("@BookingStatus"); }
                    if (availableColumns.Contains("PointsUsed")) { columns.Add("PointsUsed"); parameters.Add("@PointsUsed"); }
                    if (availableColumns.Contains("BookingType")) { columns.Add("BookingType"); parameters.Add("@BookingType"); }

                    // Optional columns
                    if (availableColumns.Contains("FirstName")) { columns.Add("FirstName"); parameters.Add("@FirstName"); }
                    if (availableColumns.Contains("LastName")) { columns.Add("LastName"); parameters.Add("@LastName"); }

                    string registerQuery = $@"
                INSERT INTO VideoCallBookings 
                ({string.Join(", ", columns)})
                VALUES 
                ({string.Join(", ", parameters)})";

                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(registerQuery, conn))
                        {
                            // Add parameters based on available columns
                            if (availableColumns.Contains("SessionId")) cmd.Parameters.AddWithValue("@SessionId", sessionId);
                            if (availableColumns.Contains("UserId")) cmd.Parameters.AddWithValue("@UserId", userId);
                            if (availableColumns.Contains("CustomerName")) cmd.Parameters.AddWithValue("@CustomerName", txtFirstName.Text.Trim() + " " + txtLastName.Text.Trim());
                            if (availableColumns.Contains("CustomerPhone")) cmd.Parameters.AddWithValue("@CustomerPhone", txtPhone.Text.Trim());
                            if (availableColumns.Contains("CustomerEmail")) cmd.Parameters.AddWithValue("@CustomerEmail", txtEmail.Text.Trim());
                            if (availableColumns.Contains("ScamConcerns")) cmd.Parameters.AddWithValue("@ScamConcerns", ddlSecurityConcerns.SelectedValue ?? "General");
                            if (availableColumns.Contains("BookingDate")) cmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);
                            if (availableColumns.Contains("BookingStatus")) cmd.Parameters.AddWithValue("@BookingStatus", "Confirmed");
                            if (availableColumns.Contains("PointsUsed")) cmd.Parameters.AddWithValue("@PointsUsed", pointsRequired);
                            if (availableColumns.Contains("BookingType")) cmd.Parameters.AddWithValue("@BookingType", "Expert Session");
                            if (availableColumns.Contains("FirstName")) cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                            if (availableColumns.Contains("LastName")) cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());

                            System.Diagnostics.Debug.WriteLine("🔄 Executing registration insert...");
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"✅ User registered successfully in VideoCallBookings");

                                // Generate and update session link if column exists
                                if (availableColumns.Contains("SessionLink"))
                                {
                                    string sessionLink = GenerateSessionLink(sessionId, userId);
                                    UpdateBookingWithSessionLink(sessionId, userId, sessionLink);
                                }

                                // Deduct points if required
                                if (pointsRequired > 0)
                                {
                                    DeductPoints(conn, userId, pointsRequired, $"Webinar Registration - Session {sessionId}");
                                    System.Diagnostics.Debug.WriteLine($"✅ Points deducted: {pointsRequired}");
                                }

                                // Update session participant count
                                UpdateSessionParticipantCount(conn, sessionId);
                                System.Diagnostics.Debug.WriteLine($"✅ Session participant count updated");

                                ShowSuccessMessage("Registration successful! You will receive webinar details shortly.");

                                // Refresh points display after successful registration
                                LoadUserCurrentPoints();
                                UpdatePointsDisplay();

                                // Redirect to success page
                                string redirectUrl = $"WebinarRegistrationSuccess.aspx?sessionId={sessionId}&firstName={txtFirstName.Text}&lastName={txtLastName.Text}";

                                string redirectScript = $@"
                            setTimeout(function() {{ 
                                window.location.href = '{redirectUrl}'; 
                            }}, 2000);";
                                ClientScript.RegisterStartupScript(this.GetType(), "SuccessRedirect", redirectScript, true);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("❌ Registration failed - no rows affected");
                                ShowErrorMessage("Registration failed. Please try again.");
                            }
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ SQL Error during registration: {sqlEx.Message}");
                        System.Diagnostics.Debug.WriteLine($"❌ SQL Error Number: {sqlEx.Number}");

                        // Handle specific SQL errors
                        if (sqlEx.Message.Contains("Cannot insert the value NULL"))
                        {
                            ShowErrorMessage("Please ensure all required fields are filled out completely.");
                        }
                        else if (sqlEx.Message.Contains("PRIMARY KEY constraint") || sqlEx.Message.Contains("UNIQUE constraint"))
                        {
                            ShowErrorMessage("You are already registered for this session.");
                        }
                        else
                        {
                            ShowErrorMessage("Registration error occurred. Please try again or contact support.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error during registration: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                ShowErrorMessage("An unexpected error occurred during registration. Please try again.");
            }
        }

        private void DeductPoints(SqlConnection conn, int userId, int pointsToDeduct, string description)
        {
            try
            {
                // Add negative points transaction (deduction)
                string deductQuery = @"
                    INSERT INTO PointsTransactions (UserId, TransactionType, Points, Description, TransactionDate)
                    VALUES (@UserId, @TransactionType, @Points, @Description, @TransactionDate)";

                using (SqlCommand cmd = new SqlCommand(deductQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@TransactionType", "Webinar");
                    cmd.Parameters.AddWithValue("@Points", -pointsToDeduct); // Negative value for deduction
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine($"✅ Deducted {pointsToDeduct} points from user {userId}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error deducting points: {ex.Message}");
                throw;
            }
        }

        private void UpdateSessionParticipantCount(SqlConnection conn, int sessionId)
        {
            try
            {
                string updateQuery = @"
                    UPDATE ExpertSessions 
                    SET CurrentParticipants = (
                        SELECT COUNT(*) 
                        FROM VideoCallBookings 
                        WHERE SessionId = @SessionId AND BookingStatus = 'Confirmed'
                    )
                    WHERE Id = @SessionId";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@SessionId", sessionId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error updating participant count: {ex.Message}");
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

        // Updated UpdateBookingWithSessionLink method in UserWebinarRegistration.aspx.cs

        private void UpdateBookingWithSessionLink(int sessionId, int userId, string sessionLink)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First check if the SessionLink column exists
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
                            System.Diagnostics.Debug.WriteLine("⚠️ SessionLink column does not exist in VideoCallBookings table");
                            System.Diagnostics.Debug.WriteLine("💡 Please run: ALTER TABLE VideoCallBookings ADD SessionLink NVARCHAR(500) NULL");
                            return; // Exit gracefully instead of throwing error
                        }
                    }

                    // Check if the booking record exists
                    string checkQuery = "SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = @SessionId AND UserId = @UserId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@SessionId", sessionId);
                        checkCmd.Parameters.AddWithValue("@UserId", userId);
                        int recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (recordExists > 0)
                        {
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
                                if (rowsUpdated > 0)
                                {
                                    System.Diagnostics.Debug.WriteLine($"✅ Session link updated successfully for user {userId}");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"⚠️ No rows updated for session link - user {userId}, session {sessionId}");
                                }
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ No booking record found for user {userId}, session {sessionId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error updating session link: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                // Don't throw the exception - the registration should still succeed even if session link update fails
            }
        }

        private void ShowErrorMessage(string message)
        {
            // Show error message using the Panel and Literal controls in your ASPX
            pnlAlert.Visible = true;
            pnlAlert.CssClass = "alert alert-error";
            ltAlertMessage.Text = message;
        }

        private void ShowSuccessMessage(string message)
        {
            // Show success message using the Panel and Literal controls in your ASPX
            pnlAlert.Visible = true;
            pnlAlert.CssClass = "alert alert-success";
            ltAlertMessage.Text = message;
        }

        // Helper method to get correct expert image based on name
        private string GetExpertImageByName(string expertName)
        {
            string name = expertName?.ToLower() ?? "";

            if (name.Contains("harvey") && name.Contains("blue"))
            {
                System.Diagnostics.Debug.WriteLine("🖼️ Using expert2.jpg for Dr Harvey Blue");
                return "~/Images/expert2.jpg";
            }
            else if (name.Contains("james") && name.Contains("wilson"))
            {
                System.Diagnostics.Debug.WriteLine("🖼️ Using expert3.jpg for Officer James Wilson");
                return "~/Images/expert3.jpg";
            }
            else if (name.Contains("maria") && name.Contains("rodriguez"))
            {
                System.Diagnostics.Debug.WriteLine("🖼️ Using expert1.jpg for Maria Rodriguez");
                return "~/Images/expert1.jpg";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"🖼️ Using default expert2.jpg for {expertName}");
                return "~/Images/expert2.jpg"; // Default
            }
        }
    }
}