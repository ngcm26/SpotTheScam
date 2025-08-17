using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.Staff
{
    public partial class ManageForum : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bind();
            }
        }
        private void bind()
        {
            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {

                string query = @"
            SELECT d.DiscussionId, d.Title, d.Description, d.CreatedAt, u.Username
            FROM Discussions d
            JOIN Users u ON d.UserId = u.Id
            ORDER BY d.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    gv_forum.DataSource = reader;
                    gv_forum.DataBind();
                }
            }
        }


        protected void gv_forum_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            int DiscussionId = Convert.ToInt32(gv_forum.DataKeys[e.RowIndex].Value);

            string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string deleteRepliesSql = "DELETE FROM DiscussionReplies WHERE DiscussionId = @DiscussionId";
                using (SqlCommand cmd = new SqlCommand(deleteRepliesSql, conn))
                {
                    cmd.Parameters.AddWithValue("@DiscussionId", DiscussionId);
                    cmd.ExecuteNonQuery();
                }

                string sql = "DELETE FROM Discussions WHERE DiscussionId = @DiscussionId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DiscussionId", DiscussionId);
                    cmd.ExecuteNonQuery();

                }
                conn.Close();
            }

            bind();
        }

        protected void gv_forum_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gv_forum.SelectedRow;
            int DiscussionId = int.Parse(row.Cells[0].Text);

            Response.Redirect("IndividualForumStaff.aspx?DiscussionId=" + DiscussionId);
        }

        protected void gv_forum_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gv_forum.EditIndex = e.NewEditIndex;
            bind(); 
        }

        protected void gv_forum_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gv_forum.EditIndex = -1;
            bind(); 
        }

        protected void gv_forum_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int DiscussionId = Convert.ToInt32(gv_forum.DataKeys[e.RowIndex].Value);

            GridViewRow row = gv_forum.Rows[e.RowIndex];

            string newTitle = ((TextBox)row.Cells[2].Controls[0]).Text;
            string newDescription = ((TextBox)row.Cells[3].Controls[0]).Text;

            string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Discussions SET Title = @Title, Description = @Description WHERE DiscussionId = @DiscussionId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", newTitle);
                    cmd.Parameters.AddWithValue("@Description", newDescription);
                    cmd.Parameters.AddWithValue("@DiscussionId", DiscussionId);

                    cmd.ExecuteNonQuery();
                }
            }

            gv_forum.EditIndex = -1; 
            bind(); 
        }

    }
}