using System;

namespace SpotTheScam.Staff
{
    public partial class StaffLogout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear session data
            Session.Clear();
            Session.Abandon();

            // Redirect to login page (or you can use Dashboard to show the "Please log in" message)
            Response.Redirect("~/User/UserLogin.aspx");
        }
    }
}
