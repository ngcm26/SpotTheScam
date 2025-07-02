using System;

namespace SpotTheScam.Staff
{
    public partial class Staff : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["StaffName"] != null)
            {
                phNavOptions.Visible = true; // Show nav options when logged in
            }
            else
            {
                phNavOptions.Visible = false; // Hide nav options if not logged in
            }
        }
    }
}
