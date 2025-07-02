using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Any initialization code can go here
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Redirect to login page
            Response.Redirect("~/Login.aspx");
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            // Redirect to sign up page
            Response.Redirect("~/SignUp.aspx");
        }
    }
}