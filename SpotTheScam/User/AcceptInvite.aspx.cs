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

            Guid token;
            if (!Guid.TryParse(Request.QueryString["token"], out token))
            {
                Show(false, "Invalid or missing token."); return;
            }

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var tx = con.BeginTransaction();
                try
                {
                    int rows;
                    using (var cmd = new SqlCommand(@"
                        UPDATE FamilyGroupMembers
                        SET Status='Active', AcceptedAt=GETDATE(), InvitationToken=NULL, InvitationExpiresAt=NULL
                        WHERE InvitationToken=@t AND Status='Pending'
                              AND (InvitationExpiresAt IS NULL OR InvitationExpiresAt>GETDATE());", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@t", token);
                        rows = cmd.ExecuteNonQuery();
                    }
                    if (rows == 0) { tx.Rollback(); Show(false, "This invite is invalid or expired."); return; }
                    tx.Commit();
                    Show(true, "Your membership is now active.");
                }
                catch
                {
                    try { tx.Rollback(); } catch { }
                    Show(false, "An error occurred while accepting the invite.");
                }
            }
        }

        private void Show(bool ok, string msg)
        {
            pOk.Visible = ok; pErr.Visible = !ok;
            if (ok) lblOk.Text = msg; else lblErr.Text = msg;
        }
    }
}
