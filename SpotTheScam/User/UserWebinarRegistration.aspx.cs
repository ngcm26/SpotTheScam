using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace SpotTheScam.User
{
    public partial class UserWebinarRegistration : System.Web.UI.Page
    {
        private int currentUserPoints = 0;
        private int sessionPointsRequired = 0;
        private string connectionString;

        public UserWebinarRegistration()
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
            // For now, we'll skip authentication and use a default user
            // You can enable this later when you have login functionality
            /*
            if (!IsUserLoggedIn())
            {
                string returnUrl = Server.UrlEncode(Request.Url.ToString());
                Response.Redirect($"../Login.aspx?returnUrl={returnUrl}");
                return;
            }
            */

            if (!IsPostBack)
            {
                LoadUserPoints();
                LoadSessionDetails();
                ValidatePointsRequirement();
                UpdatePointsDisplay();
            }
        }

        private void UpdatePointsDisplay()
        {
            // Update the points display in the header
            var pointsBadge = Page.Master.FindControl("ContentPlaceHolder1").FindControl("pointsBadge");
            if (pointsBadge == null)
            {
                // If we can't find a control, we can inject the points via JavaScript
                string script = $@"
                    document.addEventListener('DOMContentLoaded', function() {{
                        var pointsSpan = document.querySelector('.points-badge span:last-child');
                        if (pointsSpan) {{
                            pointsSpan.textContent = '{currentUserPoints} ⭐';
                        }}
                    }});
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "UpdatePoints", script, true);
            }
        }

        private bool IsUserLoggedIn()
        {
            return Session["UserId"] != null &&
                   Session["UserId"].ToString() != "0" &&
                   !string.IsNullOrEmpty(Session["UserId"].ToString());
        }

        private void LoadUserPoints()
        {
            try
            {
                int userId = GetCurrentUserId();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Updated to use 'Id' column name to match your existing table
                    string query = "SELECT Points FROM Users WHERE Id = @UserId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        conn.Open();

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            currentUserPoints = Convert.ToInt32(result);
                            // Update session with current points
                            Session["UserPoints"] = currentUserPoints;
                        }
                        else
                        {
                            currentUserPoints = 0;
                            ShowErrorMessage("User account not found. Please contact support.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user points: {ex.Message}");
                currentUserPoints = 0;
                ShowErrorMessage("Error loading user information. Please try again.");
            }
        }

        private void LoadSessionDetails()
        {
            try
            {
                string sessionId = Request.QueryString["sessionId"];

                if (!string.IsNullOrEmpty(sessionId))
                {
                    LoadSessionFromDatabase(sessionId);
                }
                else
                {
                    ShowErrorMessage("Invalid session. Please select a session from the listings page.");
                    Response.Redirect("UserWebinarSessionListing.aspx");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading session details: " + ex.Message);
            }
        }

        private void LoadSessionFromDatabase(string sessionId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            s.SessionId,
                            s.Title,
                            s.Description,
                            s.SessionDate,
                            s.StartTime,
                            s.EndTime,
                            s.MaxParticipants,
                            s.PointsRequired,
                            s.SessionType,
                            e.ExpertName,
                            e.ExpertTitle,
                            e.ExpertImage,
                            e.ExpertId,
                            (SELECT COUNT(*) FROM WebinarRegistrations wr 
                             WHERE wr.SessionId = s.SessionId AND wr.IsActive = 1) as CurrentRegistrations
                        FROM WebinarSessions s
                        INNER JOIN Experts e ON s.ExpertId = e.ExpertId
                        WHERE s.SessionId = @SessionId AND s.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Check if session is full
                                int maxParticipants = Convert.ToInt32(reader["MaxParticipants"]);
                                int currentRegistrations = Convert.ToInt32(reader["CurrentRegistrations"]);

                                if (currentRegistrations >= maxParticipants)
                                {
                                    ShowErrorMessage("This session is currently full. Please select another session.");
                                    btnRegister.Enabled = false;
                                    btnRegister.Text = "Session Full";
                                    return;
                                }

                                // Check if session date has passed
                                DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                                if (sessionDate.Date < DateTime.Now.Date)
                                {
                                    ShowErrorMessage("This session has already occurred. Please select an upcoming session.");
                                    btnRegister.Enabled = false;
                                    btnRegister.Text = "Session Ended";
                                    return;
                                }

                                // Load session details from database
                                ltSessionTitle.Text = reader["Title"].ToString();
                                ltSessionDescription.Text = reader["Description"].ToString();
                                ltSessionDate.Text = sessionDate.ToString("MMMM dd, yyyy");

                                string startTime = reader["StartTime"].ToString();
                                string endTime = reader["EndTime"].ToString();
                                ltSessionTime.Text = $"{startTime} - {endTime}";

                                if (maxParticipants == 1)
                                {
                                    ltParticipants.Text = "One-on-one session";
                                }
                                else if (maxParticipants <= 10)
                                {
                                    ltParticipants.Text = $"Max {maxParticipants} Participants ({maxParticipants - currentRegistrations} spots left)";
                                }
                                else
                                {
                                    ltParticipants.Text = $"Up to {maxParticipants} Participants ({maxParticipants - currentRegistrations} spots left)";
                                }

                                sessionPointsRequired = Convert.ToInt32(reader["PointsRequired"]);
                                string sessionType = reader["SessionType"].ToString();

                                if (sessionPointsRequired == 0)
                                {
                                    ltSessionType.Text = "Free Session";
                                    btnRegister.Text = "Register Now - Free";
                                }
                                else
                                {
                                    ltSessionType.Text = $"{sessionType} - {sessionPointsRequired} Points";
                                    btnRegister.Text = $"Reserve Spot - {sessionPointsRequired} Points";
                                }

                                // Load expert details
                                ltExpertName.Text = reader["ExpertName"].ToString();
                                ltExpertTitle.Text = reader["ExpertTitle"].ToString();

                                string expertImagePath = reader["ExpertImage"].ToString();
                                if (!string.IsNullOrEmpty(expertImagePath))
                                {
                                    imgExpert.ImageUrl = expertImagePath;
                                }
                                else
                                {
                                    imgExpert.ImageUrl = "/Images/default-expert.jpg";
                                }
                            }
                            else
                            {
                                ShowErrorMessage("Session not found or no longer available.");
                                Response.Redirect("UserWebinarSessionListing.aspx");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading session from database: {ex.Message}");
                ShowErrorMessage("Error loading session details. Please try again.");
            }
        }

        private void ValidatePointsRequirement()
        {
            if (sessionPointsRequired > currentUserPoints)
            {
                // User doesn't have enough points
                btnRegister.Enabled = false;
                btnRegister.Text = $"Need More Points ({sessionPointsRequired - currentUserPoints} more needed)";
                btnRegister.CssClass += " register-btn-disabled";

                // Show error message
                ShowErrorMessage($"You need {sessionPointsRequired} points to register for this session. You currently have {currentUserPoints} points. You need {sessionPointsRequired - currentUserPoints} more points.");
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // DEBUG: Show what we're trying to process
                string sessionId = Request.QueryString["sessionId"];

                // For now, we'll skip authentication check
                // You can enable this later when you have login functionality
                /*
                if (!IsUserLoggedIn())
                {
                    Response.Redirect("../Login.aspx");
                    return;
                }
                */

                // Reload user points to ensure they haven't changed
                LoadUserPoints();

                // Check points requirement first
                if (sessionPointsRequired > currentUserPoints)
                {
                    ShowErrorMessage($"Insufficient points. You need {sessionPointsRequired} points but only have {currentUserPoints}.");
                    return;
                }

                // Check if session is still available
                if (!IsSessionAvailable())
                {
                    ShowErrorMessage("This session is no longer available or is full.");
                    return;
                }

                if (Page.IsValid)
                {
                    // Additional server-side validation
                    if (ValidateForm())
                    {
                        // Process registration
                        if (ProcessRegistration())
                        {
                            // Deduct points if it's a premium session
                            if (sessionPointsRequired > 0)
                            {
                                DeductUserPoints(sessionPointsRequired);
                            }

                            // SUCCESS: Registration worked!
                            ShowSuccessMessage($"SUCCESS! Registration completed for SessionId={sessionId}, UserId={GetCurrentUserId()}");
                            return;
                        }
                        else
                        {
                            ShowErrorMessage("Registration failed. Please try again or contact support.");
                        }
                    }
                }
                else
                {
                    ShowErrorMessage("Please correct the errors below and try again.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred during registration: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Registration Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private bool IsSessionAvailable()
        {
            try
            {
                string sessionId = Request.QueryString["sessionId"];

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            s.MaxParticipants,
                            COUNT(r.RegistrationId) as CurrentRegistrations,
                            s.SessionDate,
                            s.IsActive
                        FROM WebinarSessions s
                        LEFT JOIN WebinarRegistrations r ON s.SessionId = r.SessionId AND r.IsActive = 1
                        WHERE s.SessionId = @SessionId
                        GROUP BY s.SessionId, s.MaxParticipants, s.SessionDate, s.IsActive";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool isActive = Convert.ToBoolean(reader["IsActive"]);
                                DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                                int maxParticipants = Convert.ToInt32(reader["MaxParticipants"]);
                                int currentRegistrations = Convert.ToInt32(reader["CurrentRegistrations"]);

                                // Check if session is active, not in the past, and has space
                                return isActive &&
                                       sessionDate.Date >= DateTime.Now.Date &&
                                       currentRegistrations < maxParticipants;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking session availability: {ex.Message}");
                return false;
            }
        }

        private void DeductUserPoints(int pointsToDeduct)
        {
            try
            {
                int userId = GetCurrentUserId();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Updated to use 'Id' column name to match your existing table
                    string query = "UPDATE Users SET Points = Points - @PointsToDeduct WHERE Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PointsToDeduct", pointsToDeduct);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        conn.Open();

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Update local variable and session
                            currentUserPoints -= pointsToDeduct;
                            Session["UserPoints"] = currentUserPoints;

                            System.Diagnostics.Debug.WriteLine($"Successfully deducted {pointsToDeduct} points from user {userId}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to deduct points - no rows affected for user {userId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deducting points: {ex.Message}");
                throw; // Re-throw to handle in calling method
            }
        }

        private bool IsAlreadyRegistered(string email)
        {
            try
            {
                string sessionId = Request.QueryString["sessionId"];

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM WebinarRegistrations WHERE Email = @Email AND SessionId = @SessionId AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        conn.Open();

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking existing registration: {ex.Message}");
                return false; // Default to allowing registration on error
            }
        }

        private bool ProcessRegistration()
        {
            try
            {
                string sessionId = Request.QueryString["sessionId"];
                int userId = GetCurrentUserId();

                // Debug: Show what values we're trying to insert
                System.Diagnostics.Debug.WriteLine($"Attempting registration: SessionId={sessionId}, UserId={userId}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        INSERT INTO WebinarRegistrations 
                        (SessionId, UserId, FirstName, LastName, Email, Phone, SecurityConcerns, BankingMethods, PointsUsed, RegistrationDate, IsActive)
                        VALUES 
                        (@SessionId, @UserId, @FirstName, @LastName, @Email, @Phone, @SecurityConcerns, @BankingMethods, @PointsUsed, @RegistrationDate, 1)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@SecurityConcerns", ddlSecurityConcerns.SelectedValue ?? "");
                        cmd.Parameters.AddWithValue("@BankingMethods", GetSelectedBankingMethods());
                        cmd.Parameters.AddWithValue("@PointsUsed", sessionPointsRequired);
                        cmd.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine($"Insert result = {result}");

                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving registration: {ex.Message}");
                // Show the actual database error
                ShowErrorMessage($"Database error: {ex.Message} | SessionId: {Request.QueryString["sessionId"]} | UserId: {GetCurrentUserId()}");
                return false;
            }
        }

        private int GetCurrentUserId()
        {
            if (Session["UserId"] != null)
            {
                return Convert.ToInt32(Session["UserId"]);
            }

            // For now, return a default user ID for testing
            // You can change this later when you have proper authentication
            return 1; // Default test user ID
        }

        // Validation and helper methods
        private bool ValidateForm()
        {
            bool isValid = true;
            string errorMessage = "";

            ResetControlStyles();

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                txtFirstName.CssClass += " error";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                txtLastName.CssClass += " error";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtEmail.CssClass += " error";
                isValid = false;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                txtEmail.CssClass += " error";
                errorMessage += "Please enter a valid email address. ";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                txtPhone.CssClass += " error";
                isValid = false;
            }
            else if (!IsValidPhone(txtPhone.Text))
            {
                txtPhone.CssClass += " error";
                errorMessage += "Please enter a valid phone number. ";
                isValid = false;
            }

            if (isValid && IsAlreadyRegistered(txtEmail.Text))
            {
                txtEmail.CssClass += " error";
                errorMessage += "This email is already registered for this session. ";
                isValid = false;
            }

            if (!isValid && !string.IsNullOrEmpty(errorMessage))
            {
                ShowErrorMessage(errorMessage);
            }

            return isValid;
        }

        private void ResetControlStyles()
        {
            txtFirstName.CssClass = txtFirstName.CssClass.Replace(" error", "");
            txtLastName.CssClass = txtLastName.CssClass.Replace(" error", "");
            txtEmail.CssClass = txtEmail.CssClass.Replace(" error", "");
            txtPhone.CssClass = txtPhone.CssClass.Replace(" error", "");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            string cleanPhone = phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "");
            return cleanPhone.All(char.IsDigit) && cleanPhone.Length >= 8 && cleanPhone.Length <= 15;
        }

        private string GetSelectedBankingMethods()
        {
            var methods = new List<string>();

            if (chkMobileBanking.Checked) methods.Add("Mobile banking app");
            if (chkOnlineBanking.Checked) methods.Add("Online banking website");
            if (chkATM.Checked) methods.Add("ATM machines");
            if (chkPhoneBanking.Checked) methods.Add("Phone banking");
            if (chkBranchVisits.Checked) methods.Add("Branch visits");

            return string.Join(", ", methods);
        }

        private void ShowErrorMessage(string message)
        {
            pnlAlert.Visible = true;
            pnlAlert.CssClass = "alert alert-error";
            ltAlertMessage.Text = message;
        }

        private void ShowSuccessMessage(string message)
        {
            pnlAlert.Visible = true;
            pnlAlert.CssClass = "alert alert-success";
            ltAlertMessage.Text = message;
        }
    }

    public class WebinarRegistration
    {
        public string SessionId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SecurityConcerns { get; set; }
        public string BankingMethods { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int PointsUsed { get; set; }
    }
}