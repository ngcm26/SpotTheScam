using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
namespace SpotTheScam.User
{
    public partial class UserMaster : System.Web.UI.MasterPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Username"] != null)
                {
                    phUser.Visible = true;
                    phGuest.Visible = false;
                    lblUsername.Text = Session["Username"].ToString();

                    // Existing bell/invite logic
                    TryUpdateInviteBadgeAndBell();

                    
                }
                else
                {
                    phUser.Visible = false;
                    phGuest.Visible = true;
                }
            }
        }




        private void TryUpdateInviteBadgeAndBell()
        {
            // Controls may not exist on some layouts; fail quietly.
            var litInviteCount = FindControl("litInviteCount") as Literal;
            var litBellCount = FindControl("litBellCount") as Literal;
            var rptInvites = FindControl("rptInvites") as Repeater;
            var phNoInvites = FindControl("phNoInvites") as PlaceHolder;

            if (Session["UserId"] == null)
            {
                if (litInviteCount != null) litInviteCount.Text = "";
                if (litBellCount != null) litBellCount.Text = "";
                if (rptInvites != null) rptInvites.DataSource = null;
                if (phNoInvites != null) phNoInvites.Visible = true;
                return;
            }

            int userId;
            if (!int.TryParse(Session["UserId"].ToString(), out userId))
            {
                if (litInviteCount != null) litInviteCount.Text = "";
                if (litBellCount != null) litBellCount.Text = "";
                if (phNoInvites != null) phNoInvites.Visible = true;
                return;
            }

            try
            {
                var cs = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (var con = new SqlConnection(cs))
                {
                    con.Open();

                    // Count
                    int count;
                    using (var cmd = new SqlCommand(@"
                        SELECT COUNT(*)
                        FROM FamilyGroupMembers
                        WHERE UserId=@uid AND Status='Pending'
                          AND (InvitationExpiresAt IS NULL OR InvitationExpiresAt>GETDATE());", con))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        count = (int)cmd.ExecuteScalar();
                    }

                    if (litInviteCount != null)
                        litInviteCount.Text = count > 0 ? $" <span class='badge-pill'>{count}</span>" : "";
                    if (litBellCount != null)
                        litBellCount.Text = count > 0 ? $" <span class='badge-pill'>{count}</span>" : "";

                    // Top 5 most recent pending invites (for the bell dropdown)
                    if (rptInvites != null)
                    {
                        using (var cmd = new SqlCommand(@"
                            SELECT TOP 5 m.GroupId, g.Name AS GroupName, m.GroupRole AS Role, m.InvitedAt
                            FROM FamilyGroupMembers m
                            JOIN FamilyGroups g ON g.GroupId = m.GroupId
                            WHERE m.UserId=@uid AND m.Status='Pending'
                              AND (m.InvitationExpiresAt IS NULL OR m.InvitationExpiresAt>GETDATE())
                            ORDER BY m.InvitedAt DESC;", con))
                        {
                            cmd.Parameters.AddWithValue("@uid", userId);
                            using (var da = new SqlDataAdapter(cmd))
                            {
                                var dt = new DataTable();
                                da.Fill(dt);
                                rptInvites.DataSource = dt;
                                rptInvites.DataBind();
                                if (phNoInvites != null) phNoInvites.Visible = dt.Rows.Count == 0;
                            }
                        }
                    }
                    else
                    {
                        if (phNoInvites != null) phNoInvites.Visible = count == 0;
                    }
                }
            }
            catch
            {
                // Never break the page due to notification fetch
                if (litInviteCount != null) litInviteCount.Text = "";
                if (litBellCount != null) litBellCount.Text = "";
                if (phNoInvites != null) phNoInvites.Visible = true;
                if (rptInvites != null) { rptInvites.DataSource = null; rptInvites.DataBind(); }
            }
        }
    }
}
