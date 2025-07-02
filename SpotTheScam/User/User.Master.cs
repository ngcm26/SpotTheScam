using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class UserMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Username"] != null)
                {
                    // User is logged in
                    phUser.Visible = true;
                    phGuest.Visible = false;

                    lblUsername.Text = Session["Username"].ToString();
                }
                else
                {
                    // User is a guest
                    phUser.Visible = false;
                    phGuest.Visible = true;
                }
            }
        }
    }
}
