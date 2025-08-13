using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace SpotTheScam.Staff
{
    public partial class ManageBlog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bind();
            }
        }

        protected void gv_blog_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gv_blog.SelectedRow;
            int postID = int.Parse(row.Cells[0].Text);

            Response.Redirect("IndividualBlogStafff.aspx?postID=" + postID);


        }

        protected void gv_blog_DataBinding(object sender, EventArgs e)
        {

        }
        protected void bind()
        {
            string filter = ddlFilter.SelectedValue;
            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT * FROM posts";

                if (filter == "approved")
                {
                    query += " WHERE isApproved = 1";
                }
                else if (filter == "unapproved")
                {
                    query += " WHERE isApproved = 0";
                }

                query += " ORDER BY created_at DESC"; // optional sorting

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    gv_blog.DataSource = reader;
                    gv_blog.DataBind();
                }
            }
        }

        protected void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            bind();
        }


        protected void gv_blog_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int postId = Convert.ToInt32(gv_blog.DataKeys[e.RowIndex].Value);

            string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM posts WHERE post_id = @PostId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", postId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            // Rebind the GridView to show updated data
            bind();
        }
        private void ApprovePost(int postId)
        {
            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string updateQuery = "UPDATE posts SET isApproved = 1 WHERE post_id = @PostId";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@PostId", postId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void gv_blog_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Approve")
            {
                int post_id = Convert.ToInt32(e.CommandArgument);

                ApprovePost(post_id);
                bind(); // Refresh list
            }
        }
    }
}