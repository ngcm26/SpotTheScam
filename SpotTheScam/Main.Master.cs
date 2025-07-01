using System;

namespace SpotTheScam
{
    public partial class Main : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] != null)
            {
                phUser.Visible = true;
                phGuest.Visible = false;
                lblUsername.Text = Session["Username"].ToString();
            }
            else
            {
                phUser.Visible = false;
                phGuest.Visible = true;
            }
        }
    }
}
