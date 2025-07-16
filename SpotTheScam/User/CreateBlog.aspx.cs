using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class CreateBlog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('Please login to proceed!');</script>");
                Response.Redirect("UserLogin.aspx");
            }


        }

        protected void btn_Publish_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] != null && Session["Username"] != null)
            {
                int userId = (int)Session["UserId"];
                string username = Session["Username"].ToString();

                int result = 0;

                string image = "";
                string blog_title = tb_BlogTitle.Text;
                string blog_content = tb_BlogContent.Text;
                DateTime current_time = DateTime.Now;

                string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "INSERT INTO posts(user_id, title, content, cover_image, isApproved, created_at) VALUES (@UserID, @Title, @Content, @Image, 0, @CurrentTime)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Title", blog_title);
                        cmd.Parameters.AddWithValue("@Content", blog_content);
                        cmd.Parameters.AddWithValue("@Image", image);
                        cmd.Parameters.AddWithValue("@CurrentTime", current_time);

                        conn.Open();
                        result += cmd.ExecuteNonQuery();
                        conn.Close();
                    }

                }
                if (result >= 0)
                {
                    string folderPath = Server.MapPath("~/Uploads/Blog_Pictures/");
                    image = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(blog_FileUpload.FileName);
                    string fullPath = folderPath + image;

                    blog_FileUpload.SaveAs(fullPath);
                    Response.Write("<script>alert('You have successfully submitted your blog post. Your blog post will be verified by a staff before being published!');</script>");
                    Response.Redirect("UserHome.aspx");
                }
                else
                {
                    Response.Write("<script>alert('Problem submitting the blog post. Try again!');</script>");
                }
            }
            else
            {
                // If not logged in, redirect to login page
                Response.Redirect("UserLogin.aspx");
            }


        }

        protected void TextBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}