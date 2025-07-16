using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace SpotTheScam.Staff
{
    public partial class EditUsers : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
        private int userId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["StaffName"] == null)
            {
                Response.Redirect("StaffDashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["user_id"] != null && int.TryParse(Request.QueryString["user_id"], out userId))
                {
                    LoadUserInformation();
                }
                else
                {
                    ShowError("No user ID provided");
                }
            }
        }

        private void LoadUserInformation()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Username, Email, PhoneNumber, Role FROM Users WHERE Id = @userId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtUsername.Text = reader["Username"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                txtPhoneNumber.Text = reader["PhoneNumber"].ToString();
                                
                                string role = reader["Role"].ToString();
                                if (!string.IsNullOrEmpty(role) && ddlRole.Items.FindByValue(role) != null)
                                    ddlRole.SelectedValue = role;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading user information: " + ex.Message);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["user_id"], out userId))
            {
                ShowError("Invalid user ID");
                return;
            }

            if (Page.IsValid)
            {
                try
                {
                    string username = txtUsername.Text.Trim();
                    string email = txtEmail.Text.Trim();
                    string phoneNumber = txtPhoneNumber.Text.Trim();
                    string role = ddlRole.SelectedValue;
                    string password = txtPassword.Text.Trim();

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        
                        string query;
                        if (!string.IsNullOrEmpty(password))
                        {
                            // Update with new password
                            query = "UPDATE Users SET Username = @Username, Email = @Email, PhoneNumber = @PhoneNumber, Role = @Role, Password = @Password WHERE Id = @Id";
                        }
                        else
                        {
                            // Update without changing password
                            query = "UPDATE Users SET Username = @Username, Email = @Email, PhoneNumber = @PhoneNumber, Role = @Role WHERE Id = @Id";
                        }

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                            cmd.Parameters.AddWithValue("@Role", role);
                            cmd.Parameters.AddWithValue("@Id", userId);
                            
                            if (!string.IsNullOrEmpty(password))
                            {
                                cmd.Parameters.AddWithValue("@Password", password);
                            }
                            
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Clear password fields
                    txtPassword.Text = "";
                    txtConfirmPassword.Text = "";
                    
                    // Show alert and redirect
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "updateSuccess", 
                        "alert('User information updated successfully!'); window.location='ManageUsers.aspx';", true);
                }
                catch (Exception ex)
                {
                    ShowError("Error updating user information: " + ex.Message);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageUsers.aspx");
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            lblSuccess.Visible = false;
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
        }
    }
}