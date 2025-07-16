using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam
{
    public partial class EditModule : System.Web.UI.Page
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
        private int moduleId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["StaffName"] == null)
            {
                Response.Redirect("StaffDashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["module_id"] != null && int.TryParse(Request.QueryString["module_id"], out moduleId))
                {
                    LoadModuleInformation();
                }
                else
                {
                    ShowError("No module ID provided");
                }
            }
        }

        private void LoadModuleInformation()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Load from Modules table for author and status
                    string modQuery = "SELECT author, status FROM Modules WHERE module_id = @module_id";
                    using (SqlCommand modCmd = new SqlCommand(modQuery, conn))
                    {
                        modCmd.Parameters.AddWithValue("@module_id", Request.QueryString["module_id"]);
                        using (SqlDataReader modReader = modCmd.ExecuteReader())
                        {
                            if (modReader.Read())
                            {
                                txtAuthor.Text = modReader["author"].ToString();
                                string status = modReader["status"].ToString();
                                if (!string.IsNullOrEmpty(status) && ddlStatus.Items.FindByValue(status) != null)
                                    ddlStatus.SelectedValue = status;
                            }
                        }
                    }
                    // Load from ModuleInformation for the rest
                    string query = @"SELECT * FROM ModuleInformation WHERE module_id = @module_id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@module_id", Request.QueryString["module_id"]);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtModuleName.Text = reader["module_name"].ToString();
                                txtDescription.Text = reader["description"].ToString();
                                txtIntroduction.Text = reader["introduction"].ToString();
                                txtHeader1.Text = reader["header1"].ToString();
                                txtHeader1Text.Text = reader["header1_text"].ToString();
                                txtHeader2.Text = reader["header2"].ToString();
                                txtHeader2Text.Text = reader["header2_text"].ToString();
                                txtHeader3.Text = reader["header3"].ToString();
                                txtHeader3Text.Text = reader["header3_text"].ToString();
                                txtHeader4.Text = reader["header4"].ToString();
                                txtHeader4Text.Text = reader["header4_text"].ToString();
                                txtHeader5.Text = reader["header5"].ToString();
                                txtHeader5Text.Text = reader["header5_text"].ToString();
                                // Image 1
                                string image1 = reader["image1"].ToString();
                                if (!string.IsNullOrEmpty(image1))
                                {
                                    imgCurrentImage1.ImageUrl = image1;
                                    imgCurrentImage1.Visible = true;
                                }
                                // Image 2
                                string image2 = reader["image2"].ToString();
                                if (!string.IsNullOrEmpty(image2))
                                {
                                    imgCurrentImage2.ImageUrl = image2;
                                    imgCurrentImage2.Visible = true;
                                }
                            }
                        }
                    }
                    // Load cover image
                    string coverQuery = "SELECT cover_image FROM Modules WHERE module_id = @module_id";
                    using (SqlCommand coverCmd = new SqlCommand(coverQuery, conn))
                    {
                        coverCmd.Parameters.AddWithValue("@module_id", Request.QueryString["module_id"]);
                        object coverObj = coverCmd.ExecuteScalar();
                        if (coverObj != null && !string.IsNullOrEmpty(coverObj.ToString()))
                        {
                            imgCurrentCover.ImageUrl = coverObj.ToString();
                            imgCurrentCover.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading module information: " + ex.Message);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["module_id"], out moduleId))
            {
                ShowError("Invalid module ID");
                return;
            }
            if (Page.IsValid)
            {
                try
                {
                    string image1Path = null;
                    string image2Path = null;
                    string coverImagePath = null;
                    string uploadPath = Server.MapPath("~/Uploads/Modules/");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);
                    // Handle cover image upload
                    if (fuCoverImage.HasFile)
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fuCoverImage.FileName;
                        string filePath = Path.Combine(uploadPath, fileName);
                        fuCoverImage.SaveAs(filePath);
                        coverImagePath = "~/Uploads/Modules/" + fileName;
                    }
                    // Handle image1 upload
                    if (fuImage1.HasFile)
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fuImage1.FileName;
                        string filePath = Path.Combine(uploadPath, fileName);
                        fuImage1.SaveAs(filePath);
                        image1Path = "~/Uploads/Modules/" + fileName;
                    }
                    // Handle image2 upload
                    if (fuImage2.HasFile)
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fuImage2.FileName;
                        string filePath = Path.Combine(uploadPath, fileName);
                        fuImage2.SaveAs(filePath);
                        image2Path = "~/Uploads/Modules/" + fileName;
                    }
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        // Update Modules table for author, status, and cover image
                        string modUpdate = @"UPDATE Modules SET author = @author, status = @status {0} WHERE module_id = @module_id";
                        string coverSet = coverImagePath != null ? ", cover_image = @cover_image" : "";
                        modUpdate = string.Format(modUpdate, coverSet);
                        using (SqlCommand modCmd = new SqlCommand(modUpdate, conn))
                        {
                            modCmd.Parameters.AddWithValue("@module_id", moduleId);
                            modCmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                            modCmd.Parameters.AddWithValue("@author", txtAuthor.Text);
                            if (coverImagePath != null) modCmd.Parameters.AddWithValue("@cover_image", coverImagePath);
                            modCmd.ExecuteNonQuery();
                        }
                        // Check if row exists in ModuleInformation
                        string checkQuery = "SELECT COUNT(*) FROM ModuleInformation WHERE module_id = @module_id";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@module_id", moduleId);
                            int count = (int)checkCmd.ExecuteScalar();
                            if (count > 0)
                            {
                                // Update
                                string updateQuery = @"UPDATE ModuleInformation SET
                                    module_name = @module_name,
                                    description = @description,
                                    introduction = @introduction,
                                    header1 = @header1,
                                    header1_text = @header1_text,
                                    header2 = @header2,
                                    header2_text = @header2_text,
                                    header3 = @header3,
                                    header3_text = @header3_text,
                                    header4 = @header4,
                                    header4_text = @header4_text,
                                    header5 = @header5,
                                    header5_text = @header5_text
                                    {0}
                                    WHERE module_id = @module_id";
                                string imageSet = "";
                                if (image1Path != null) imageSet += ", image1 = @image1";
                                if (image2Path != null) imageSet += ", image2 = @image2";
                                updateQuery = string.Format(updateQuery, imageSet);
                                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@module_id", moduleId);
                                    cmd.Parameters.AddWithValue("@module_name", txtModuleName.Text.Trim());
                                    cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                                    cmd.Parameters.AddWithValue("@introduction", txtIntroduction.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header1", txtHeader1.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header1_text", txtHeader1Text.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header2", txtHeader2.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header2_text", txtHeader2Text.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header3", txtHeader3.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header3_text", txtHeader3Text.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header4", txtHeader4.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header4_text", txtHeader4Text.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header5", txtHeader5.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header5_text", txtHeader5Text.Text.Trim());
                                    if (image1Path != null) cmd.Parameters.AddWithValue("@image1", image1Path);
                                    if (image2Path != null) cmd.Parameters.AddWithValue("@image2", image2Path);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insert
                                string insertQuery = @"INSERT INTO ModuleInformation
                                    (module_id, module_name, description, introduction, header1, header1_text, header2, header2_text, header3, header3_text, header4, header4_text, header5, header5_text, image1, image2)
                                    VALUES
                                    (@module_id, @module_name, @description, @introduction, @header1, @header1_text, @header2, @header2_text, @header3, @header3_text, @header4, @header4_text, @header5, @header5_text, @image1, @image2)";
                                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@module_id", moduleId);
                                    cmd.Parameters.AddWithValue("@module_name", txtModuleName.Text.Trim());
                                    cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                                    cmd.Parameters.AddWithValue("@introduction", txtIntroduction.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header1", txtHeader1.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header1_text", txtHeader1Text.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header2", txtHeader2.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header2_text", txtHeader2Text.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header3", txtHeader3.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header3_text", txtHeader3Text.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header4", txtHeader4.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header4_text", txtHeader4Text.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header5", txtHeader5.Text.Trim());
                                    cmd.Parameters.AddWithValue("@header5_text", txtHeader5Text.Text.Trim());
                                    cmd.Parameters.AddWithValue("@image1", (object)image1Path ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@image2", (object)image2Path ?? DBNull.Value);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "updateSuccess", "alert('Module information updated successfully!');", true);
                }
                catch (Exception ex)
                {
                    ShowError("Error updating module information: " + ex.Message);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewModules.aspx");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["module_id"], out moduleId))
            {
                ShowError("Invalid module ID");
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM ModuleInformation WHERE module_id = @module_id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@module_id", moduleId);
                        cmd.ExecuteNonQuery();
                    }
                }
                ShowSuccess("Module information deleted successfully!");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "deleteSuccess", "alert('Module deleted successfully!'); window.location='ViewModules.aspx';", true);
            }
            catch (Exception ex)
            {
                ShowError("Error deleting module information: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            lblSuccess.Visible = false;
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblMessage.Visible = false;
        }
    }
}