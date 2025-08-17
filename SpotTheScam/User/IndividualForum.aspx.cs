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
    public partial class IndividualForum : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDiscussion();
                LoadComments();
            }
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
                        DateTime createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                        lb_createdAt.Text = GetTimeAgo(createdAt);
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
            int discussionId = int.Parse(Request.QueryString["DiscussionId"]);
            DataTable allComments = GetAllComments(discussionId);

            // Build a hierarchy DataTable with indentation levels
            DataTable threadedComments = BuildHierarchy(allComments, null, 0);

            rptComments.DataSource = threadedComments;
            rptComments.DataBind();

        }
        private DataTable GetAllComments(int discussionId)
        {
            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = @"
            SELECT d.ReplyId, d.ParentReplyId, d.Content, d.CreatedAt, u.Username
            FROM DiscussionReplies d
            JOIN Users u ON d.UserId = u.Id
            WHERE d.DiscussionId = @DiscussionId
            ORDER BY d.CreatedAt ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DiscussionId", discussionId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private DataTable BuildHierarchy(DataTable source, object parentId, int level)
        {
            DataTable result = source.Clone();
            result.Columns.Add("Level", typeof(int)); // for indentation

            DataRow[] rows;
            if (parentId == null)
                rows = source.Select("ParentReplyId IS NULL");
            else
                rows = source.Select("ParentReplyId = " + parentId);

            foreach (DataRow row in rows)
            {
                DataRow newRow = result.NewRow();
                newRow.ItemArray = row.ItemArray; // copy columns from original
                newRow["Level"] = level;
                result.Rows.Add(newRow);

                // Recursively add child replies
                DataTable childRows = BuildHierarchy(source, row["ReplyId"], level + 1);
                foreach (DataRow childRow in childRows.Rows)
                    result.ImportRow(childRow);
            }

            return result;
        }



        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("ForumPage.aspx");
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
        protected void btnReply_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int parentReplyId = int.Parse(btn.CommandArgument);
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            TextBox tb_reply = (TextBox)item.FindControl("tb_reply");

            if (string.IsNullOrWhiteSpace(tb_reply.Text))
                return;

            int discussionId = int.Parse(Request.QueryString["DiscussionId"]);
            int userId = (int)Session["UserId"];

            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = @"INSERT INTO DiscussionReplies (DiscussionId, UserId, Content, ParentReplyId)
                 VALUES (@DiscussionId, @UserId, @Content, @ParentReplyId)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DiscussionId", discussionId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Content", tb_reply.Text.Trim());
                    cmd.Parameters.AddWithValue("@ParentReplyId", parentReplyId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Clear the textbox
            tb_reply.Text = "";

            // Redirect to same page to avoid duplicate post on reload
            Response.Redirect(Request.RawUrl);
        }

        protected void rptComments_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                int replyId = Convert.ToInt32(drv["ReplyId"]);
                Label lblCreatedAt = (Label)e.Item.FindControl("lblCommentCreatedAt");
                if (lblCreatedAt != null)
                {
                    DateTime createdAt = Convert.ToDateTime(drv["CreatedAt"]);
                    lblCreatedAt.Text = GetTimeAgo(createdAt);
                }


                // Find the nested repeater
                Repeater rptReplies = (Repeater)e.Item.FindControl("rptReplies");

                if (rptReplies != null) // <-- add this check
                {
                    string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                    using (SqlConnection conn = new SqlConnection(cs))
                    {
                        string query = @"SELECT dr.ReplyId, dr.Content, dr.CreatedAt, u.Username
                                 FROM DiscussionReplies dr
                                 JOIN Users u ON dr.UserId = u.Id
                                 WHERE dr.ParentReplyId = @ParentReplyId
                                 ORDER BY dr.CreatedAt ASC";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ParentReplyId", replyId);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            rptReplies.DataSource = dt;
                            rptReplies.DataBind();
                            rptReplies.ItemDataBound += rptReplies_ItemDataBound;

                        }
                    }
                }
            }
        }


        protected void rptComments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Reply")
            {
                // You could handle reply logic here if you prefer over btnReply_Click
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            // Register all reply buttons in main and nested repeaters
            RegisterReplyButtons(rptComments);

            // Call base Render after registering
            base.Render(writer);
        }

        private void RegisterReplyButtons(Repeater repeater)
        {
            foreach (RepeaterItem item in repeater.Items)
            {
                Button replyButton = item.FindControl("btnReply") as Button;
                if (replyButton != null)
                {
                    // CommandArgument must be non-null, else use empty string
                    ClientScript.RegisterForEventValidation(replyButton.UniqueID, replyButton.CommandArgument ?? "");
                }

                // Recursively register nested repeaters
                Repeater nestedRepeater = item.FindControl("rptReplies") as Repeater;
                if (nestedRepeater != null)
                {
                    RegisterReplyButtons(nestedRepeater);
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

        protected void rptReplies_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                Label lblCreatedAt = (Label)e.Item.FindControl("lblReplyCreatedAt");
                if (lblCreatedAt != null)
                {
                    DateTime createdAt = Convert.ToDateTime(drv["CreatedAt"]);
                    lblCreatedAt.Text = GetTimeAgo(createdAt);
                }
            }
        }

    }
}