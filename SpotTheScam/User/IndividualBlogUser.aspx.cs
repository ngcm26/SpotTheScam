using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class IndividualBlogUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["postID"]))
            {
                // Show message and redirect if postID is missing
                Session["ErrorMessage"] = "Invalid blog post. Redirected to home.";
                Response.Redirect("UserHome.aspx");  // Change to your actual homepage path
                return;
            }

            int postID;
            // Check if postID is a valid integer
            if (!int.TryParse(Request.QueryString["postID"], out postID))
            {
                Session["ErrorMessage"] = "Invalid blog ID format. Redirected to home.";
                Response.Redirect("UserHome.aspx");
                return;
            }

            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT * FROM posts WHERE post_id = @PostId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", postID);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string title = reader["title"].ToString();
                        string content = reader["content"].ToString();
                        string coverImage = reader["cover_image"].ToString();

                        lbl_title.Text = title;
                        lbl_content.Text = content;
                        img_cover.ImageUrl = "~/Uploads/Blog_Pictures/" + coverImage;
                    }
                    else
                    {
                        Session["ErrorMessage"] = "Blog post not found.";
                        Response.Redirect("UserHome.aspx");
                    }

                    reader.Close();
                }
            }
        }

        protected void btn_back_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserBlog.aspx");
        }
    }
}