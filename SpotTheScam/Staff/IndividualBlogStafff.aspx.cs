using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.Staff
{
    public partial class IndividualBlogStafff : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int postID = int.Parse(Request.QueryString["postID"].ToString());
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

                        // Now you can use title, content, coverImage
                        lbl_title.Text = title;
                        lbl_content.Text = content;
                        img_cover.ImageUrl = "~/Uploads/Blog_Pictures/" + coverImage;
                    }
                    else
                    {
                        lblMessage.Text = "Post not found.";
                    }
                    reader.Close();
                    conn.Close();
                }

            }

        }
    }
}