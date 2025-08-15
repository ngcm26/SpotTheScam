using System;
using System.Configuration;
using System.Data.SqlClient;
using SpotTheScam.Utils;
using System.Security.Cryptography;

namespace SpotTheScam.User
{
	public partial class ForgotPassword : System.Web.UI.Page
	{
		private readonly string cs = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void btnSend_Click(object sender, EventArgs e)
		{
			string email = txtEmail.Text.Trim();
			if (string.IsNullOrEmpty(email))
			{
				lblMessage.Text = "Please enter your email.";
				return;
			}

			using (var con = new SqlConnection(cs))
			{
				con.Open();

				// Check if email belongs to staff
				string role = null;
				int userId = 0;
				using (var cmd = new SqlCommand("SELECT Id, Role FROM Users WHERE Email=@e", con))
				{
					cmd.Parameters.AddWithValue("@e", email);
					using (var r = cmd.ExecuteReader())
					{
						if (r.Read())
						{
							userId = Convert.ToInt32(r["Id"]);
							role = (r["Role"] as string ?? string.Empty).ToLower();
						}
					}
				}

				if (string.IsNullOrEmpty(role))
				{
					// Avoid account enumeration: generic success message
					lblMessage.Text = "If the email is registered, a password reset link has been sent.";
					return;
				}

				if (role == "staff" || role == "admin")
				{
					lblMessage.Text = "This email belongs to a staff account. Please contact your administrator to reset your password.";
					return;
				}

				// Reuse existing, unexpired token if present; otherwise create a new one
				string token;
				DateTime expiresAt;
				using (var get = new SqlCommand("SELECT VerifyCode, VerifyCodeExpiresAt FROM Users WHERE Id=@id AND VerifyCode IS NOT NULL AND VerifyCodeExpiresAt > GETUTCDATE()", con))
				{
					get.Parameters.AddWithValue("@id", userId);
					using (var r2 = get.ExecuteReader())
					{
						if (r2.Read())
						{
							token = Convert.ToString(r2["VerifyCode"]);
							expiresAt = Convert.ToDateTime(r2["VerifyCodeExpiresAt"]);
						}
						else
						{
							token = GenerateNumericToken(6);
							expiresAt = DateTime.UtcNow.AddHours(1);
						}
					}
				}

				// If we generated a new token, persist it
				using (var set = new SqlCommand("UPDATE Users SET VerifyCode=@c, VerifyCodeExpiresAt=@exp WHERE Id=@id", con))
				{
					set.Parameters.AddWithValue("@c", token);
					set.Parameters.AddWithValue("@exp", expiresAt);
					set.Parameters.AddWithValue("@id", userId);
					set.ExecuteNonQuery();
				}

				// Build reset URL
				string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/User/ResetPassword.aspx");
				string resetUrl = baseUrl + "?token=" + Server.UrlEncode(token) + "&email=" + Server.UrlEncode(email);

				try
				{
					EmailService.Send(email, "Reset your Spot The Scam password",
						$"<p>We received a request to reset your password.</p>" +
						$"<p><a href='{resetUrl}'>Click here to reset your password</a>. This link will expire in 1 hour.</p>" +
						"<p>If you did not request this, you can safely ignore this email.</p>");
				}
				catch { /* Ignore email errors, but still show generic message */ }

				lblMessage.Text = "If the email is registered, a password reset link has been sent.";
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
	}
}


