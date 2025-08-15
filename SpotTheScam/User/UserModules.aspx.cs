using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SpotTheScam.User
{
    public partial class UserModules : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Require login
            if (Session["Username"] == null && Session["UserId"] == null)
            {
                Response.Redirect("UserLogin.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadPublishedModules();
            }
        }

        private void LoadPublishedModules()
        {
            DataTable dt = new DataTable();

            try
            {
                int userId = 0;
                if (Session["UserId"] != null)
                {
                    userId = Convert.ToInt32(Session["UserId"]);
                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Get all published modules
                    string query = "SELECT module_id, module_name, cover_image FROM Modules WHERE status = @status";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", "published");
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }

                    // Add completed column
                    dt.Columns.Add("completed", typeof(bool));
                    // Default to false so UI bindings are safe even when user has no progress
                    foreach (DataRow row in dt.Rows)
                    {
                        row["completed"] = false;
                    }

                    // Get completed modules for this user, if the progress table exists
                    if (userId != 0 && dt.Rows.Count > 0)
                    {
                        bool hasUserModuleProgressTable = false;
                        using (SqlCommand checkCmd = new SqlCommand(
                            "SELECT 1 FROM sys.tables WHERE name = @tableName AND schema_id = SCHEMA_ID('dbo')",
                            conn))
                        {
                            checkCmd.Parameters.AddWithValue("@tableName", "UserModuleProgress");
                            object existsResult = checkCmd.ExecuteScalar();
                            hasUserModuleProgressTable = existsResult != null;
                        }

                        if (hasUserModuleProgressTable)
                        {
                            string completedQuery = "SELECT module_id FROM UserModuleProgress WHERE user_id = @user_id AND status = 'completed'";
                            using (SqlCommand completedCmd = new SqlCommand(completedQuery, conn))
                            {
                                completedCmd.Parameters.AddWithValue("@user_id", userId);
                                using (SqlDataReader reader = completedCmd.ExecuteReader())
                                {
                                    var completedModules = new System.Collections.Generic.HashSet<int>();
                                    while (reader.Read())
                                    {
                                        completedModules.Add(Convert.ToInt32(reader["module_id"]));
                                    }
                                    // Set completed flag for each module
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        int moduleId = Convert.ToInt32(row["module_id"]);
                                        row["completed"] = completedModules.Contains(moduleId);
                                    }
                                }
                            }
                        }
                        // If the table doesn't exist, we keep the default false values
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    rptModules.DataSource = dt;
                    rptModules.DataBind();
                }
                else
                {
                    lblMessage.Text = "No published modules available.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }
    }
}
