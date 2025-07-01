using System;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace SpotTheScam
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username=@username AND Password=@password";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = (int)cmd.ExecuteScalar();
                    if (count == 1)
                    {
                        // ✅ Save username in Session
                        Session["Username"] = username;
                        Response.Redirect("Default.aspx");
                    }
                    else
                    {
                        lblMessage.Text = "Invalid credentials!";
                    }
                }
            }
        }
    }
}
