using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User.Controls
{
    public partial class FamilySideNav : UserControl
    {
        // For nav highlighting ("groups" | "create" | "invites" | "connect" | "add" | "logs" | "overview")
        public string ActiveKey { get; set; } = "";

        private readonly string _cs =
            ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGroups();   // populate "Your groups"
                DataBind();     // evaluate <%# ActiveClass(...) %>
            }
        }

        protected string ActiveClass(string key)
        {
            return string.Equals(key, ActiveKey, StringComparison.OrdinalIgnoreCase)
                ? "nav-link active"
                : "nav-link";
        }

        private int CurrentUserId
        {
            get
            {
                var o = Context?.Session?["UserId"];
                return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
            }
        }

        private int? CurrentGroupId
        {
            get
            {
                var o = Context?.Session?["CurrentGroupId"];
                if (o == null || o == DBNull.Value) return null;
                int gid;
                return int.TryParse(o.ToString(), out gid) ? (int?)gid : null;
            }
        }

        private void BindGroups()
        {
            if (CurrentUserId <= 0) return;

            const string sql = @"
SELECT g.[GroupId], g.[Name] AS GroupName
FROM FamilyGroupMembers m
JOIN FamilyGroups g ON g.[GroupId] = m.[GroupId]
WHERE m.[UserId] = @U AND m.[Status] = 'Active'
ORDER BY CASE m.[GroupRole] WHEN 'GroupOwner' THEN 0 WHEN 'Guardian' THEN 1 ELSE 2 END,
         g.[CreatedAt] DESC;";

            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@U", CurrentUserId);

                var dt = new DataTable();
                da.Fill(dt);

                // mark the currently active group
                dt.Columns.Add("IsActive", typeof(bool));
                int? active = CurrentGroupId;
                foreach (DataRow r in dt.Rows)
                    r["IsActive"] = active.HasValue && active.Value == Convert.ToInt32(r["GroupId"]);

                rptGroups.DataSource = dt;
                rptGroups.DataBind();
            }
        }

        protected void lnkSelectGroup_Click(object sender, EventArgs e)
        {
            var btn = sender as LinkButton;
            if (btn == null) return;

            if (!int.TryParse(btn.CommandArgument, out int groupId)) return;

            if (!UserIsInGroup(CurrentUserId, groupId)) return; // guard against bad ids

            Context.Session["CurrentGroupId"] = groupId;

            // After selecting, go to your renamed overview page
            Response.Redirect("~/User/FamilyOverview.aspx", true);
        }

        private bool UserIsInGroup(int userId, int groupId)
        {
            const string sql = @"
SELECT COUNT(*) FROM FamilyGroupMembers
WHERE [GroupId] = @G AND [UserId] = @U AND [Status] = 'Active';";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@G", groupId);
                cmd.Parameters.AddWithValue("@U", userId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}
