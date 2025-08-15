using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class MyGroups : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected int CurrentUserId => Convert.ToInt32(Session["UserId"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadGroups();
            }
        }

        protected void btnCreateGroup_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("CreateGroup.aspx");
        }

        private void LoadGroups()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT g.GroupId, g.Name, g.Description, m.GroupRole, m.Status
                    FROM FamilyGroups g
                    INNER JOIN FamilyGroupMembers m ON g.GroupId = m.GroupId
                    WHERE m.UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gvGroups.Visible = true;
                    pnlEmpty.Visible = false;
                    gvGroups.DataSource = dt;
                    gvGroups.DataBind();
                }
                else
                {
                    gvGroups.Visible = false;
                    pnlEmpty.Visible = true;
                }
            }
        }

        protected void gvGroups_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Optional styling or logic
            }
        }
    }
}
