using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class WebinarRegistrationSuccess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSessionDetails();
                SetBackToHomeLink();
            }
        }

        private void LoadSessionDetails()
        {
            try
            {
                string sessionId = Request.QueryString["sessionId"];
                string firstName = Request.QueryString["firstName"];
                string lastName = Request.QueryString["lastName"];

                // Load session details based on ID
                if (!string.IsNullOrEmpty(sessionId))
                {
                    LoadSessionById(sessionId);
                }
                else
                {
                    SetDefaultSessionDetails();
                }

                // Personalize if user info available
                if (!string.IsNullOrEmpty(firstName))
                {
                    // Could personalize the success message here if needed
                }
            }
            catch (Exception ex)
            {
                // Handle error gracefully
                System.Diagnostics.Debug.WriteLine($"Error loading session details: {ex.Message}");
                SetDefaultSessionDetails();
            }
        }

        private void LoadSessionById(string sessionId)
        {
            switch (sessionId)
            {
                case "1":
                    ltSessionName.Text = "Protecting Your Online Banking";
                    ltDateTime.Text = "June 15, 2025 at 2:00 PM - 3:00 PM";
                    ltExpertName.Text = "Dr Harvey Blue";
                    break;

                case "2":
                    ltSessionName.Text = "Small Group: Latest Phone Scam Tactics";
                    ltDateTime.Text = "June 17, 2025 at 10:00 AM - 11:30 AM";
                    ltExpertName.Text = "Officer James Wilson";
                    break;

                case "3":
                    ltSessionName.Text = "VIP One-on-One Safety Consultation";
                    ltDateTime.Text = "June 19, 2025 at 3:00 PM - 4:00 PM";
                    ltExpertName.Text = "Maria Rodriguez";
                    break;

                default:
                    SetDefaultSessionDetails();
                    break;
            }
        }

        private void SetDefaultSessionDetails()
        {
            ltSessionName.Text = "Protecting Your Online Banking";
            ltDateTime.Text = "June 15, 2025 at 2:00 PM - 3:00 PM";
            ltExpertName.Text = "Dr Harvey Blue";
        }

        private void SetBackToHomeLink()
        {
            // Set the navigation URL for the back to home button
            lnkBackToHome.NavigateUrl = "~/User/UserHome.aspx";
        }
    }
}