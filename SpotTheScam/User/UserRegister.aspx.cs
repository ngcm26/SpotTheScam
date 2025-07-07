using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.Configuration;

namespace SpotTheScam
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // You can add initialization logic here if needed.
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // First, check if the page passes all validation rules (Required, Compare, etc.)
            if (!Page.IsValid)
            {
                return;
            }

            // Retrieve the trimmed input from the textboxes
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text; // Password should not be trimmed

            // It's good practice to hash passwords before storing them.
            // For this example, we'll store it as plain text as in your original code.
            // In a real-world app, use a library like BCrypt.Net to hash passwords.

            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                try
                {
                    con.Open();

                    // Step 1: Check if the username or email already exists in the database.
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        checkCmd.Parameters.AddWithValue("@Email", email);

                        int userExists = (int)checkCmd.ExecuteScalar();
                        if (userExists > 0)
                        {
                            // If a user is found, display an error message.
                            lblMessage.ForeColor = Color.Red;
                            lblMessage.Text = "A user with that username or email already exists. Please try another.";
                            return;
                        }
                    }

                    // Step 2: If no existing user, insert the new user record.
                    // Your table schema should have at least Username, Email, and Password columns.
                    string insertQuery = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password); // Storing plain text password

                        cmd.ExecuteNonQuery();

                        // Display a success message and provide a link to the login page.
                        lblMessage.ForeColor = Color.Green;
                        lblMessage.Text = "Account created successfully! You can now <a href='UserLogin.aspx'>login here</a>.";

                        // Optionally, disable the button to prevent double submission
                        btnRegister.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    // In case of a database error, show a generic error message.
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "An error occurred while creating your account. Please try again later.";
                    // It's a good practice to log the detailed error (ex.ToString()) for debugging.
                }
            }
        }
    }
}
