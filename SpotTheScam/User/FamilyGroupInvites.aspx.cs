using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class FamilyGroupInvites : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadInvites();
            }
        }

        private void LoadInvites()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT fg.GroupName, fgm.GroupId
                    FROM FamilyGroupMembers fgm
                    INNER JOIN FamilyGroups fg ON fgm.GroupId = fg.GroupId
                    WHERE fgm.UserId = @UserId AND fgm.IsApproved = 0
                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    rptInvites.DataSource = dt;
                    rptInvites.DataBind();
                    pnlNoInvites.Visible = false;
                }
                else
                {
                    rptInvites.DataSource = null;
                    rptInvites.DataBind();
                    pnlNoInvites.Visible = true;
                }
            }
        }

        protected void rptInvites_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Approve")
            {
                int groupId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string update = @"UPDATE FamilyGroupMembers
                                      SET IsApproved = 1
                                      WHERE GroupId = @GroupId AND UserId = @UserId";
                    SqlCommand cmd = new SqlCommand(update, conn);
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowAlert("You have successfully joined the group!", "success");
                LoadInvites();
            }
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = type == "success"
                ? "alert alert-success"
                : "alert alert-danger";
            AlertMessage.Text = message;
            AlertPanel.Visible = true;
        }
    }
}
