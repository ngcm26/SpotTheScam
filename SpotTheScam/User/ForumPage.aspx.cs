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


            if (!IsPostBack && Request.QueryString["success"] == "1")
            {
                lblMessage.Text = "✅ Your post has been created successfully!";
                lblMessage.Visible = true;
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

            // Clear fields
            tb_title.Text = "";
            tb_content.Text = "";
            pnlNewPost.Visible = false;

            // Redirect with success flag
            Response.Redirect(Request.RawUrl + "?success=1", false);
            Context.ApplicationInstance.CompleteRequest();



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
        private string GetTimeAgo(DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return $"{Math.Floor(timeSpan.TotalSeconds)} seconds ago";
            if (timeSpan.TotalMinutes < 60)
                return $"{Math.Floor(timeSpan.TotalMinutes)} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{Math.Floor(timeSpan.TotalHours)} hours ago";
            if (timeSpan.TotalDays < 30)
                return $"{Math.Floor(timeSpan.TotalDays)} days ago";
            if (timeSpan.TotalDays < 365)
                return $"{Math.Floor(timeSpan.TotalDays / 30)} months ago";

            return $"{Math.Floor(timeSpan.TotalDays / 365)} years ago";
        }
        protected void rptDiscussions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblCreatedAt = (Label)e.Item.FindControl("lblCreatedAt");
                if (lblCreatedAt != null)
                {
                    DateTime createdAt = Convert.ToDateTime(DataBinder.Eval(e.Item.DataItem, "CreatedAt"));
                    lblCreatedAt.Text = GetTimeAgo(createdAt);
                }
            }
        }



    }
}