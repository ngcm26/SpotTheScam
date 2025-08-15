using System;

namespace SpotTheScam.Staff
{
    public partial class Staff : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If staff is logged in, show nav options
            if (Session["StaffName"] != null)
            {
                phNavOptions.Visible = true;

                // Only show manage staff for admins
                var role = Session["StaffRole"] as string;
                if (!string.IsNullOrEmpty(role) && role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    var ph = FindControl("phManageStaff") as System.Web.UI.WebControls.PlaceHolder;
                    if (ph != null) ph.Visible = true;
                }
            }
            else
            {
                phNavOptions.Visible = false;

                // OPTIONAL: Force redirect if needed
                // If you want to prevent access to pages without login,
                // uncomment this:
                //
                // Response.Redirect("StaffLogin.aspx");
            }
        }
    }
}
