using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace SpotTheScam.User
{
    public partial class UserProfile : System.Web.UI.Page
    {
        private string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("UserLogin.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadUserInfo();
            }
        }

        private void LoadUserInfo()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Username, Email, PhoneNumber FROM Users WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtUsername.Text = reader["Username"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtPhoneNumber.Text = reader["PhoneNumber"].ToString();
                        }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("UserLogin.aspx");
                return;
            }
            int userId = Convert.ToInt32(Session["UserId"]);
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            string oldPassword = txtOldPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Check for duplicate email/phone for other users
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE (Email = @Email OR PhoneNumber = @PhoneNumber) AND Id <> @Id";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        checkCmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        checkCmd.Parameters.AddWithValue("@Id", userId);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "profileFail", "alert('A user with this email or phone number already exists.');", true);
                            return;
                        }
                    }
                    // Password change logic
                    bool changingPassword = !string.IsNullOrWhiteSpace(oldPassword) || !string.IsNullOrWhiteSpace(newPassword) || !string.IsNullOrWhiteSpace(confirmPassword);
                    if (changingPassword)
                    {
                        if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "profileFail", "alert('To change your password, please fill in all password fields.');", true);
                            return;
                        }
                        // Check old password
                        string getPassQuery = "SELECT Password FROM Users WHERE Id = @Id";
                        using (SqlCommand passCmd = new SqlCommand(getPassQuery, conn))
                        {
                            passCmd.Parameters.AddWithValue("@Id", userId);
                            string currentPassword = passCmd.ExecuteScalar()?.ToString();
                            if (currentPassword != oldPassword)
                            {
                                lblOldPasswordError.Text = "Old password is incorrect.";
                                lblOldPasswordError.Visible = true;
                                lblMessage.Visible = false;
                                lblSuccess.Visible = false;
                                return;
                            }
                            else
                            {
                                lblOldPasswordError.Visible = false;
                            }
                            if (newPassword != confirmPassword)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "profileFail", "alert('New passwords do not match.');", true);
                                return;
                            }
                            if (newPassword == oldPassword)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "profileFail", "alert('New password cannot be the same as the old password.');", true);
                                return;
                            }
                        }
                        // Update user info and password
                        string updateQuery = "UPDATE Users SET Username = @Username, Email = @Email, PhoneNumber = @PhoneNumber, Password = @Password WHERE Id = @Id";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                            cmd.Parameters.AddWithValue("@Password", newPassword);
                            cmd.Parameters.AddWithValue("@Id", userId);
                            cmd.ExecuteNonQuery();
                        }
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "profileSuccess", "alert('Profile and password updated successfully!');", true);
                        Session["Username"] = username;
                        return;
                    }
                    // Update user info only
                    string updateInfoQuery = "UPDATE Users SET Username = @Username, Email = @Email, PhoneNumber = @PhoneNumber WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(updateInfoQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Id", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "profileSuccess", "alert('Profile updated successfully!');", true);
                Session["Username"] = username;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "profileFail", $"alert('Error updating profile: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserHome.aspx");
        }
    }
}