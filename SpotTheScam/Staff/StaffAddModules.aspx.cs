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
            string description = txtDescription.Text.Trim();
            string introduction = txtIntroduction.Text.Trim();
            string header1 = txtHeader1.Text.Trim();
            string header1Text = txtHeader1Text.Text.Trim();
            string header2 = txtHeader2.Text.Trim();
            string header2Text = txtHeader2Text.Text.Trim();
            string header3 = txtHeader3.Text.Trim();
            string header3Text = txtHeader3Text.Text.Trim();
            string header4 = txtHeader4.Text.Trim();
            string header4Text = txtHeader4Text.Text.Trim();
            string header5 = txtHeader5.Text.Trim();
            string header5Text = txtHeader5Text.Text.Trim();
            string image1Path = "";
            string image2Path = "";

            if (string.IsNullOrEmpty(moduleName))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Module name is required.";
                return;
            }

            try
            {
                string folderPath = Server.MapPath("~/Uploads/Modules/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                if (fuCoverImage.HasFile)
                {
                    string fileExtension = Path.GetExtension(fuCoverImage.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    if (Array.IndexOf(allowedExtensions, fileExtension) == -1)
                    {
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Text = "Please select a valid image file (jpg, jpeg, png, gif, bmp).";
                        return;
                    }
                    string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(fuCoverImage.FileName);
                    string savePath = Path.Combine(folderPath, filename);
                    fuCoverImage.SaveAs(savePath);
                    coverImagePath = "Uploads/Modules/" + filename;
                }

                if (fuImage1.HasFile)
                {
                    string fileExtension = Path.GetExtension(fuImage1.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    if (Array.IndexOf(allowedExtensions, fileExtension) != -1)
                    {
                        string filename = "img1_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(fuImage1.FileName);
                        string savePath = Path.Combine(folderPath, filename);
                        fuImage1.SaveAs(savePath);
                        image1Path = "Uploads/Modules/" + filename;
                    }
                }

                if (fuImage2.HasFile)
                {
                    string fileExtension = Path.GetExtension(fuImage2.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    if (Array.IndexOf(allowedExtensions, fileExtension) != -1)
                    {
                        string filename = "img2_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(fuImage2.FileName);
                        string savePath = Path.Combine(folderPath, filename);
                        fuImage2.SaveAs(savePath);
                        image2Path = "Uploads/Modules/" + filename;
                    }
                }

                int newModuleId = 0;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"INSERT INTO Modules 
                        (module_name, cover_image, author, status, date_created, date_updated)
                        VALUES 
                        (@module_name, @cover_image, @author, @status,
                        GETDATE(), GETDATE());
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@module_name", moduleName);
                        cmd.Parameters.AddWithValue("@cover_image", coverImagePath ?? "");
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@author", author);
                        object result = cmd.ExecuteScalar();
                        newModuleId = Convert.ToInt32(result);
                    }

                    // Debug message before inserting ModuleInformation
                    lblMessage.ForeColor = System.Drawing.Color.Black;
                    lblMessage.Text = "Saving detailed module information...";

                    string infoQuery = @"INSERT INTO ModuleInformation
                        (module_id, description, introduction, header1, header1_text, header2, header2_text, header3, header3_text, header4, header4_text, header5, header5_text, image1, image2)
                        VALUES
                        (@module_id, @description, @introduction, @header1, @header1_text, @header2, @header2_text, @header3, @header3_text, @header4, @header4_text, @header5, @header5_text, @image1, @image2)";

                    using (SqlCommand cmd = new SqlCommand(infoQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@module_id", newModuleId);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@introduction", introduction);
                        cmd.Parameters.AddWithValue("@header1", header1);
                        cmd.Parameters.AddWithValue("@header1_text", header1Text);
                        cmd.Parameters.AddWithValue("@header2", header2);
                        cmd.Parameters.AddWithValue("@header2_text", header2Text);
                        cmd.Parameters.AddWithValue("@header3", header3);
                        cmd.Parameters.AddWithValue("@header3_text", header3Text);
                        cmd.Parameters.AddWithValue("@header4", header4);
                        cmd.Parameters.AddWithValue("@header4_text", header4Text);
                        cmd.Parameters.AddWithValue("@header5", header5);
                        cmd.Parameters.AddWithValue("@header5_text", header5Text);
                        cmd.Parameters.AddWithValue("@image1", image1Path ?? "");
                        cmd.Parameters.AddWithValue("@image2", image2Path ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }

                // Clear form
                txtModuleName.Text = "";
                ddlStatus.SelectedIndex = 0;
                txtDescription.Text = "";
                txtIntroduction.Text = "";
                txtHeader1.Text = "";
                txtHeader1Text.Text = "";
                txtHeader2.Text = "";
                txtHeader2Text.Text = "";
                txtHeader3.Text = "";
                txtHeader3Text.Text = "";
                txtHeader4.Text = "";
                txtHeader4Text.Text = "";
                txtHeader5.Text = "";
                txtHeader5Text.Text = "";

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Module and details added successfully!";
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Error: " + ex.ToString(); // full trace for debugging
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewModules.aspx");
        }
    }
}
