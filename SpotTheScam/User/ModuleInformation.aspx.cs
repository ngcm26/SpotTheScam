using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;

namespace SpotTheScam.User
{
    public partial class ModuleInformation : System.Web.UI.Page
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

		protected void Page_PreInit(object sender, EventArgs e)
		{
			bool isPreview = string.Equals(Request["preview"], "1") && Session["StaffName"] != null;
			this.MasterPageFile = isPreview ? "~/Staff/Staff.Master" : "~/User/User.Master";
		}

		protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
            {
				int moduleId;
				bool hasModuleId = int.TryParse(Request.QueryString["module_id"], out moduleId);
				bool loaded = false;
				if (hasModuleId)
				{
					loaded = LoadModuleInformation(moduleId);
					if (loaded) { CheckAndSetModuleCompletion(moduleId); }
				}

				var contentPanel = FindControl("pnlContent") as System.Web.UI.WebControls.Panel;
				var unavailablePanel = FindControl("pnlUnavailable") as System.Web.UI.WebControls.Panel;
				if (contentPanel != null) contentPanel.Visible = loaded;
				if (unavailablePanel != null) unavailablePanel.Visible = !loaded;

				// Staff preview/back-to-edit setup
				bool isPreview = string.Equals(Request["preview"], "1") && Session["StaffName"] != null;
				if (isPreview && hasModuleId)
				{
					if (lnkBackToEdit != null)
					{
						lnkBackToEdit.NavigateUrl = ResolveUrl("~/Staff/EditModule.aspx?module_id=" + moduleId);
						lnkBackToEdit.Visible = true;
					}
					// Hide completion and quiz actions in preview, and hide back-to-modules
					btnCompleteModule.Visible = false;
					lnkTakeQuiz.Visible = false;
					if (lnkBackToModules != null) lnkBackToModules.Visible = false;
				}
            }
        }

		private bool LoadModuleInformation(int moduleId)
        {
            string author = "";
            string date = "";
            // Get author and date from Modules table
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
				string modQuery = "SELECT author, date_created FROM Modules WHERE module_id = @module_id AND status = @status";
                using (SqlCommand modCmd = new SqlCommand(modQuery, conn))
                {
                    modCmd.Parameters.AddWithValue("@module_id", moduleId);
					modCmd.Parameters.AddWithValue("@status", "published");
                    using (SqlDataReader modReader = modCmd.ExecuteReader())
                    {
						if (modReader.Read())
                        {
                            author = modReader["author"].ToString();
                            DateTime dt;
                            if (DateTime.TryParse(modReader["date_created"].ToString(), out dt))
                                date = dt.ToString("MMMM d, yyyy");
                        }
						else
						{
							return false; // Not found or not published
						}
                    }
                }
            }
            lblAuthor.Text = author;
            lblDate.Text = date;

            // Get module info from ModuleInformation table
			using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                conn2.Open();
				bool isPreview = string.Equals(Request["preview"], "1") && Session["StaffName"] != null;
				string query = isPreview
					? "SELECT mi.*, m.module_name, mi.quiz_page FROM ModuleInformation mi INNER JOIN Modules m ON mi.module_id = m.module_id WHERE mi.module_id = @module_id"
					: "SELECT mi.*, m.module_name, mi.quiz_page FROM ModuleInformation mi INNER JOIN Modules m ON mi.module_id = m.module_id WHERE mi.module_id = @module_id AND m.status = @status";
                using (SqlCommand cmd = new SqlCommand(query, conn2))
                {
                    cmd.Parameters.AddWithValue("@module_id", moduleId);
					if (!isPreview) cmd.Parameters.AddWithValue("@status", "published");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblModuleName.Text = reader["module_name"].ToString();
                            lblIntroduction.Text = reader["introduction"].ToString();
                            lblDescription.Text = reader["description"].ToString();

                            // Header 1
                            if (!string.IsNullOrWhiteSpace(reader["header1"].ToString()))
                            {
                                lblHeader1.Text = reader["header1"].ToString();
                                lblHeader1.Visible = true;
                            }
                            if (!string.IsNullOrWhiteSpace(reader["header1_text"].ToString()))
                            {
                                lblHeader1Text.Text = reader["header1_text"].ToString();
                                lblHeader1Text.Visible = true;
                            }
                            // Header 2
                            if (!string.IsNullOrWhiteSpace(reader["header2"].ToString()))
                            {
                                lblHeader2.Text = reader["header2"].ToString();
                                lblHeader2.Visible = true;
                            }
                            if (!string.IsNullOrWhiteSpace(reader["header2_text"].ToString()))
                            {
                                lblHeader2Text.Text = reader["header2_text"].ToString();
                                lblHeader2Text.Visible = true;
                            }
                            // Header 3
                            if (!string.IsNullOrWhiteSpace(reader["header3"].ToString()))
                            {
                                lblHeader3.Text = reader["header3"].ToString();
                                lblHeader3.Visible = true;
                            }
                            if (!string.IsNullOrWhiteSpace(reader["header3_text"].ToString()))
                            {
                                lblHeader3Text.Text = reader["header3_text"].ToString();
                                lblHeader3Text.Visible = true;
                            }
                            // Header 4
                            if (!string.IsNullOrWhiteSpace(reader["header4"].ToString()))
                            {
                                lblHeader4.Text = reader["header4"].ToString();
                                lblHeader4.Visible = true;
                            }
                            if (!string.IsNullOrWhiteSpace(reader["header4_text"].ToString()))
                            {
                                lblHeader4Text.Text = reader["header4_text"].ToString();
                                lblHeader4Text.Visible = true;
                            }
                            // Header 5
                            if (!string.IsNullOrWhiteSpace(reader["header5"].ToString()))
                            {
                                lblHeader5.Text = reader["header5"].ToString();
                                lblHeader5.Visible = true;
                            }
                            if (!string.IsNullOrWhiteSpace(reader["header5_text"].ToString()))
                            {
                                lblHeader5Text.Text = reader["header5_text"].ToString();
                                lblHeader5Text.Visible = true;
                            }
                            // Images with validation and placeholders
                            img1.ImageUrl = GetValidatedImageUrl(reader["image1"].ToString());
                            img1.Visible = true;
                            img2.ImageUrl = GetValidatedImageUrl(reader["image2"].ToString());
                            img2.Visible = true;

                            // Quiz link – show only if module completed and quiz_page exists
                            string quizUrl = reader["quiz_page"].ToString();
                            if (!string.IsNullOrWhiteSpace(quizUrl))
                            {
                                lnkTakeQuiz.NavigateUrl = ResolveUrl(quizUrl);
                            }
                        }
                    }
                }
            }
  
			return !string.IsNullOrWhiteSpace(lblModuleName.Text);
		}

		private static bool TableExists(SqlConnection connection, string schemaName, string tableName)
		{
			using (var checkCmd = new SqlCommand("SELECT 1 FROM sys.tables WHERE name = @tableName AND schema_id = SCHEMA_ID(@schema)", connection))
			{
				checkCmd.Parameters.AddWithValue("@tableName", tableName);
				checkCmd.Parameters.AddWithValue("@schema", schemaName);
				var exists = checkCmd.ExecuteScalar();
				return exists != null;
			}
		}

		private void CheckAndSetModuleCompletion(int moduleId)
        {
            if (Session["UserId"] == null) return;
            int userId = Convert.ToInt32(Session["UserId"]);
            bool isCompleted = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
				// Only attempt to read progress if the tracking table exists
				if (TableExists(conn, "dbo", "UserModuleProgress"))
				{
					string query = "SELECT status FROM UserModuleProgress WHERE user_id = @user_id AND module_id = @module_id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@user_id", userId);
						cmd.Parameters.AddWithValue("@module_id", moduleId);
						var result = cmd.ExecuteScalar();
						if (result != null && result.ToString() == "completed")
							isCompleted = true;
						}
					}
				}
            btnCompleteModule.Visible = !isCompleted;
            lblCompleteMessage.Visible = isCompleted;
            // Expose quiz button only when completed and a quiz link is available
            if (isCompleted && lnkTakeQuiz != null && !string.IsNullOrWhiteSpace(lnkTakeQuiz.NavigateUrl))
            {
                lnkTakeQuiz.Visible = true;
            }
            else if (lnkTakeQuiz != null)
            {
                lnkTakeQuiz.Visible = false;
            }
            if (isCompleted)
            {
                lblCompleteMessage.Text = "You have completed this module.";
            }
            else
            {
                lblCompleteMessage.Text = "";
            }
        }

        protected void btnCompleteModule_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Request.QueryString["module_id"] == null) return;
            int userId = Convert.ToInt32(Session["UserId"]);
            int moduleId = Convert.ToInt32(Request.QueryString["module_id"]);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
				if (!TableExists(conn, "dbo", "UserModuleProgress"))
				{
					lblCompleteMessage.Text = "Progress tracking is currently unavailable.";
					lblCompleteMessage.Visible = true;
					btnCompleteModule.Visible = false;
					return;
				}
                // Check if record exists
                string checkQuery = "SELECT COUNT(*) FROM UserModuleProgress WHERE user_id = @user_id AND module_id = @module_id";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@user_id", userId);
                    checkCmd.Parameters.AddWithValue("@module_id", moduleId);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        // Update to completed
                        string updateQuery = "UPDATE UserModuleProgress SET status = 'completed', date_completed = GETDATE() WHERE user_id = @user_id AND module_id = @module_id";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@user_id", userId);
                            updateCmd.Parameters.AddWithValue("@module_id", moduleId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Insert new record
                        string insertQuery = "INSERT INTO UserModuleProgress (user_id, module_id, status, date_started, date_completed) VALUES (@user_id, @module_id, 'completed', GETDATE(), GETDATE())";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@user_id", userId);
                            insertCmd.Parameters.AddWithValue("@module_id", moduleId);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
			// Update UI immediately
			lblCompleteMessage.Text = "You have completed this module.";
			lblCompleteMessage.Visible = true;
			btnCompleteModule.Visible = false;
			if (lnkTakeQuiz != null && !string.IsNullOrWhiteSpace(lnkTakeQuiz.NavigateUrl))
			{
				lnkTakeQuiz.Visible = true;
			}
			// Keep user at the bottom near the quiz button
			string targetId = (lnkTakeQuiz != null) ? lnkTakeQuiz.ClientID : "";
			string scrollScript = string.IsNullOrEmpty(targetId)
				? "window.scrollTo(0, document.body.scrollHeight);"
				: ("var el=document.getElementById('" + targetId + "'); if(el){ el.scrollIntoView({behavior:'smooth',block:'center'}); } else { window.scrollTo(0, document.body.scrollHeight); }");
			ClientScript.RegisterStartupScript(this.GetType(), "ScrollToBottomAfterComplete", scrollScript, true);
        }

		private string GetValidatedImageUrl(string rawUrl)
		{
			string placeholder = ResolveUrl("~/Images/govscam.jpg");
			if (string.IsNullOrWhiteSpace(rawUrl)) return placeholder;

			// Allow external absolute URLs
			if (rawUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || rawUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				return rawUrl;
			}

			string resolved = ResolveUrl(rawUrl);
			try
			{
				string physicalPath = Server.MapPath(resolved);
				if (!string.IsNullOrEmpty(physicalPath) && File.Exists(physicalPath))
				{
					return resolved;
				}
			}
			catch
			{
				// Ignore mapping errors and fall back to placeholder
			}
			return placeholder;
		}
    }
}