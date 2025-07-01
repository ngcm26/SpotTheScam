using System;

namespace SpotTheScam
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] != null)
            {
                phWelcome.Visible = true;
                phGuest.Visible = false;

                lblName.Text = Session["Username"].ToString();
            }
            else
            {
                phWelcome.Visible = false;
                phGuest.Visible = true;
            }
        }
    }
}
