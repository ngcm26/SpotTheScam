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
                        // Check for duplicate email
                        using (SqlCommand emailCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", conn))
                        {
                            emailCmd.Parameters.AddWithValue("@Email", email);
                            int emailCount = (int)emailCmd.ExecuteScalar();
                            if (emailCount > 0)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "dupEmail", "alert('This email is already in use. Please use a different email.');", true);
                                lblMessage.Visible = false;
                                lblSuccess.Visible = false;
                                return;
                            }
                        }

                        // Check for duplicate phone (if provided)
                        if (!string.IsNullOrWhiteSpace(phoneNumber))
                        {
                            using (SqlCommand phoneCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE PhoneNumber = @PhoneNumber", conn))
                            {
                                phoneCmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                                int phoneCount = (int)phoneCmd.ExecuteScalar();
                                if (phoneCount > 0)
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "dupPhone", "alert('This phone number is already in use. Please use a different phone number.');", true);
                                    lblMessage.Visible = false;
                                    lblSuccess.Visible = false;
                                    return;
                                }
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