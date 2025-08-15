using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User.Controls
{
    public partial class FamilySideNav : System.Web.UI.UserControl
    {
        // Set this from each page, e.g., "groups" | "create" | "invites" | "connect" | "add" | "logs"
        public string ActiveKey { get; set; } = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) DataBind(); // needed for <%# %> bindings
        }

        protected string ActiveClass(string key)
        {
            return string.Equals(key, ActiveKey, StringComparison.OrdinalIgnoreCase)
                ? "nav-link active" : "nav-link";
        }
    }
}