using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class ForumPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Write("<script>alert('Please login to proceed!');</script>");
                Response.Redirect("UserLogin.aspx");
            }
            LoadDiscussions();
        }

        protected void btnToggleForm_Click(object sender, EventArgs e)
        {
            pnlNewPost.Visible = !pnlNewPost.Visible;
        }

        protected void btnSubmitPost_Click(object sender, EventArgs e)
        {
            string title = tb_title.Text;
            string content = tb_content.Text;
            int userId = Convert.ToInt32(Session["UserId"]);
            string imageFileName = "";
            if (img_forum.HasFile)
            {
                string folderPath = Server.MapPath("~/Uploads/forum_pictures/");
                imageFileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(img_forum.FileName);
                string fullPath = folderPath + imageFileName;

                img_forum.SaveAs(fullPath);
            }



            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "INSERT INTO Discussions (UserId, Title, Description, ImagePath) VALUES (@UserId, @Title, @Description, @ImagePath)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", content);
                    cmd.Parameters.AddWithValue("@ImagePath", imageFileName);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            tb_title.Text = "";
            tb_content.Text = "";

            pnlNewPost.Visible = false;
            LoadDiscussions();


        }
        private void LoadDiscussions()
        {
            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = @"
            SELECT d.DiscussionId, d.Title, d.Description, d.CreatedAt, d.ImagePath,
                   u.Username
            FROM Discussions d
            JOIN Users u ON d.UserId = u.Id
            ORDER BY d.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptDiscussions.DataSource = dt;
                    rptDiscussions.DataBind();
                }
            }
        }

    }
}