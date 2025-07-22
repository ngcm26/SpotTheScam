using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class UserBlog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bind();
            }
        }

        protected void rptBlog_ItemCommand(object source, RepeaterCommandEventArgs e)
        {


        }
        protected void bind()
        {
            string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT p.*, u.username FROM posts p INNER JOIN users u ON p.user_id = u.id WHERE p.isApproved = 0 ORDER BY p.created_at DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptBlog.DataSource = dt;
                    rptBlog.DataBind();
                }

            }
        }

        protected void btn_CreateBlog_Click(object sender, EventArgs e)
        {
            Response.Redirect("CreateBlog.aspx");
        }

        protected void rptBlog_ItemCommand1(object source, RepeaterCommandEventArgs e)
        {

        }
    }
}