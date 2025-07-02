using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam
{
    public partial class Staff : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set user name from session or database
                // For now, using default value set in the markup
                // You can uncomment below to set from session:
                // if (Session["UserName"] != null)
                // {
                //     lblUserName.Text = Session["UserName"].ToString();
                // }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Redirect to login page
            Response.Redirect("~/Login.aspx");
        }
    }
}