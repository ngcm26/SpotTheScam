using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class ScamRecoveryGuidance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user came from a specific scam type
                string scamType = Request.QueryString["type"];
                if (!string.IsNullOrEmpty(scamType))
                {
                    // You can customize the page content based on scam type
                    CustomizePageForScamType(scamType);
                }

                // Log page visit for analytics
                LogPageVisit();
            }
        }

        private void CustomizePageForScamType(string scamType)
        {
            // This method can be used to customize the guidance based on the specific scam type
            // For example, you might want to highlight certain steps or add specific warnings

            switch (scamType.ToLower())
            {
                case "banking":
                    // Add banking-specific guidance
                    Page.Title = "Banking Scam Recovery - Spot The Scam";
                    break;
                case "tech":
                    // Add tech support scam-specific guidance
                    Page.Title = "Tech Support Scam Recovery - Spot The Scam";
                    break;
                case "shopping":
                    // Add shopping scam-specific guidance
                    Page.Title = "Shopping Scam Recovery - Spot The Scam";
                    break;
                case "romance":
                    // Add romance scam-specific guidance
                    Page.Title = "Romance Scam Recovery - Spot The Scam";
                    break;
                case "phishing":
                    // Add phishing scam-specific guidance
                    Page.Title = "Phishing Scam Recovery - Spot The Scam";
                    break;
                case "employment":
                    // Add employment scam-specific guidance
                    Page.Title = "Employment Scam Recovery - Spot The Scam";
                    break;
                case "medical":
                    // Add medical scam-specific guidance
                    Page.Title = "Medical Scam Recovery - Spot The Scam";
                    break;
                case "lottery":
                    // Add lottery scam-specific guidance
                    Page.Title = "Lottery Scam Recovery - Spot The Scam";
                    break;
                default:
                    // Default title
                    Page.Title = "Scam Recovery Guidance - Spot The Scam";
                    break;
            }
        }

        private void LogPageVisit()
        {
            // Implementation for logging page visits
            // This could be useful for understanding which recovery guidance is most accessed
            try
            {
                // Log to database or analytics service
                string userAgent = Request.UserAgent;
                string ipAddress = GetClientIP();
                DateTime visitTime = DateTime.Now;
                string scamType = Request.QueryString["type"] ?? "general";

                // Example: Insert into database
                // LogVisit("ScamRecoveryGuidance", scamType, ipAddress, userAgent, visitTime);
            }
            catch (Exception ex)
            {
                // Handle logging errors gracefully
                // Don't let logging errors affect user experience
                System.Diagnostics.Debug.WriteLine("Error logging page visit: " + ex.Message);
            }
        }

        private string GetClientIP()
        {
            string ipAddress = "";
            try
            {
                ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
                {
                    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch
            {
                ipAddress = "Unknown";
            }
            return ipAddress;
        }

        // Method to handle external link clicks (for tracking purposes)
        protected void TrackExternalLinkClick(string linkType, string destination)
        {
            try
            {
                // Track when users click on external resources
                // This can help understand which resources are most helpful
                DateTime clickTime = DateTime.Now;
                string scamType = Request.QueryString["type"] ?? "general";

                // Example: LogExternalClick(linkType, destination, scamType, clickTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error tracking external link click: " + ex.Message);
            }
        }

        // Method to handle timeline view requests
        protected void ViewTimeline_Click(object sender, EventArgs e)
        {
            // Redirect to detailed timeline page or show modal
            Response.Redirect("~/User/RecoveryTimeline.aspx");
        }

        // Method to handle CSA guidelines link
        protected void CSAGuidelines_Click(object sender, EventArgs e)
        {
            // Track CSA guidelines access
            TrackExternalLinkClick("CSA_Guidelines", "https://www.csa.gov.sg/");

            // Redirect to CSA website or open in new tab
            // This would typically be handled by client-side JavaScript
        }

        // Method to handle prevention tips link
        protected void PreventionTips_Click(object sender, EventArgs e)
        {
            // Redirect to prevention tips page
            Response.Redirect("~/User/PreventionTips.aspx");
        }
    }
}