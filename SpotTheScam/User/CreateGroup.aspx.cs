using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SpotTheScam.User
{
    public partial class CreateGroup : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // must be logged in
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            int ownerId = Convert.ToInt32(Session["UserId"]);
            string name = (txtName?.Text ?? string.Empty).Trim();
            string desc = (txtDesc?.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowMsg("Group name is required.", false);
                return;
            }

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var tx = con.BeginTransaction();

                try
                {
                    // 1) Create group
                    int groupId;
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO FamilyGroups (Name, [Description], OwnerUserId, CreatedAt)
                        OUTPUT INSERTED.GroupId
                        VALUES (@n, @d, @owner, GETDATE());", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@d", string.IsNullOrWhiteSpace(desc) ? (object)DBNull.Value : desc);
                        cmd.Parameters.AddWithValue("@owner", ownerId);

                        groupId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 2) Ensure creator is a member as GroupOwner (Active)
                    using (var cmd = new SqlCommand(@"
                        IF NOT EXISTS (
                            SELECT 1 FROM FamilyGroupMembers WHERE GroupId = @gid AND UserId = @uid
                        )
                        BEGIN
                            INSERT INTO FamilyGroupMembers
                                (GroupId, UserId, GroupRole, Status, InvitedByUserId, InvitedAt, AcceptedAt)
                            VALUES
                                (@gid, @uid, 'GroupOwner', 'Active', @uid, GETDATE(), GETDATE());
                        END", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@gid", groupId);
                        cmd.Parameters.AddWithValue("@uid", ownerId);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();

                    // Redirect to the group management page with the new id
                    Response.Redirect($"ManageGroup.aspx?groupId={groupId}", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { /* ignore rollback errors */ }
                    ShowMsg("Could not create group. " + ex.Message, false);
                }
            }
        }

        private void ShowMsg(string msg, bool ok)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = ok ? "msg ok" : "msg err";
            lblMsg.Text = msg;
        }
    }
}
