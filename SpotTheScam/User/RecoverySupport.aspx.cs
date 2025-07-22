using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class RecoverySupport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Add any initialization logic here
                // For example, you might want to check if user is logged in
                // and personalize the experience

                // You can also track page visits for analytics
                LogPageVisit();
            }
        }

        private void LogPageVisit()
        {
            // Implementation for logging page visits
            // This could be useful for understanding which recovery resources are most accessed
            try
            {
                // Log to database or analytics service
                string userAgent = Request.UserAgent;
                string ipAddress = GetClientIP();
                DateTime visitTime = DateTime.Now;

                // Example: Insert into database
                // LogVisit("RecoverySupport", ipAddress, userAgent, visitTime);
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

        // Method to handle scam type selection (if implementing server-side handling)
        // This method is not currently being used since we're using JavaScript for navigation
        // But it's kept here as a reference for server-side implementation if needed
        protected void HandleScamTypeSelection(string scamType)
        {
            // This method could be called via AJAX or postback
            // to provide personalized guidance based on scam type

            switch (scamType.ToLower())
            {
                case "banking":
                    // Redirect to ScamRecoveryGuidance with banking type
                    Response.Redirect("~/User/ScamRecoveryGuidance.aspx?type=banking");
                    break;
                case "tech":
                    // Redirect to ScamRecoveryGuidance with tech type
                    Response.Redirect("~/User/ScamRecoveryGuidance.aspx?type=tech");
                    break;
                case "shopping":
                    // Redirect to ScamRecoveryGuidance with shopping type
                    Response.Redirect("~/User/ScamRecoveryGuidance.aspx?type=shopping");
                    break;
                case "romance":
                    // Redirect to ScamRecoveryGuidance with romance type
                    Response.Redirect("~/User/ScamRecoveryGuidance.aspx?type=romance");
                    break;
                case "phishing":
                    // Redirect to ScamRecoveryGuidance with phishing type
                    Response.Redirect("~/User/ScamRecoveryGuidance.aspx?type=phishing");
                    break;
                case "employment":
                    // Redirect to ScamRecoveryGuidance with employment type
                    Response.Redirect("~/User/ScamRecoveryGuidance.aspx?type=employment");
                    break;
                case "medical":
                    // Redirect to ScamRecoveryGuidance with medical type
                    Response.Redirect("~/User/ScamRecoveryGuidance.aspx?type=medical");
                    break;
                case "lottery":
                    // Redirect to ScamRecoveryGuidance with lottery type
                    Response.Redirect("~/User/ScamRecoveryGuidance.aspx?type=lottery");
                    break;
                default:
                    // Redirect to general guidance
                    Response.Redirect("~/User/ScamRecoveryGuidance.aspx?type=other");
                    break;
            }
        }

        // Method to initiate emergency contact
        protected void InitiateEmergencyContact(string contactType)
        {
            // Log emergency contact initiation for tracking and improvement
            try
            {
                // Log the emergency contact type and timestamp
                DateTime contactTime = DateTime.Now;

                // This could be useful for understanding which emergency services
                // are most needed and improving response times

                // Example: LogEmergencyContact(contactType, contactTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error logging emergency contact: " + ex.Message);
            }
        }

        // Method to handle chat widget opening
        protected void OpenChatSupport()
        {
            // Implementation for opening chat support
            // This could integrate with a live chat service or redirect to a chat page

            // For now, this is a placeholder that could be extended with:
            // - Integration with live chat services (like Zendesk, Intercom)
            // - Connection to automated chatbot
            // - Escalation to human support during business hours

            Response.Redirect("~/User/ChatSupport.aspx");
        }

        // Method to track scam type selections for analytics
        protected void TrackScamTypeSelection(string scamType)
        {
            try
            {
                // Track which scam types users are selecting most often
                // This can help understand trends and improve content
                DateTime selectionTime = DateTime.Now;
                string ipAddress = GetClientIP();

                // Example: LogScamTypeSelection(scamType, ipAddress, selectionTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error tracking scam type selection: " + ex.Message);
            }
        }
    }
}