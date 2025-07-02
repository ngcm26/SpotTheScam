using System;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace SpotTheScam
{
    public partial class Register : System.Web.UI.Page
    {
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Check if username exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username=@username";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@username", username);
                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        lblMessage.Text = "Username already exists!";
                        return;
                    }
                }

                // Insert new user
                string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.ExecuteNonQuery();
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.Text = "Account created successfully! <a href='UserLogin.aspx'>Login here</a>";
                }
            }
        }
    }
}
