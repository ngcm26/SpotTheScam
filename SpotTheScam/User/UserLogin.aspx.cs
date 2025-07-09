using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.Configuration;

namespace SpotTheScam
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                try
                {
                    con.Open();
                    // Pull Username, Role, AND Id so you can check which page to redirect to and set UserId.
                    string query = "SELECT Id, Username, Role FROM Users WHERE Email = @Email AND Password = @Password"; // MODIFIED LINE
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            int userId = Convert.ToInt32(reader["Id"]); // ADDED LINE
                            string username = reader["Username"].ToString();
                            string role = reader["Role"].ToString().ToLower(); // lowercase for safety

                            Session["UserId"] = userId; // ADDED LINE
                            Session["Username"] = username;

                            if (role == "admin")
                            {
                                // Admin → redirect to staff dashboard
                                Session["StaffName"] = username;
                                Response.Redirect("~/Staff/StaffDashboard.aspx");
                            }
                            else
                            {
                                // Normal user → user home
                                Response.Redirect("UserHome.aspx");
                            }
                        }
                        else
                        {
                            lblMessage.ForeColor = Color.Red;
                            lblMessage.Text = "Invalid email or password. Please try again.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "An error occurred. Please try again later.";
                }
            }
        }
    }
}