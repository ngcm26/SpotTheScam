using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
	public partial class ResetPassword : System.Web.UI.Page
	{
		private readonly string cs = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				// Basic token presence check
				string token = Request.QueryString["token"];
				string email = Request.QueryString["email"];
				if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
				{
					lblMessage.Text = "This password reset link has expired or is no longer valid. Please request a new link.";
					phForm.Visible = false;
					phBackOnly.Visible = true;
					ForceGuestNavbar();
					return;
				}

				// Validate token immediately on page load; if invalid, disable the form
				using (var con = new SqlConnection(cs))
				{
					con.Open();
					string select = @"SELECT COUNT(*) FROM Users WHERE Email=@e AND VerifyCode=@t AND (VerifyCodeExpiresAt IS NOT NULL AND VerifyCodeExpiresAt > GETUTCDATE())";
					using (var check = new SqlCommand(select, con))
					{
						check.Parameters.AddWithValue("@e", email);
						check.Parameters.AddWithValue("@t", token);
						int ok = (int)check.ExecuteScalar();
						if (ok == 0)
						{
							lblMessage.Text = "This password reset link has expired or is no longer valid. Please request a new link.";
							phForm.Visible = false;
							phBackOnly.Visible = true;
							ForceGuestNavbar();
						}
					}
				}
			}
		}

		protected void btnReset_Click(object sender, EventArgs e)
		{
			string token = Request.QueryString["token"] ?? string.Empty;
			string email = Request.QueryString["email"] ?? string.Empty;
			string password = txtPassword.Text;
			string confirm = txtConfirm.Text;

			if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
			{
				lblMessage.Text = "Invalid reset request.";
				return;
			}

			// Strong password requirements
			string pwdError;
			if (!IsPasswordStrong(password, email, out pwdError))
			{
				lblMessage.Text = pwdError;
				return;
			}

			if (!string.Equals(password, confirm, StringComparison.Ordinal))
			{
				lblMessage.Text = "Passwords do not match.";
				return;
			}

			using (var con = new SqlConnection(cs))
			{
				con.Open();
				// Validate token and expiry and that it's for this email
				string select = @"SELECT COUNT(*) FROM Users WHERE Email=@e AND VerifyCode=@t AND (VerifyCodeExpiresAt IS NOT NULL AND VerifyCodeExpiresAt > GETUTCDATE())";
				using (var check = new SqlCommand(select, con))
				{
					check.Parameters.AddWithValue("@e", email);
					check.Parameters.AddWithValue("@t", token);
					int ok = (int)check.ExecuteScalar();
					if (ok == 0)
					{
						lblMessage.Text = "This password reset link has expired or is no longer valid. Please request a new link.";
						phForm.Visible = false;
						phBackOnly.Visible = true;
						ForceGuestNavbar();
						return;
					}
				}

				// Ensure new password is not the same as the current one
				string currentPassword = null;
				using (var get = new SqlCommand("SELECT Password FROM Users WHERE Email=@e", con))
				{
					get.Parameters.AddWithValue("@e", email);
					var result = get.ExecuteScalar();
					currentPassword = result == null ? null : Convert.ToString(result);
				}

				if (!string.IsNullOrEmpty(currentPassword) && string.Equals(currentPassword, password, StringComparison.Ordinal))
				{
					lblMessage.Text = "Your new password must be different from your current password.";
					return;
				}

				// Update password and clear token
				using (var upd = new SqlCommand("UPDATE Users SET Password=@p, VerifyCode=NULL, VerifyCodeExpiresAt=NULL WHERE Email=@e", con))
				{
					upd.Parameters.AddWithValue("@p", password);
					upd.Parameters.AddWithValue("@e", email);
					upd.ExecuteNonQuery();
				}
			}

			// Redirect to login with success message
			Response.Redirect("UserLogin.aspx?reset=success");
		}

		private static bool IsPasswordStrong(string password, string email, out string error)
		{
			error = string.Empty;
			if (string.IsNullOrWhiteSpace(password))
			{
				error = "Password cannot be empty.";
				return false;
			}

			if (password.Length < 8 || password.Length > 64)
			{
				error = "Password must be between 8 and 64 characters.";
				return false;
			}

			if (password.Trim() != password)
			{
				error = "Password cannot start or end with spaces.";
				return false;
			}

			if (!Regex.IsMatch(password, @"[A-Z]"))
			{
				error = "Password must include at least one uppercase letter.";
				return false;
			}

			if (!Regex.IsMatch(password, @"[a-z]"))
			{
				error = "Password must include at least one lowercase letter.";
				return false;
			}

			if (!Regex.IsMatch(password, @"\d"))
			{
				error = "Password must include at least one digit.";
				return false;
			}

			if (!Regex.IsMatch(password, @"[^A-Za-z0-9]"))
			{
				error = "Password must include at least one symbol.";
				return false;
			}

			// Do not allow password to contain the local part of the email
			if (!string.IsNullOrEmpty(email) && email.Contains("@"))
			{
				var local = email.Split('@')[0];
				if (!string.IsNullOrEmpty(local) && password.IndexOf(local, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					error = "Password cannot contain your email username.";
					return false;
				}
			}

			return true;
		}

		private void ForceGuestNavbar()
		{
			var phUserCtrl = Master.FindControl("phUser") as PlaceHolder;
			var phGuestCtrl = Master.FindControl("phGuest") as PlaceHolder;
			if (phUserCtrl != null) phUserCtrl.Visible = false;
			if (phGuestCtrl != null) phGuestCtrl.Visible = true;
		}
	}
}


