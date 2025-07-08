using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class UserWebinarSessionListing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize sessions display
                ShowAllSessions();
            }
        }

        protected void FilterSessions_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string filter = btn.CommandArgument;

            // Update button styles
            btnAllSessions.CssClass = "filter-btn " + (filter == "All" ? "active" : "inactive");
            btnFreeOnly.CssClass = "filter-btn " + (filter == "Free" ? "active" : "inactive");
            btnPremium.CssClass = "filter-btn " + (filter == "Premium" ? "active" : "inactive");

            // Reset topic filter when session filter changes
            ddlTopics.SelectedValue = "All";

            // Apply filter
            ApplyFilters(filter, "All");
        }

        protected void ddlTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            string topicFilter = ddlTopics.SelectedValue;
            string sessionFilter = "All";

            // Determine current session filter
            if (btnFreeOnly.CssClass.Contains("active"))
                sessionFilter = "Free";
            else if (btnPremium.CssClass.Contains("active"))
                sessionFilter = "Premium";

            ApplyFilters(sessionFilter, topicFilter);
        }

        private void ApplyFilters(string sessionFilter, string topicFilter)
        {
            // Session 1: Banking + Free
            bool showSession1 = (sessionFilter == "All" || sessionFilter == "Free") &&
                               (topicFilter == "All" || topicFilter == "Banking");
            session1.Visible = showSession1;

            // Session 2: Phone + Premium
            bool showSession2 = (sessionFilter == "All" || sessionFilter == "Premium") &&
                               (topicFilter == "All" || topicFilter == "Phone");
            session2.Visible = showSession2;

            // Session 3: Social + Premium
            bool showSession3 = (sessionFilter == "All" || sessionFilter == "Premium") &&
                               (topicFilter == "All" || topicFilter == "Social");
            session3.Visible = showSession3;
        }

        private void ShowAllSessions()
        {
            session1.Visible = true;
            session2.Visible = true;
            session3.Visible = true;
        }
    }
}