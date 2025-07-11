using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace SpotTheScam.Staff
{
    public partial class StaffAddModules : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check staff session
                if (Session["StaffName"] == null)
                {
                    Response.Redirect("StaffDashboard.aspx");
                }
            }
        }

        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            if (Session["StaffName"] == null)
            {
                Response.Redirect("StaffDashboard.aspx");
                return;
            }

            string moduleName = txtModuleName.Text.Trim();
            string author = Session["StaffName"]?.ToString() ?? "Unknown";
            string status = ddlStatus.SelectedValue;
            string coverImagePath = "";

            // Validation
            if (string.IsNullOrEmpty(moduleName))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Module name is required.";
                return;
            }

            try
            {
                // Make sure the folder exists
                string folderPath = Server.MapPath("~/Uploads/Modules/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                if (fuCoverImage.HasFile)
                {
                    // Validate file type
                    string fileExtension = Path.GetExtension(fuCoverImage.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

                    if (Array.IndexOf(allowedExtensions, fileExtension) == -1)
                    {
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Text = "Please select a valid image file (jpg, jpeg, png, gif, bmp).";
                        return;
                    }

                    // Create unique filename to avoid conflicts
                    string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(fuCoverImage.FileName);
                    string savePath = Path.Combine(folderPath, filename);
                    fuCoverImage.SaveAs(savePath);

                    // Store relative path for DB (without ~/)
                    coverImagePath = "Uploads/Modules/" + filename;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Modules 
                             (module_name, cover_image, author, status, date_created, date_updated)
                             VALUES 
                             (@module_name, @cover_image, @author, @status,
                              GETDATE(), GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@module_name", moduleName);
                        cmd.Parameters.AddWithValue("@cover_image", coverImagePath);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@author", author);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Clear form after successful insertion
                txtModuleName.Text = "";
                ddlStatus.SelectedIndex = 0;

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Module added successfully!";
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Error: " + ex.Message;
            }
        }
    }
}