using System;
using System.Configuration;
using System.Data.SqlClient;
using SpotTheScam.Utils;

namespace SpotTheScam.User
{
	public partial class VerifyEmail : System.Web.UI.Page
	{
		private readonly string cs = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				var q = Request.QueryString["email"];
				if (!string.IsNullOrEmpty(q)) txtEmail.Text = q;
			}
		}

		protected void btnVerify_Click(object sender, EventArgs e)
		{
			string email = txtEmail.Text.Trim();
			string code = txtCode.Text.Trim();

			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
			{
				lblMessage.Text = "Please enter your email and code.";
				return;
			}

			using (var con = new SqlConnection(cs))
			{
				con.Open();

				// Ensure not already verified, code matches, and not expired
				string select = @"SELECT COUNT(*) FROM Users 
					WHERE Email=@e AND Verify=0 AND VerifyCode=@c AND (VerifyCodeExpiresAt IS NOT NULL AND VerifyCodeExpiresAt > GETUTCDATE())";
				using (var check = new SqlCommand(select, con))
				{
					check.Parameters.AddWithValue("@e", email);
					check.Parameters.AddWithValue("@c", code);
					int ok = (int)check.ExecuteScalar();
					if (ok == 0)
					{
						lblMessage.Text = "Invalid or expired code, or account already verified.";
						return;
					}
				}

				string update = "UPDATE Users SET Verify=1, VerifyCode=NULL, VerifyCodeExpiresAt=NULL WHERE Email=@e AND VerifyCode=@c";
				using (var cmd = new SqlCommand(update, con))
				{
					cmd.Parameters.AddWithValue("@e", email);
					cmd.Parameters.AddWithValue("@c", code);
					cmd.ExecuteNonQuery();
				}

				// Auto-login and redirect
				using (var getUser = new SqlCommand("SELECT Id, Username, Role FROM Users WHERE Email=@e", con))
				{
					getUser.Parameters.AddWithValue("@e", email);
					using (var r = getUser.ExecuteReader())
					{
						if (r.Read())
						{
							int userId = Convert.ToInt32(r["Id"]);
							string username = r["Username"].ToString();
							string role = r["Role"].ToString().ToLower();

							Session["UserId"] = userId;
							Session["Username"] = username;

							if (role == "admin")
							{
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
								Response.Redirect("UserHome.aspx");
							}
							return;
						}
					}
				}

				// fallback
				Response.Redirect("UserLogin.aspx");
			}
		}

		protected void btnResend_Click(object sender, EventArgs e)
		{
			string email = txtEmail.Text.Trim();
			if (string.IsNullOrEmpty(email))
			{
				lblMessage.Text = "Enter your email to resend the code.";
				return;
			}

			var otp = new Random().Next(100000, 999999).ToString();
			var expiresAtUtc = DateTime.UtcNow.AddMinutes(10);

			using (var con = new SqlConnection(cs))
			{
				con.Open();
				// Only allow resend for accounts not yet verified
				string update = @"UPDATE Users SET VerifyCode=@c, VerifyCodeExpiresAt=@exp
					WHERE Email=@e AND Verify=0";
				using (var cmd = new SqlCommand(update, con))
				{
					cmd.Parameters.AddWithValue("@c", otp);
					cmd.Parameters.AddWithValue("@exp", expiresAtUtc);
					cmd.Parameters.AddWithValue("@e", email);
					int rows = cmd.ExecuteNonQuery();
					if (rows == 0)
					{
						lblMessage.Text = "This account is already verified or does not exist.";
						return;
					}
				}
			}

			try { EmailService.Send(email, "Your Spot The Scam verification code", $"<p>Your new verification code is:</p><h2>{otp}</h2><p>This code expires in 10 minutes.</p>"); lblMessage.Text = "A new code has been sent to your email."; } catch { lblMessage.Text = "Failed to send email. Please try again later."; }
		}
	}
}


