using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class Invites : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        private int CurrentUserId => Convert.ToInt32(Session["UserId"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }
            if (!IsPostBack) BindInvites();
        }

        private void BindInvites()
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT m.GroupId,
                       g.Name AS GroupName,
                       m.GroupRole AS Role,
                       m.InvitedAt
                FROM FamilyGroupMembers m
                JOIN FamilyGroups g ON g.GroupId = m.GroupId
                WHERE m.UserId=@uid AND m.Status='Pending'
                ORDER BY m.InvitedAt DESC", con))
            {
                cmd.Parameters.AddWithValue("@uid", CurrentUserId);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                gvInvites.DataSource = dt;
                gvInvites.DataBind();
            }
        }

        protected void gvInvites_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            // Optional: disable Accept if invite expired
            // (You can also show expiry column and check InvitationExpiresAt here.)
        }

        protected void gvInvites_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            int groupId = Convert.ToInt32(e.CommandArgument);

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var tx = con.BeginTransaction();

                try
                {
                    if (e.CommandName == "Accept")
                    {
                        using (var cmd = new SqlCommand(@"
                            UPDATE FamilyGroupMembers
                            SET Status='Active', AcceptedAt=GETDATE(), InvitationToken=NULL, InvitationExpiresAt=NULL
                            WHERE GroupId=@gid AND UserId=@uid AND Status='Pending'
                                  AND (InvitationExpiresAt IS NULL OR InvitationExpiresAt>GETDATE());", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@gid", groupId);
                            cmd.Parameters.AddWithValue("@uid", CurrentUserId);
                            int rows = cmd.ExecuteNonQuery();
                            if (rows == 0)
                            {
                                Msg("Invite is invalid or expired.", false, tx);
                                return;
                            }
                        }
                        Msg("Invite accepted.", true, tx);
                    }
                    else if (e.CommandName == "Decline")
                    {
                        using (var cmd = new SqlCommand(@"
                            UPDATE FamilyGroupMembers
                            SET Status='Removed'
                            WHERE GroupId=@gid AND UserId=@uid AND Status='Pending';", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@gid", groupId);
                            cmd.Parameters.AddWithValue("@uid", CurrentUserId);
                            int rows = cmd.ExecuteNonQuery();
                            Msg(rows > 0 ? "Invite declined." : "Nothing to decline.", rows > 0, tx);
                        }
                    }

                    tx.Commit();
                    BindInvites();
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    pnlMsg.Visible = true;
                    pnlMsg.CssClass = "msg err";
                    lblMsg.Text = "Action failed. " + ex.Message;
                }
            }
        }

        private void Msg(string text, bool ok, SqlTransaction tx)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = "msg " + (ok ? "ok" : "err");
            lblMsg.Text = text;
        }
    }
}
