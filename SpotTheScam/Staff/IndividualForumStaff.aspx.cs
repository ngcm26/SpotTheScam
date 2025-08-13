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
    public partial class IndividualForum : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDiscussion();
            LoadComments();
        }
        private void LoadDiscussion()
        {
            int DiscussionId = int.Parse(Request.QueryString["DiscussionId"].ToString());
            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT d.Title, d.Description, d.CreatedAt, d.ImagePath, u.Username FROM Discussions d JOIN Users u ON d.UserId = u.Id WHERE DiscussionId = @DiscussionID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DiscussionID", DiscussionId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lb_username.Text = reader["Username"].ToString();
                        lb_title.Text = reader["Title"].ToString();
                        lb_createdAt.Text = reader["CreatedAt"].ToString();
                        lb_description.Text = reader["Description"].ToString();
                        img_forum.ImageUrl = "~/Uploads/forum_pictures/" + reader["ImagePath"].ToString();


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

        private void LoadComments()
        {
            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = @"SELECT d.Content, d.CreatedAt, u.Username
                         FROM DiscussionReplies d
                         JOIN Users u ON d.UserId = u.Id
                         WHERE d.DiscussionId = @DiscussionId
                         ORDER BY d.CreatedAt ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DiscussionId", Request.QueryString["DiscussionId"]);
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptComments.DataSource = dt;
                    rptComments.DataBind();
                }
            }
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageForum.aspx");
        }

        protected void btnSubmitComment_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("UserLogin.aspx");
                return;
            }

            int discussionId = int.Parse(Request.QueryString["DiscussionId"]);
            int userId = (int)Session["UserId"];
            string content = tb_comment.Text.Trim();

            if (string.IsNullOrEmpty(content))
            {
                lblMessage.Text = "Comment cannot be empty.";
                return;
            }

            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = @"INSERT INTO DiscussionReplies (DiscussionId, UserId, Content) 
                         VALUES (@DiscussionId, @UserId, @Content)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DiscussionId", discussionId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Content", content);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            tb_comment.Text = ""; // Clear textbox
            LoadComments(); // Refresh comments
        }
    }
}