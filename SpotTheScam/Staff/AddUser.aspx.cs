using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace SpotTheScam.Staff
{
    public partial class AddUser : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["StaffName"] == null)
            {
                Response.Redirect("StaffDashboard.aspx");
                return;
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string username = txtUsername.Text.Trim();
                string email = txtEmail.Text.Trim();
                string phoneNumber = txtPhoneNumber.Text.Trim();
                string role = ddlRole.SelectedValue;
                string password = txtPassword.Text.Trim();

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        // Check for duplicate email AND phone number (must match both)
                        string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND PhoneNumber = @PhoneNumber";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Email", email);
                            checkCmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                            int count = (int)checkCmd.ExecuteScalar();
                            if (count > 0)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "userExists", "alert('A user with this email and/or phone number already exists.');", true);
                                lblMessage.Visible = false;
                                lblSuccess.Visible = false;
                                return;
                            }
                        }
                        // Insert new user
                        string query = "INSERT INTO Users (Username, Email, PhoneNumber, Role, Password) VALUES (@Username, @Email, @PhoneNumber, @Role, @Password)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                            cmd.Parameters.AddWithValue("@Role", role);
                            cmd.Parameters.AddWithValue("@Password", password);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // Redirect to ManageUsers so the new user is visible
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "addSuccess", "alert('User added successfully!'); window.location='ManageUsers.aspx';", true);
                }
                catch (Exception ex)
                {
                    if (ex is SqlException sqlEx && sqlEx.Message.Contains("UNIQUE KEY constraint"))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "userExists", "alert('A user with this email and/or phone number already exists.');", true);
                        lblMessage.Visible = false;
                        lblSuccess.Visible = false;
                    }
                    else
                    {
                        lblMessage.Text = "Error adding user: " + ex.Message;
                        lblMessage.Visible = true;
                        lblSuccess.Visible = false;
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageUsers.aspx");
        }
    }
}