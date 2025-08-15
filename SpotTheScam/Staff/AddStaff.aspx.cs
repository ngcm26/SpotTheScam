using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SpotTheScam.Staff
{
    public partial class AddStaff : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["StaffName"] == null || !IsAdmin())
            {
                Response.Redirect("StaffDashboard.aspx");
                return;
            }
        }

        private bool IsAdmin()
        {
            var role = Session["StaffRole"] as string;
            return !string.IsNullOrEmpty(role) && role.Equals("admin", StringComparison.OrdinalIgnoreCase);
        }

        protected void btnAddStaff_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhoneNumber.Text.Trim();
            string password = txtPassword.Text.Trim();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Duplicate email check
                    using (var emailCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email=@e", conn))
                    {
                        emailCmd.Parameters.AddWithValue("@e", email);
                        int emailCount = (int)emailCmd.ExecuteScalar();
                        if (emailCount > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "dupEmail", "alert('This email is already in use. Please use a different email.');", true);
                            return;
                        }
                    }

                    // Duplicate phone check (if provided)
                    if (!string.IsNullOrWhiteSpace(phone))
                    {
                        using (var phoneCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE PhoneNumber=@p", conn))
                        {
                            phoneCmd.Parameters.AddWithValue("@p", phone);
                            int phoneCount = (int)phoneCmd.ExecuteScalar();
                            if (phoneCount > 0)
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "dupPhone", "alert('This phone number is already in use. Please use a different phone number.');", true);
                                return;
                            }
                        }
                    }

                    using (var cmd = new SqlCommand("INSERT INTO Users (Username, Email, PhoneNumber, Role, Password) VALUES (@u, @e, @p, 'staff', @pwd)", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.Parameters.AddWithValue("@p", phone);
                        cmd.Parameters.AddWithValue("@pwd", password);
                        cmd.ExecuteNonQuery();
                    }
                }
                ClientScript.RegisterStartupScript(this.GetType(), "ok", "alert('Staff added successfully!'); window.location='ManageStaff.aspx';", true);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageStaff.aspx");
        }
    }
}