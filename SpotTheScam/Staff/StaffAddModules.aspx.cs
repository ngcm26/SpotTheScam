using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace SpotTheScam.Staff
{
    public partial class StaffAddModules : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
        private const string TempCoverImageSessionKey = "TempCoverImagePath";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["StaffName"] == null)
                {
                    Response.Redirect("StaffDashboard.aspx");
                }
                // Load persisted preview if present
                if (Session[TempCoverImageSessionKey] != null)
                {
                    hdnCoverImagePath.Value = Session[TempCoverImageSessionKey].ToString();
                }
            }
        }

        protected void btnUploadCover_Click(object sender, EventArgs e)
        {
            try
            {
                // If a cropped base64 image is provided, save it; else fallback to raw upload
                if (!string.IsNullOrWhiteSpace(hdnCoverImageBase64.Value) && hdnCoverImageBase64.Value.StartsWith("data:image"))
                {
                    string base64 = hdnCoverImageBase64.Value;
                    var commaIndex = base64.IndexOf(',');
                    if (commaIndex > 0)
                    {
                        string meta = base64.Substring(0, commaIndex);
                        string data = base64.Substring(commaIndex + 1);
                        byte[] bytes = Convert.FromBase64String(data);

                        string ext = meta.Contains("png") ? ".png" : ".jpg";
                        string folderPath = Server.MapPath("~/Uploads/Modules/");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                        string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "_cropped" + ext;
                        string savePath = Path.Combine(folderPath, filename);
                        File.WriteAllBytes(savePath, bytes);
                        string relativePath = "Uploads/Modules/" + filename;

                        Session[TempCoverImageSessionKey] = relativePath;
                        hdnCoverImagePath.Value = relativePath;
                        hdnCoverImageBase64.Value = string.Empty;

                        string script = "(function(){var p=document.getElementById('coverImagePreview'); if(p){p.innerHTML=''; var img=new Image(); img.style.width='100%'; img.style.height='100%'; img.style.objectFit='cover'; img.style.borderRadius='16px'; img.src='" + ResolveUrl("~/" + relativePath) + "'; p.appendChild(img);} })();";
                        ClientScript.RegisterStartupScript(this.GetType(), "UpdateCoverPreview", script, true);
                        return;
                    }
                }

                if (fuCoverImage.HasFile)
                {
                    string folderPath = Server.MapPath("~/Uploads/Modules/");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

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
                    string relativePath = "Uploads/Modules/" + filename;

                    // Persist path for later save and reflect in preview
                    Session[TempCoverImageSessionKey] = relativePath;
                    hdnCoverImagePath.Value = relativePath;

                    // Update preview instantly with server path
                    string script = "(function(){var p=document.getElementById('coverImagePreview'); if(p){p.innerHTML=''; var img=new Image(); img.style.width='100%'; img.style.height='100%'; img.style.objectFit='cover'; img.style.borderRadius='16px'; img.src='" + ResolveUrl("~/" + relativePath) + "'; p.appendChild(img);} })();";
                    ClientScript.RegisterStartupScript(this.GetType(), "UpdateCoverPreview", script, true);
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Error uploading image: " + ex.Message;
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
            string coverImagePath = hdnCoverImagePath.Value ?? "";
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

            if (string.IsNullOrEmpty(moduleName) ||
                string.IsNullOrEmpty(status) ||
                string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(introduction) ||
                string.IsNullOrWhiteSpace(header1) ||
                string.IsNullOrWhiteSpace(header1Text) ||
                string.IsNullOrWhiteSpace(header2) ||
                string.IsNullOrWhiteSpace(header2Text) ||
                string.IsNullOrWhiteSpace(header3) ||
                string.IsNullOrWhiteSpace(header3Text) ||
                string.IsNullOrWhiteSpace(header4) ||
                string.IsNullOrWhiteSpace(header4Text) ||
                string.IsNullOrWhiteSpace(header5) ||
                string.IsNullOrWhiteSpace(header5Text) ||
                string.IsNullOrWhiteSpace(coverImagePath) ||
                (!fuImage1.HasFile && string.IsNullOrWhiteSpace(image1Path)) ||
                (!fuImage2.HasFile && string.IsNullOrWhiteSpace(image2Path)))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "All fields and images are required.";
                return;
            }

            try
            {
                string folderPath = Server.MapPath("~/Uploads/Modules/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Use previously uploaded temp cover image if available
                if (!string.IsNullOrWhiteSpace(hdnCoverImagePath.Value))
                {
                    coverImagePath = hdnCoverImagePath.Value;
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

                // Clear persisted temp image
                hdnCoverImagePath.Value = "";
                Session.Remove(TempCoverImageSessionKey);

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Module and details added successfully!";

                // Expose preview for the newly created module
                if (lnkPreviewNew != null && newModuleId > 0)
                {
                    lnkPreviewNew.NavigateUrl = ResolveUrl("~/User/ModuleInformation.aspx?module_id=" + newModuleId + "&preview=1");
                    lnkPreviewNew.Visible = true;
                }
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
