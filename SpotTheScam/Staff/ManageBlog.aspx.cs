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
            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT * FROM posts";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gv_blog.DataSource = dt;   // 👈 Bind to GridView
                    gv_blog.DataBind();
                }

            }
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
    }
}