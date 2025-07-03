using System;

namespace SpotTheScam.Staff
{
    public partial class StaffDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["StaffName"] == null)
            {
                // Not logged in → show access denied placeholder
                phDashboard.Visible = false;
                phPleaseLogin.Visible = true;
            }
            else
            {
                // Logged in → show dashboard
                lblStaffName.Text = Session["StaffName"].ToString();
                phDashboard.Visible = true;
                phPleaseLogin.Visible = false;
            }
        }
    }
}
