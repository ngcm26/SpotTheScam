using System;

namespace SpotTheScam
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Username"] != null)
                {
                    // User is logged in
                    phWelcome.Visible = true;
                    lblName.Text = Session["Username"].ToString();

                    phHero.Visible = false;
                }
                else
                {
                    // User is NOT logged in
                    phHero.Visible = true;
                    phWelcome.Visible = false;
                }
            }
        }
    }
}
