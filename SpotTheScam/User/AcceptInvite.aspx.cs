using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SpotTheScam.User
{
    public partial class AcceptInvite : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // 1) Validate token
            string tokenStr = Request.QueryString["token"];
            if (string.IsNullOrWhiteSpace(tokenStr))
            {
                ShowError("Missing invite token.");
                return;
            }

            Guid token;
            if (!Guid.TryParse(tokenStr, out token))
            {
                ShowError("Invalid invite token.");
                return;
            }

            // 2) Activate invite if still valid; show nice success/error UI
            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var tx = con.BeginTransaction();
                try
                {
                    // Fetch group info for UI (only if token is valid & pending & not expired)
                    int groupId = 0;
                    string groupName = null;

                    using (var look = new SqlCommand(@"
                        SELECT TOP 1 m.GroupId, g.Name
                        FROM FamilyGroupMembers m
                        JOIN FamilyGroups g ON g.GroupId = m.GroupId
                        WHERE m.InvitationToken = @t
                          AND m.Status = 'Pending'
                          AND (m.InvitationExpiresAt IS NULL OR m.InvitationExpiresAt > GETUTCDATE());",
                        con, tx))
                    {
                        look.Parameters.AddWithValue("@t", token);
                        using (var rd = look.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                tx.Rollback();
                                ShowError("This invite is invalid or has expired. Please ask the group owner to resend a new invite.");
                                return;
                            }
                            groupId = rd.GetInt32(0);
                            groupName = rd.GetString(1);
                        }
                    }

                    // Activate: mark Active, stamp AcceptedAt, and clear token/expiry (single-use)
                    int rows;
                    using (var cmd = new SqlCommand(@"
                        UPDATE FamilyGroupMembers
                        SET Status='Active',
                            AcceptedAt=GETUTCDATE(),
                            InvitationToken=NULL,
                            InvitationExpiresAt=NULL
                        WHERE InvitationToken=@t
                          AND Status='Pending'
                          AND (InvitationExpiresAt IS NULL OR InvitationExpiresAt > GETUTCDATE());",
                        con, tx))
                    {
                        cmd.Parameters.AddWithValue("@t", token);
                        rows = cmd.ExecuteNonQuery();
                    }

                    if (rows == 0)
                    {
                        tx.Rollback();
                        ShowError("This invite is invalid or has expired. Please ask the group owner to resend a new invite.");
                        return;
                    }

                    tx.Commit();
                    ShowSuccess(groupId, groupName);
                }
                catch (Exception)
                {
                    try { tx.Rollback(); } catch { }
                    ShowError("Sorry, something went wrong while activating your invite. Please try again.");
                }
            }
        }

        // ---------- UI helpers ----------

        private void ShowSuccess(int groupId, string groupName)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;

            litGroupName.Text = Server.HtmlEncode(groupName ?? "Family Group");
            hlGoGroup.NavigateUrl = $"~/User/ManageGroup.aspx?groupId={groupId}";
            // hlMyGroups already has NavigateUrl in markup
            // hlFamilyHub already has NavigateUrl in markup
        }

        private void ShowError(string message)
        {
            pnlSuccess.Visible = false;
            pnlError.Visible = true;

            litError.Text = Server.HtmlEncode(message ?? "We couldn’t activate your invite.");
        }
    }
}
