using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.Configuration;
using SpotTheScam.Utils;

namespace SpotTheScam
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reset = Request.QueryString["reset"];
                if (!string.IsNullOrEmpty(reset) && reset.Equals("success", StringComparison.OrdinalIgnoreCase))
                {
                    lblMessage.ForeColor = Color.Green;
                    lblMessage.Text = "Your password has been reset. Please log in.";
                }
            }
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
                    // Pull Username, Role, Id, Status, and Verify
                    string query = "SELECT Id, Username, Role, Status, Verify FROM Users WHERE Email = @Email AND Password = @Password";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            int userId = Convert.ToInt32(reader["Id"]); // ADDED LINE
                            string username = reader["Username"].ToString();
                            string role = reader["Role"].ToString().ToLower();
                            string status = (reader["Status"] as string ?? "active").ToLower();
                            bool isVerified = false;
                            try { isVerified = Convert.ToBoolean(reader["Verify"]); } catch { isVerified = false; }

                            if (status == "suspended")
                            {
                                lblMessage.ForeColor = Color.Red;
                                lblMessage.Text = role == "staff"
                                    ? "Your staff account has been suspended. Please contact your administrator."
                                    : "Your account has been suspended. Please contact support at support@spotthescam.cs.com";
                                return;
                            }

                            if (!isVerified)
                            {
                                reader.Close();

                                // Check if an active OTP exists
                                bool hasActiveOtp = false;
                                using (var check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email=@e AND VerifyCode IS NOT NULL AND VerifyCodeExpiresAt > GETUTCDATE()", con))
                                {
                                    check.Parameters.AddWithValue("@e", email);
                                    hasActiveOtp = ((int)check.ExecuteScalar()) > 0;
                                }

                                // If no active code, generate a new one and send
                                if (!hasActiveOtp)
                                {
                                    var otp = new Random().Next(100000, 999999).ToString();
                                    using (var set = new SqlCommand("UPDATE Users SET VerifyCode=@c, VerifyCodeExpiresAt=@exp WHERE Email=@e AND Verify=0", con))
                                    {
                                        set.Parameters.AddWithValue("@c", otp);
                                        set.Parameters.AddWithValue("@exp", DateTime.UtcNow.AddMinutes(10));
                                        set.Parameters.AddWithValue("@e", email);
                                        set.ExecuteNonQuery();
                                    }
                                    try { EmailService.Send(email, "Your Spot The Scam verification code", $"<p>Your verification code is:</p><h2>{otp}</h2><p>This code expires in 10 minutes.</p>"); } catch { }
                                }

                                Response.Redirect("VerifyEmail.aspx?email=" + Server.UrlEncode(email));
                                return;
                            }

                            Session["UserId"] = userId; // ADDED LINE
                            Session["Username"] = username;

                            if (role == "admin")
                            {
                                // Admin → redirect to staff dashboard
                                Session["StaffName"] = username;
                                Session["StaffRole"] = "admin";
                                Response.Redirect("~/Staff/StaffDashboard.aspx");
                            }
                            else if (role == "staff")
                            {
                                Session["StaffName"] = username;
                                Session["StaffRole"] = "staff";
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