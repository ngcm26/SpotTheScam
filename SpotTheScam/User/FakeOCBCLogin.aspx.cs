using System;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace SpotTheScam.User
{
    public partial class FakeOCBCLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No specific logic needed on page load for this simulation
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Simulate a successful login
                ShowAlert("Login successful! You have securely linked your simulated OCBC account.", "success");
                // In a real scenario, you would redirect to the UserBankAccounts page
                // and potentially pass data to add a new account.
                // For this simulation, we just show a success message.
            }
            else
            {
                ShowAlert("Please enter both access code and PIN.", "danger");
            }
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = $"alert alert-{type}";
            AlertMessage.Text = message;
            AlertPanel.Visible = true;

            // Hide alert after 5 seconds
            string script = @"
                setTimeout(function() {
                    var alertPanel = document.getElementById('" + AlertPanel.ClientID + @"');
                    if (alertPanel) {
                        alertPanel.style.display = 'none';
                    }
                }, 5000);
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideAlert", script, true);
        }
    }
}
