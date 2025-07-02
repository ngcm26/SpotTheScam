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
                // For now, using default value "Name Example"
                // You can set from session like this:
                // if (Session["UserName"] != null)
                // {
                //     lblUserName.Text = Session["UserName"].ToString();
                // }
                // else
                // {
                //     lblUserName.Text = "Name Example";
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