using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class Quizzes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Any initialization code can go here
                // For example, check if user is logged in, load user-specific data, etc.
            }
        }

        // Search is now handled by client-side JavaScript
        // No server-side search method needed for this simple implementation
    }
}