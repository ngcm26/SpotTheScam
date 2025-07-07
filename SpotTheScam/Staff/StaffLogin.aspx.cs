using System;
using System.Data.SqlClient;
using System.Configuration;

namespace SpotTheScam.Staff
{
    public partial class StaffLogin : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["StaffName"] != null)
            {
                Response.Redirect("StaffDashboard.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Role FROM Staff WHERE LTRIM(RTRIM(Username)) = @Username AND LTRIM(RTRIM(Password)) = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Found matching staff user
                    Session["StaffName"] = username;
                    Session["StaffRole"] = reader["Role"].ToString();

                    reader.Close();

                    // Optional: Update LastLogin timestamp
                    string updateQuery = "UPDATE Staff SET LastLogin = GETDATE() WHERE Username = @Username";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@Username", username);
                    updateCmd.ExecuteNonQuery();

                    Response.Redirect("StaffDashboard.aspx");
                }
                else
                {
                    lblMessage.Text = "Invalid username or password.";
                }
            }
        }
    }
}
