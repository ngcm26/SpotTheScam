using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Web.UI;
using SpotTheScam.Utils;

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
                    ConfigurePasswordSectionByRole();
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

        private void ConfigurePasswordSectionByRole()
        {
            string role = (ddlRole.SelectedValue ?? string.Empty).ToLower();
            bool isStaff = role == "staff";
            if (phPasswordFields != null)
            {
                phPasswordFields.Visible = !isStaff;
            }
            if (btnSendReset != null)
            {
                btnSendReset.Visible = isStaff;
            }
            if (cvPassword != null)
            {
                cvPassword.Enabled = !isStaff;
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

        protected void btnSendReset_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["user_id"], out int targetUserId))
            {
                ShowError("Invalid user ID");
                return;
            }

            string email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                ShowError("User email is empty. Save a valid email first.");
                return;
            }

            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Reuse existing, unexpired token if present; otherwise create a new one
                    string token = null;
                    DateTime expiresAt = DateTime.UtcNow.AddHours(1);
                    using (var get = new SqlCommand("SELECT VerifyCode, VerifyCodeExpiresAt FROM Users WHERE Id=@id AND VerifyCode IS NOT NULL AND VerifyCodeExpiresAt > GETUTCDATE()", con))
                    {
                        get.Parameters.AddWithValue("@id", targetUserId);
                        using (var r = get.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                token = Convert.ToString(r["VerifyCode"]);
                                expiresAt = Convert.ToDateTime(r["VerifyCodeExpiresAt"]);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(token))
                    {
                        token = GenerateNumericToken(6);
                    }

                    using (var set = new SqlCommand("UPDATE Users SET VerifyCode=@c, VerifyCodeExpiresAt=@exp WHERE Id=@id", con))
                    {
                        set.Parameters.AddWithValue("@c", token);
                        set.Parameters.AddWithValue("@exp", expiresAt);
                        set.Parameters.AddWithValue("@id", targetUserId);
                        set.ExecuteNonQuery();
                    }

                    string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/User/ResetPassword.aspx");
                    string resetUrl = baseUrl + "?token=" + Server.UrlEncode(token) + "&email=" + Server.UrlEncode(email);

                    try
                    {
                        EmailService.Send(email, "Reset your Spot The Scam password",
                            $"<p>An administrator has initiated a password reset for your account.</p>" +
                            $"<p><a href='{resetUrl}'>Click here to reset your password</a>. This link will expire in 1 hour.</p>" +
                            "<p>If you did not request this, you can safely ignore this email.</p>");
                    }
                    catch
                    {
                        // Ignore email errors and still report generic success
                    }
                }

                ShowSuccess("If the email is valid, a password reset link has been sent to the staff member.");
            }
            catch (Exception ex)
            {
                ShowError("Failed to send reset email: " + ex.Message);
            }
        }

        private static string GenerateNumericToken(int length)
        {
            const string digits = "0123456789";
            char[] buffer = new char[length];
            byte[] randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            for (int i = 0; i < length; i++)
            {
                buffer[i] = digits[randomBytes[i] % digits.Length];
            }
            return new string(buffer);
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