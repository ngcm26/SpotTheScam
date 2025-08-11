using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SpotTheScam.User
{
    public partial class FamilySidebar : System.Web.UI.UserControl
    {
        protected int PendingInvitesCount = 0;
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGroupsSidebar();
                CountPendingInvites();
            }
        }

        private void BindGroupsSidebar()
        {
            if (Session["UserId"] == null) return;
            int userId = Convert.ToInt32(Session["UserId"]);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT fg.GroupId, fg.GroupName, fg.DateCreated
            FROM FamilyGroups fg
            INNER JOIN FamilyGroupMembers fgm ON fg.GroupId = fgm.GroupId
            WHERE fgm.UserId = @UserId
        ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);

                // Add CurrentGroupId column for active styling
                dt.Columns.Add("CurrentGroupId", typeof(int));
                int currentGroupId = Session["CurrentGroupId"] != null ? Convert.ToInt32(Session["CurrentGroupId"]) : -1;
                foreach (DataRow row in dt.Rows)
                    row["CurrentGroupId"] = currentGroupId;

                rptGroups.DataSource = dt;
                rptGroups.DataBind();
            }
        }


        protected void btnCreateGroupSidebar_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) return;
            string groupName = txtGroupNameSidebar.Text.Trim();
            if (string.IsNullOrEmpty(groupName)) return;

            int userId = Convert.ToInt32(Session["UserId"]);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var tran = conn.BeginTransaction();
                try
                {
                    string insertGroup = "INSERT INTO FamilyGroups (GroupName, CreatedByUserId, DateCreated) OUTPUT INSERTED.GroupId VALUES (@GroupName, @CreatedByUserId, GETDATE())";
                    SqlCommand cmdGroup = new SqlCommand(insertGroup, conn, tran);
                    cmdGroup.Parameters.AddWithValue("@GroupName", groupName);
                    cmdGroup.Parameters.AddWithValue("@CreatedByUserId", userId);
                    int groupId = (int)cmdGroup.ExecuteScalar();

                    string insertMember = "INSERT INTO FamilyGroupMembers (GroupId, UserId, Role, IsApproved) VALUES (@GroupId, @UserId, @Role, 1)";
                    SqlCommand cmdMember = new SqlCommand(insertMember, conn, tran);
                    cmdMember.Parameters.AddWithValue("@GroupId", groupId);
                    cmdMember.Parameters.AddWithValue("@UserId", userId);
                    cmdMember.Parameters.AddWithValue("@Role", "admin");
                    cmdMember.ExecuteNonQuery();

                    tran.Commit();
                    txtGroupNameSidebar.Text = ""; // clear field
                }
                catch
                {
                    tran.Rollback();
                }
            }
            BindGroupsSidebar(); // Refresh sidebar after creation
        }

        protected void lnkSelectGroup_Click(object sender, EventArgs e)
        {
            var lnk = sender as System.Web.UI.WebControls.LinkButton;
            if (lnk == null) return;
            int groupId;
            if (!int.TryParse(lnk.CommandArgument, out groupId)) return;
            // Save to session for main page to pick up
            Session["CurrentGroupId"] = groupId;
            // Optionally, reload the main page to reflect new selection
            Response.Redirect("FamilyGroupManagement.aspx");
        }

        private void CountPendingInvites()
        {
            if (Session["UserId"] == null) return;
            int userId = Convert.ToInt32(Session["UserId"]);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM FamilyGroupMembers WHERE UserId = @UserId AND IsApproved = 0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                PendingInvitesCount = (int)cmd.ExecuteScalar();
            }
        }
    }
}
