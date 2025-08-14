using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class ManageGroup : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        private int GroupId
        {
            get
            {
                int gid;
                return int.TryParse(Request.QueryString["groupId"], out gid) ? gid : 0;
            }
        }

        // we keep the editing target user here between postbacks
        private int? EditingUserId
        {
            get { return ViewState["EditingUserId"] as int?; }
            set { ViewState["EditingUserId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            if (GroupId <= 0)
            {
                ShowMsg("Missing groupId.", false);
                return;
            }

            if (!IsPostBack)
            {
                if (!UserIsMemberOfGroup(Convert.ToInt32(Session["UserId"]), GroupId))
                {
                    ShowMsg("You do not have access to this group.", false);
                    gvMembers.Visible = false;
                    return;
                }

                EnsureRestrictionsTableExists();
                PopulateHourDropdowns();
                BindGroupHeader();
                BindMembers();
            }
        }

        // ---------------- Header ----------------
        private void BindGroupHeader()
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT Name, [Description] FROM FamilyGroups WHERE GroupId=@gid", con))
            {
                cmd.Parameters.AddWithValue("@gid", GroupId);
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        hGroup.InnerText = Convert.ToString(rd["Name"]);
                        pDesc.InnerText = Convert.ToString(rd["Description"]);
                    }
                    else
                    {
                        ShowMsg("Group not found.", false);
                    }
                }
            }
        }

        // --------------- Members ---------------
        private void BindMembers()
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT u.Id AS UserId, u.Username, u.Email,
                       m.GroupRole, m.Status, m.InvitedAt, m.AcceptedAt
                FROM FamilyGroupMembers m
                JOIN Users u ON u.Id = m.UserId
                WHERE m.GroupId=@gid
                ORDER BY 
                  CASE m.GroupRole WHEN 'GroupOwner' THEN 0 WHEN 'Guardian' THEN 1 ELSE 2 END,
                  u.Username", con))
            {
                cmd.Parameters.AddWithValue("@gid", GroupId);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                gvMembers.DataSource = dt;
                gvMembers.DataBind();
            }
        }

        protected void gvMembers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var drv = (DataRowView)e.Row.DataItem;

            string role = Convert.ToString(drv["GroupRole"]);
            string status = Convert.ToString(drv["Status"]);

            var lnkRestrict = (LinkButton)e.Row.FindControl("lnkRestrict");
            var lnkActivate = (LinkButton)e.Row.FindControl("lnkActivate");

            // show Restrict only for PrimaryAccountHolder and when Active
            if (lnkRestrict != null)
                lnkRestrict.Visible = (role == "PrimaryAccountHolder" && status == "Active");

            // show Activate only when Pending
            if (lnkActivate != null)
                lnkActivate.Visible = (status == "Pending");
        }

        protected void gvMembers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            int targetUserId = Convert.ToInt32(e.CommandArgument);
            int actorId = Convert.ToInt32(Session["UserId"]);

            if (e.CommandName == "Activate")
            {
                using (var con = new SqlConnection(cs))
                {
                    con.Open();
                    var tx = con.BeginTransaction();
                    try
                    {
                        if (!UserHasRole(actorId, GroupId, "GroupOwner", con, tx))
                        {
                            ShowMsg("Only the Group Owner can activate members.", false);
                            tx.Rollback();
                            return;
                        }

                        using (var cmd = new SqlCommand(@"
                            UPDATE FamilyGroupMembers
                            SET Status='Active', AcceptedAt=GETDATE()
                            WHERE GroupId=@gid AND UserId=@uid AND Status='Pending';", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@gid", GroupId);
                            cmd.Parameters.AddWithValue("@uid", targetUserId);
                            int rows = cmd.ExecuteNonQuery();
                            ShowMsg(rows > 0 ? "Member activated." : "Nothing to activate.", rows > 0);
                        }

                        tx.Commit();
                        BindMembers();
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        ShowMsg("Action failed. " + ex.Message, false);
                    }
                }
            }
            else if (e.CommandName == "Restrict")
            {
                // open editor
                LoadRestrictionsFor(targetUserId);
            }
        }

        // --------------- Invite ---------------
        protected void btnInvite_Click(object sender, EventArgs e)
        {
            // === HARD DIAG: what file & DLL are running? ===
            var asm = typeof(SpotTheScam.User.ManageGroup).Assembly;
            var built = System.IO.File.GetLastWriteTime(asm.Location);
            var path = Server.MapPath(Request.CurrentExecutionFilePath);

            ShowMsg($"DBG → page={path} | asm={System.IO.Path.GetFileName(asm.Location)} | built={built:yyyy-MM-dd HH:mm:ss}", false);
            return; // TEMP: stop here for this run
        }


        // --------------- Restrictions UI ---------------
        private void LoadRestrictionsFor(int userId)
        {
            // only for PrimaryAccountHolder
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT u.Username, m.GroupRole
                FROM FamilyGroupMembers m
                JOIN Users u ON u.Id = m.UserId
                WHERE m.GroupId=@gid AND m.UserId=@uid", con))
            {
                cmd.Parameters.AddWithValue("@gid", GroupId);
                cmd.Parameters.AddWithValue("@uid", userId);
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                    {
                        ShowMsg("Member not found.", false);
                        return;
                    }
                    var role = Convert.ToString(rd["GroupRole"]);
                    var username = Convert.ToString(rd["Username"]);
                    if (role != "PrimaryAccountHolder")
                    {
                        ShowMsg("Only Primary Account Holders can have spending restrictions.", false);
                        return;
                    }
                    lblEditingUser.Text = username;
                    EditingUserId = userId;
                }
            }

            // load existing values (if any)
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT SingleTransactionLimit, MaxDailyTransfer, AllowedStartHour, AllowedEndHour
                FROM FamilyGroupMemberRestrictions
                WHERE GroupId=@gid AND UserId=@uid", con))
            {
                cmd.Parameters.AddWithValue("@gid", GroupId);
                cmd.Parameters.AddWithValue("@uid", userId);
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtSingleLimit.Text = rd["SingleTransactionLimit"] == DBNull.Value ? "" : Convert.ToDecimal(rd["SingleTransactionLimit"]).ToString("0.##");
                        txtDailyLimit.Text = rd["MaxDailyTransfer"] == DBNull.Value ? "" : Convert.ToDecimal(rd["MaxDailyTransfer"]).ToString("0.##");
                        SelectHour(ddlStartHour, rd["AllowedStartHour"]);
                        SelectHour(ddlEndHour, rd["AllowedEndHour"]);
                    }
                    else
                    {
                        txtSingleLimit.Text = "";
                        txtDailyLimit.Text = "";
                        ddlStartHour.SelectedIndex = 0;
                        ddlEndHour.SelectedIndex = 0;
                    }
                }
            }

            pnlRestrict.Visible = true;
        }

        protected void btnSaveRestrictions_Click(object sender, EventArgs e)
        {
            if (!EditingUserId.HasValue)
            {
                ShowMsg("No member selected.", false);
                return;
            }

            decimal? single = TryParseDecimal(txtSingleLimit.Text);
            decimal? daily = TryParseDecimal(txtDailyLimit.Text);
            int? startHr = TryParseInt(ddlStartHour.SelectedValue);
            int? endHr = TryParseInt(ddlEndHour.SelectedValue);

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var tx = con.BeginTransaction();
                try
                {
                    // upsert
                    using (var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM FamilyGroupMemberRestrictions WHERE GroupId=@gid AND UserId=@uid)
    UPDATE FamilyGroupMemberRestrictions
    SET SingleTransactionLimit=@single, MaxDailyTransfer=@daily,
        AllowedStartHour=@sh, AllowedEndHour=@eh
    WHERE GroupId=@gid AND UserId=@uid;
ELSE
    INSERT INTO FamilyGroupMemberRestrictions
        (GroupId, UserId, SingleTransactionLimit, MaxDailyTransfer, AllowedStartHour, AllowedEndHour)
    VALUES (@gid, @uid, @single, @daily, @sh, @eh);", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@gid", GroupId);
                        cmd.Parameters.AddWithValue("@uid", EditingUserId.Value);
                        cmd.Parameters.AddWithValue("@single", (object)single ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@daily", (object)daily ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@sh", (object)startHr ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@eh", (object)endHr ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    ShowMsg("Restrictions saved.", true);
                    pnlRestrict.Visible = false;
                    EditingUserId = null;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    ShowMsg("Could not save restrictions. " + ex.Message, false);
                }
            }
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlRestrict.Visible = false;
            EditingUserId = null;
        }

        // --------------- Helpers ---------------
        private bool UserIsMemberOfGroup(int userId, int groupId)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT 1 FROM FamilyGroupMembers
                WHERE GroupId=@gid AND UserId=@uid AND Status IN ('Active','Pending')", con))
            {
                cmd.Parameters.AddWithValue("@gid", groupId);
                cmd.Parameters.AddWithValue("@uid", userId);
                con.Open();
                return cmd.ExecuteScalar() != null;
            }
        }

        private bool UserHasRole(int userId, int groupId, string role, SqlConnection con, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand(@"
                SELECT 1 FROM FamilyGroupMembers
                WHERE GroupId=@gid AND UserId=@uid AND GroupRole=@r AND Status='Active'", con, tx))
            {
                cmd.Parameters.AddWithValue("@gid", groupId);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@r", role);
                return cmd.ExecuteScalar() != null;
            }
        }

        private void ShowMsg(string msg, bool ok)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = "msg " + (ok ? "ok" : "err");
            lblMsg.Text = msg;
        }

        private void PopulateHourDropdowns()
        {
            if (ddlStartHour.Items.Count > 0) return; // already done

            for (int h = 0; h < 24; h++)
            {
                string label = DateTime.Today.AddHours(h).ToString("h:00 tt");
                ddlStartHour.Items.Add(new ListItem(label, h.ToString()));
                ddlEndHour.Items.Add(new ListItem(label, h.ToString()));
            }
        }

        private void SelectHour(DropDownList ddl, object val)
        {
            if (val == null || val == DBNull.Value) { ddl.ClearSelection(); return; }
            var v = Convert.ToInt32(val).ToString();
            var li = ddl.Items.FindByValue(v);
            if (li != null) { ddl.ClearSelection(); li.Selected = true; }
        }

        private decimal? TryParseDecimal(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            decimal d; return decimal.TryParse(s, out d) ? d : (decimal?)null;
        }

        private int? TryParseInt(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            int i; return int.TryParse(s, out i) ? i : (int?)null;
        }

        // creates the restrictions table if it doesn't exist yet
        private void EnsureRestrictionsTableExists()
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FamilyGroupMemberRestrictions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FamilyGroupMemberRestrictions](
        [GroupId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [SingleTransactionLimit] DECIMAL(18,2) NULL,
        [MaxDailyTransfer] DECIMAL(18,2) NULL,
        [AllowedStartHour] INT NULL,
        [AllowedEndHour] INT NULL,
        CONSTRAINT [PK_FGMR] PRIMARY KEY CLUSTERED ([GroupId],[UserId]),
        CONSTRAINT [FK_FGMR_Group] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[FamilyGroups]([GroupId]) ON DELETE CASCADE,
        CONSTRAINT [FK_FGMR_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
    );
END", con))
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
