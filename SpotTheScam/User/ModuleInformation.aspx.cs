using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace SpotTheScam.User
{
    public partial class ModuleInformation : System.Web.UI.Page
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["module_id"] != null)
            {
                int moduleId;
                if (int.TryParse(Request.QueryString["module_id"], out moduleId))
                {
                    LoadModuleInformation(moduleId);
                    CheckAndSetModuleCompletion(moduleId);
                }
            }
        }

        private void LoadModuleInformation(int moduleId)
        {
            string author = "";
            string date = "";
            // Get author and date from Modules table
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string modQuery = "SELECT author, date_created FROM Modules WHERE module_id = @module_id";
                using (SqlCommand modCmd = new SqlCommand(modQuery, conn))
                {
                    modCmd.Parameters.AddWithValue("@module_id", moduleId);
                    using (SqlDataReader modReader = modCmd.ExecuteReader())
                    {
                        if (modReader.Read())
                        {
                            author = modReader["author"].ToString();
                            DateTime dt;
                            if (DateTime.TryParse(modReader["date_created"].ToString(), out dt))
                                date = dt.ToString("MMMM d, yyyy");
                        }
                    }
                }
            }
            lblAuthor.Text = author;
            lblDate.Text = date;

            // Get module info from ModuleInformation table
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT mi.*, m.module_name FROM ModuleInformation mi INNER JOIN Modules m ON mi.module_id = m.module_id WHERE mi.module_id = @module_id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@module_id", moduleId);
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
                            // Images
                            if (!string.IsNullOrWhiteSpace(reader["image1"].ToString()))
                            {
                                img1.ImageUrl = ResolveUrl(reader["image1"].ToString());
                                img1.Visible = true;
                            }
                            if (!string.IsNullOrWhiteSpace(reader["image2"].ToString()))
                            {
                                img2.ImageUrl = ResolveUrl(reader["image2"].ToString());
                                img2.Visible = true;
                            }
                        }
                    }
                }
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
            btnCompleteModule.Visible = !isCompleted;
            lblCompleteMessage.Visible = isCompleted;
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
            lblCompleteMessage.Text = "Module marked as complete!";
            lblCompleteMessage.Visible = true;
            btnCompleteModule.Visible = false;
        }
    }
}