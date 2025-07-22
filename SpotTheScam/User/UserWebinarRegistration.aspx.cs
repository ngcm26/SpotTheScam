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
                    ShowGoBackButton();
                    return;
                }

                LoadUserCurrentPoints();
                LoadSessionDetails();
                ValidatePointsRequirement();
                UpdatePointsDisplay();
            }
            else
            {
                // On postback, reload points to ensure we have current data
                LoadUserCurrentPoints();
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
                    ShowGoBackButton();
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
                            ShowGoBackButton();
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
                                    ShowGoBackButton();
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
                                ShowGoBackButton();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading session details: {ex.Message}");
                ShowErrorMessage("Error loading session details. Please try again.");
                ShowGoBackButton();
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
            // Update the points display dynamically on the client side
            string script = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    // Find and update points badge
                    var pointsBadge = document.querySelector('.points-badge');
                    if (pointsBadge) {{
                        pointsBadge.innerHTML = '<span>Current Points: {CurrentUserPoints} ⭐</span>';
                    }}
                    
                    // Update any other points displays
                    var pointsElements = document.querySelectorAll('.current-points, .points-display');
                    for (var i = 0; i < pointsElements.length; i++) {{
                        pointsElements[i].textContent = 'Current Points: {CurrentUserPoints}';
                    }}
                    
                    console.log('Points display updated to: {CurrentUserPoints}');
                }});
            ";
            ClientScript.RegisterStartupScript(this.GetType(), "UpdatePointsDisplay", script, true);
        }

        private void ShowGoBackButton()
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
            ClientScript.RegisterStartupScript(this.GetType(), "ShowGoBackButton", script, true);
        }

        private void CreateExpertSessionRegistrationsTable(SqlConnection conn)
        {
            try
            {
                string createTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ExpertSessionRegistrations')
                    CREATE TABLE ExpertSessionRegistrations (
                        Id int IDENTITY(1,1) PRIMARY KEY,
                        UserId int NOT NULL,
                        SessionId int NOT NULL,
                        RegistrationDate datetime NOT NULL,
                        Status nvarchar(50) NOT NULL DEFAULT 'Confirmed',
                        PointsSpent int NOT NULL DEFAULT 0,
                        FirstName nvarchar(100) NULL,
                        LastName nvarchar(100) NULL,
                        Email nvarchar(255) NULL,
                        Phone nvarchar(50) NULL,
                        CreatedDate datetime DEFAULT GETDATE(),
                        FOREIGN KEY (UserId) REFERENCES Users(Id),
                        FOREIGN KEY (SessionId) REFERENCES ExpertSessions(Id)
                    )";

                using (SqlCommand cmd = new SqlCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("✅ ExpertSessionRegistrations table created successfully");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error creating ExpertSessionRegistrations table: {ex.Message}");
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["SessionId"] == null)
                {
                    ShowErrorMessage("Session information not found. Please try again.");
                    return;
                }

                // CRITICAL FIX: Reload current points before validation
                LoadUserCurrentPoints();

                int sessionId = Convert.ToInt32(ViewState["SessionId"]);
                int pointsRequired = Convert.ToInt32(ViewState["PointsRequired"] ?? 0);
                int userId = Convert.ToInt32(Session["UserID"]);

                System.Diagnostics.Debug.WriteLine($"🔄 Processing registration: SessionId={sessionId}, UserId={userId}, PointsRequired={pointsRequired}");
                System.Diagnostics.Debug.WriteLine($"🔄 Current user points at registration: {CurrentUserPoints}");

                // Final validation with reloaded points
                if (pointsRequired > 0 && CurrentUserPoints < pointsRequired)
                {
                    ShowErrorMessage($"Insufficient points. You need {pointsRequired} points but only have {CurrentUserPoints}.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // CRITICAL: Create the registration table if it doesn't exist
                    CreateExpertSessionRegistrationsTable(conn);

                    // Check if user is already registered
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM ExpertSessionRegistrations 
                        WHERE UserId = @UserId AND SessionId = @SessionId AND Status = 'Confirmed'";

                    using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);

                        int existingRegistration = Convert.ToInt32(cmd.ExecuteScalar());
                        if (existingRegistration > 0)
                        {
                            ShowErrorMessage("You are already registered for this session.");
                            return;
                        }
                    }

                    // Check if session is still available
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
                            ShowErrorMessage("This session is no longer available or is full.");
                            return;
                        }
                    }

                    // Register user for session
                    string registerQuery = @"
                        INSERT INTO ExpertSessionRegistrations 
                        (UserId, SessionId, RegistrationDate, Status, PointsSpent, FirstName, LastName, Email, Phone, CreatedDate)
                        VALUES 
                        (@UserId, @SessionId, @RegistrationDate, @Status, @PointsSpent, @FirstName, @LastName, @Email, @Phone, @CreatedDate)";

                    using (SqlCommand cmd = new SqlCommand(registerQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Status", "Confirmed");
                        cmd.Parameters.AddWithValue("@PointsSpent", pointsRequired);

                        // Get user info from form fields (if available)
                        cmd.Parameters.AddWithValue("@FirstName", txtFirstName?.Text ?? "");
                        cmd.Parameters.AddWithValue("@LastName", txtLastName?.Text ?? "");
                        cmd.Parameters.AddWithValue("@Email", txtEmail?.Text ?? "");
                        cmd.Parameters.AddWithValue("@Phone", txtPhone?.Text ?? "");
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"✅ User registered successfully");

                            // Deduct points if required
                            if (pointsRequired > 0)
                            {
                                DeductPoints(conn, userId, pointsRequired, $"Webinar Registration - Session {sessionId}");
                                System.Diagnostics.Debug.WriteLine($"✅ Points deducted: {pointsRequired}");
                            }

                            // Update session participant count
                            UpdateSessionParticipantCount(conn, sessionId);
                            System.Diagnostics.Debug.WriteLine($"✅ Session participant count updated");

                            ShowSuccessMessage("Registration successful! You will receive a confirmation email shortly.");

                            // Refresh points display after successful registration
                            LoadUserCurrentPoints();
                            UpdatePointsDisplay();

                            // Redirect to success page with session info
                            string redirectUrl = $"WebinarRegistrationSuccess.aspx?sessionId={sessionId}&firstName={txtFirstName?.Text}&lastName={txtLastName?.Text}";

                            string redirectScript = $@"
                                setTimeout(function() {{ 
                                    window.location.href = '{redirectUrl}'; 
                                }}, 2000);";
                            ClientScript.RegisterStartupScript(this.GetType(), "SuccessRedirect", redirectScript, true);
                        }
                        else
                        {
                            ShowErrorMessage("Registration failed. Please try again.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error during registration: {ex.Message}");
                ShowErrorMessage("An error occurred during registration. Please try again.");
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
                        FROM ExpertSessionRegistrations 
                        WHERE SessionId = @SessionId AND Status = 'Confirmed'
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

        private int GetCurrentUserId()
        {
            if (Session["UserID"] != null)
            {
                return Convert.ToInt32(Session["UserID"]);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            int userId = Convert.ToInt32(result);
                            Session["UserID"] = userId;
                            return userId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting user ID: {ex.Message}");
            }
            return 0;
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