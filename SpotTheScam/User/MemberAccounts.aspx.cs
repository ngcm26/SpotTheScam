using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace SpotTheScam.User
{
    public partial class MemberAccounts : Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        private int GroupId => int.TryParse(Request.QueryString["groupId"], out var g) ? g : 0;
        private int MemberUserId => int.TryParse(Request.QueryString["userId"], out var u) ? u : 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/User/UserLogin.aspx"); return; }
            if (GroupId <= 0 || MemberUserId <= 0) { Show("Missing parameters.", false); return; }

            if (!IsPostBack)
            {
                int viewer = Convert.ToInt32(Session["UserId"]);

                if (!ViewerIsGuardianOrOwner(viewer, GroupId) || !TargetIsActivePrimary(MemberUserId, GroupId))
                { Show("You do not have permission to view this member.", false); return; }

                lblMember.Text = GetUsername(MemberUserId);
                BindAccounts();
            }
        }

        private void BindAccounts()
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT AccountId, BankName, AccountNumberMasked, AccountNickname, Balance
                FROM BankAccounts WHERE UserId=@u ORDER BY AccountNickname", con))
            {
                cmd.Parameters.Add("@u", SqlDbType.Int).Value = MemberUserId;
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                gvAccounts.DataSource = dt;
                gvAccounts.DataBind();
            }
        }

        private string GetUsername(int uid)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT Username FROM Users WHERE Id=@u", con))
            { cmd.Parameters.Add("@u", SqlDbType.Int).Value = uid; con.Open(); return Convert.ToString(cmd.ExecuteScalar()); }
        }

        private bool ViewerIsGuardianOrOwner(int viewerId, int groupId)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT 1 FROM FamilyGroupMembers
                WHERE GroupId=@g AND UserId=@v AND Status='Active' AND GroupRole IN ('Guardian','GroupOwner')", con))
            { cmd.Parameters.Add("@g", SqlDbType.Int).Value = groupId; cmd.Parameters.Add("@v", SqlDbType.Int).Value = viewerId; con.Open(); return cmd.ExecuteScalar() != null; }
        }

        private bool TargetIsActivePrimary(int userId, int groupId)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT 1 FROM FamilyGroupMembers
                WHERE GroupId=@g AND UserId=@u AND Status='Active' AND GroupRole='Primary'", con))
            { cmd.Parameters.Add("@g", SqlDbType.Int).Value = groupId; cmd.Parameters.Add("@u", SqlDbType.Int).Value = userId; con.Open(); return cmd.ExecuteScalar() != null; }
        }

        private void Show(string msg, bool ok)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = "msg " + (ok ? "ok" : "err");
            lblMsg.Text = msg;
        }
    }
}
