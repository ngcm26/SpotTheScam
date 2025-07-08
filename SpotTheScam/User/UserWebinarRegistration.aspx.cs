using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace SpotTheScam.User
{
    public partial class UserWebinarRegistration : System.Web.UI.Page
    {
        private int currentUserPoints = 75; // This should come from your user session/database
        private int sessionPointsRequired = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSessionDetails();
                ValidatePointsRequirement();
            }
        }

        private void LoadSessionDetails()
        {
            try
            {
                // Get session ID from query string
                string sessionId = Request.QueryString["sessionId"];

                if (!string.IsNullOrEmpty(sessionId))
                {
                    // Load session details based on ID
                    LoadSessionById(sessionId);
                }
                else
                {
                    // Default session details (for demo)
                    SetDefaultSessionDetails();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading session details: " + ex.Message);
            }
        }

        private void LoadSessionById(string sessionId)
        {
            // This would typically load from database
            // For demo purposes, using hardcoded data based on sessionId
            switch (sessionId)
            {
                case "1":
                    ltSessionTitle.Text = "Protecting Your Online Banking";
                    ltSessionDescription.Text = "Learn essential security practices for online banking, how to spot fraudulent websites, and what to do if your account is compromised.";
                    ltSessionDate.Text = "June 15, 2025";
                    ltSessionTime.Text = "2:00 PM - 3:00 PM";
                    ltParticipants.Text = "Up to 100 Participants";
                    ltSessionType.Text = "Free Session";
                    ltExpertName.Text = "Dr Harvey Blue";
                    ltExpertTitle.Text = "Cybersecurity Specialist, 15+ years experience";
                    imgExpert.ImageUrl = "/Images/expert2.jpg";
                    btnRegister.Text = "Register Now - Free";
                    sessionPointsRequired = 0;
                    break;

                case "2":
                    ltSessionTitle.Text = "Small Group: Latest Phone Scam Tactics";
                    ltSessionDescription.Text = "Intimate session with max 10 participants. Deep dive into current phone scam methods with personalized Q&A time.";
                    ltSessionDate.Text = "June 17, 2025";
                    ltSessionTime.Text = "10:00 AM - 11:30 AM";
                    ltParticipants.Text = "Max 10 Participants";
                    ltSessionType.Text = "Premium Session - 50 Points";
                    ltExpertName.Text = "Officer James Wilson";
                    ltExpertTitle.Text = "Investigating phone and romance scams for 10+ years";
                    imgExpert.ImageUrl = "/Images/expert3.jpg";
                    btnRegister.Text = "Reserve Spot - 50 Points";
                    sessionPointsRequired = 50;
                    break;

                case "3":
                    ltSessionTitle.Text = "VIP One-on-One Safety Consultation";
                    ltSessionDescription.Text = "Private consultation to review your personal digital security, analyze any suspicious communications, and create a personalized safety plan.";
                    ltSessionDate.Text = "June 19, 2025";
                    ltSessionTime.Text = "3:00 PM - 4:00 PM";
                    ltParticipants.Text = "One-on-one session";
                    ltSessionType.Text = "VIP Premium - 100 Points";
                    ltExpertName.Text = "Maria Rodriguez";
                    ltExpertTitle.Text = "Digital Safety Educator, Senior Specialist";
                    imgExpert.ImageUrl = "/Images/expert1.jpg";
                    btnRegister.Text = "Reserve Spot - 100 Points";
                    sessionPointsRequired = 100;
                    break;

                default:
                    SetDefaultSessionDetails();
                    sessionPointsRequired = 0;
                    break;
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

                // Optionally redirect back to listings page after a delay
                // Response.Write("<script>setTimeout(function(){ window.location.href = 'UserWebinarSessionListing.aspx'; }, 3000);</script>");
            }
        }

        private void SetDefaultSessionDetails()
        {
            ltSessionTitle.Text = "Protecting Your Online Banking";
            ltSessionDescription.Text = "Learn essential security practices for online banking, how to spot fraudulent websites, and what to do if your account is compromised.";
            ltSessionDate.Text = "June 15, 2025";
            ltSessionTime.Text = "2:00 PM - 3:00 PM";
            ltParticipants.Text = "Up to 100 Participants";
            ltSessionType.Text = "Free Session";
            ltExpertName.Text = "Dr Harvey Blue";
            ltExpertTitle.Text = "Cybersecurity Specialist, 15+ years experience";
            imgExpert.ImageUrl = "/Images/expert2.jpg";
            btnRegister.Text = "Register Now - Free";
            sessionPointsRequired = 0;
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // Check points requirement first
                if (sessionPointsRequired > currentUserPoints)
                {
                    ShowErrorMessage($"Insufficient points. You need {sessionPointsRequired} points but only have {currentUserPoints}.");
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
                                DeductPoints(sessionPointsRequired);
                            }

                            // Redirect to success page with session details
                            string sessionId = Request.QueryString["sessionId"] ?? "1";
                            Response.Redirect($"WebinarRegistrationSuccess.aspx?sessionId={sessionId}&firstName={Server.UrlEncode(txtFirstName.Text)}&lastName={Server.UrlEncode(txtLastName.Text)}");
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
                // Log error details for debugging
                System.Diagnostics.Debug.WriteLine($"Registration Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void DeductPoints(int pointsToDeduct)
        {
            try
            {
                // TODO: Implement database update to deduct points from user's account
                // string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                // using (SqlConnection conn = new SqlConnection(connectionString))
                // {
                //     string query = "UPDATE Users SET Points = Points - @PointsToDeduct WHERE UserId = @UserId";
                //     using (SqlCommand cmd = new SqlCommand(query, conn))
                //     {
                //         cmd.Parameters.AddWithValue("@PointsToDeduct", pointsToDeduct);
                //         cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                //         conn.Open();
                //         cmd.ExecuteNonQuery();
                //     }
                // }

                // For demo purposes, just update the local variable
                currentUserPoints -= pointsToDeduct;

                // Update session if needed
                if (Session["UserPoints"] != null)
                {
                    Session["UserPoints"] = currentUserPoints;
                }

                System.Diagnostics.Debug.WriteLine($"Deducted {pointsToDeduct} points. User now has {currentUserPoints} points.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deducting points: {ex.Message}");
            }
        }

        private int GetCurrentUserPoints()
        {
            // TODO: Get from database or session
            // For demo purposes, returning hardcoded value
            if (Session["UserPoints"] != null)
            {
                return Convert.ToInt32(Session["UserPoints"]);
            }
            return 75; // Default points
        }

        private bool ValidateForm()
        {
            bool isValid = true;
            string errorMessage = "";

            // Reset control styles
            ResetControlStyles();

            // Validate First Name
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                txtFirstName.CssClass += " error";
                isValid = false;
            }

            // Validate Last Name
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                txtLastName.CssClass += " error";
                isValid = false;
            }

            // Validate Email
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

            // Validate Phone
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

            // Check for duplicate registration
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
            // Remove common phone number characters
            string cleanPhone = phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "");

            // Check if it contains only digits and is reasonable length
            return cleanPhone.All(char.IsDigit) && cleanPhone.Length >= 8 && cleanPhone.Length <= 15;
        }

        private bool IsAlreadyRegistered(string email)
        {
            // In a real application, this would check the database
            // For demo purposes, returning false
            try
            {
                string sessionId = Request.QueryString["sessionId"] ?? "1";

                // TODO: Check database for existing registration
                return false; // For demo
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking registration: {ex.Message}");
                return false;
            }
        }

        private bool ProcessRegistration()
        {
            try
            {
                string sessionId = Request.QueryString["sessionId"] ?? "1";

                // Create registration object
                var registration = new WebinarRegistration
                {
                    SessionId = sessionId,
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    SecurityConcerns = ddlSecurityConcerns.SelectedValue,
                    BankingMethods = GetSelectedBankingMethods(),
                    RegistrationDate = DateTime.Now,
                    UserId = GetCurrentUserId(),
                    PointsUsed = sessionPointsRequired
                };

                // Save to database
                return SaveRegistration(registration);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing registration: {ex.Message}");
                return false;
            }
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

        private int GetCurrentUserId()
        {
            // TODO: Implement based on your authentication system
            if (Session["UserId"] != null)
            {
                return Convert.ToInt32(Session["UserId"]);
            }
            return 0; // Guest user or implement proper user ID retrieval
        }

        private bool SaveRegistration(WebinarRegistration registration)
        {
            try
            {
                // TODO: Implement database save
                // For demo purposes, simulate successful save
                System.Threading.Thread.Sleep(500); // Simulate processing time
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving registration: {ex.Message}");
                return false;
            }
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

    // Registration data model
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