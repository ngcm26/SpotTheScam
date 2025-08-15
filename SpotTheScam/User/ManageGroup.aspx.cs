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

        // Cache my role across postbacks
        private string MyRole
        {
            get { return ViewState["MyRole"] as string; }
            set { ViewState["MyRole"] = value; }
        }

        // keep the editing target user between postbacks
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
                int me = Convert.ToInt32(Session["UserId"]);
                if (!UserIsMemberOfGroup(me, GroupId))
                {
                    ShowMsg("You do not have access to this group.", false);
                    gvMembers.Visible = false;
                    return;
                }

                // get my role once and cache it
                MyRole = GetRole(me, GroupId);

                PopulateHourDropdowns();
                BindGroupHeader();
                BindMembers();

                // Owner-only editor and prefill
                pnlOwner.Visible = (MyRole == "GroupOwner");
                if (pnlOwner.Visible)
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
                                txtGroupName.Text = Convert.ToString(rd["Name"]);
                                txtGroupDesc.Text = Convert.ToString(rd["Description"]);
                            }
                        }
                    }
                }
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
            var lnkRemove = (LinkButton)e.Row.FindControl("lnkRemove");
            var pnlRoleEdit = (Panel)e.Row.FindControl("pnlRoleEdit");
            var ddlRoleRow = (DropDownList)e.Row.FindControl("ddlRoleRow");

            // Restrict: only for Primary & Active; actor must be Owner or Guardian
            if (lnkRestrict != null)
                lnkRestrict.Visible = (role == "Primary" && status == "Active" &&
                                       (MyRole == "GroupOwner" || MyRole == "Guardian"));

            // Remove: owner only; cannot remove owner
            if (lnkRemove != null)
                lnkRemove.Visible = (MyRole == "GroupOwner" && role != "GroupOwner");

            // Inline role change: owner only; target not owner
            if (pnlRoleEdit != null)
            {
                pnlRoleEdit.Visible = (MyRole == "GroupOwner" && role != "GroupOwner");
                if (pnlRoleEdit.Visible && ddlRoleRow != null)
                {
                    var li = ddlRoleRow.Items.FindByValue(role);
                    if (li != null) { ddlRoleRow.ClearSelection(); li.Selected = true; }
                }
            }
        }

        protected void gvMembers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            int targetUserId = Convert.ToInt32(e.CommandArgument);
            int actorId = Convert.ToInt32(Session["UserId"]);

            if (e.CommandName == "Restrict")
            {
                LoadRestrictionsFor(targetUserId);
                return;
            }

            if (e.CommandName == "Remove")
            {
                using (var con = new SqlConnection(cs))
                {
                    con.Open();
                    var tx = con.BeginTransaction();
                    try
                    {
                        if (!UserHasRole(actorId, GroupId, "GroupOwner", con, tx))
                        { ShowMsg("Only the Group Owner can remove users.", false); tx.Rollback(); return; }

                        // disallow removing the owner
                        using (var chk = new SqlCommand(@"SELECT GroupRole FROM FamilyGroupMembers WHERE GroupId=@g AND UserId=@u", con, tx))
                        {
                            chk.Parameters.AddWithValue("@g", GroupId);
                            chk.Parameters.AddWithValue("@u", targetUserId);
                            var r = chk.ExecuteScalar() as string;
                            if (r == "GroupOwner") { ShowMsg("You cannot remove the group owner.", false); tx.Rollback(); return; }
                        }

                        using (var cmd = new SqlCommand(@"UPDATE FamilyGroupMembers SET Status='Removed' WHERE GroupId=@g AND UserId=@u;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@g", GroupId);
                            cmd.Parameters.AddWithValue("@u", targetUserId);
                            cmd.ExecuteNonQuery();
                        }

                        // cleanup restrictions/whitelist for that user (optional)
                        using (var cmd = new SqlCommand(@"
                            DELETE FROM FamilyGroupMemberRestrictions WHERE GroupId=@g AND PrimaryUserId=@u;
                            DELETE FROM FamilyGroupMemberWhitelistedRecipients WHERE GroupId=@g AND UserId=@u;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@g", GroupId);
                            cmd.Parameters.AddWithValue("@u", targetUserId);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        ShowMsg("Member removed.", true);
                        BindMembers();
                    }
                    catch (Exception ex) { try { tx.Rollback(); } catch { } ShowMsg("Remove failed. " + ex.Message, false); }
                }
                return;
            }

            if (e.CommandName == "ChangeRole")
            {
                var row = ((Control)e.CommandSource).NamingContainer as GridViewRow;
                var ddl = row?.FindControl("ddlRoleRow") as DropDownList;
                if (ddl == null) { ShowMsg("No role selected.", false); return; }
                string newRole = ddl.SelectedValue;

                using (var con = new SqlConnection(cs))
                {
                    con.Open();
                    var tx = con.BeginTransaction();
                    try
                    {
                        if (!UserHasRole(actorId, GroupId, "GroupOwner", con, tx))
                        { ShowMsg("Only the Group Owner can change roles.", false); tx.Rollback(); return; }

                        using (var chk = new SqlCommand(@"SELECT GroupRole FROM FamilyGroupMembers WHERE GroupId=@g AND UserId=@u", con, tx))
                        {
                            chk.Parameters.AddWithValue("@g", GroupId);
                            chk.Parameters.AddWithValue("@u", targetUserId);
                            var current = (chk.ExecuteScalar() as string) ?? "";
                            if (current == "GroupOwner") { ShowMsg("You cannot change the owner’s role.", false); tx.Rollback(); return; }
                        }

                        using (var cmd = new SqlCommand(@"UPDATE FamilyGroupMembers SET GroupRole=@r WHERE GroupId=@g AND UserId=@u AND Status='Active';", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@g", GroupId);
                            cmd.Parameters.AddWithValue("@u", targetUserId);
                            cmd.Parameters.AddWithValue("@r", newRole);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        ShowMsg("Role updated.", true);
                        BindMembers();
                    }
                    catch (Exception ex) { try { tx.Rollback(); } catch { } ShowMsg("Change role failed. " + ex.Message, false); }
                }
            }
        }

        // --------------- Invite ---------------
        protected void btnInvite_Click(object sender, EventArgs e)
        {
            int inviterId = Convert.ToInt32(Session["UserId"]);
            string email = (txtInviteEmail?.Text ?? "").Trim();
            string role = (ddlInviteRole?.SelectedValue ?? "").Trim();

            // normalize any legacy value
            if (string.Equals(role, "PrimaryAccountHolder", StringComparison.OrdinalIgnoreCase))
                role = "Primary";

            bool okRole = role.Equals("Guardian", StringComparison.OrdinalIgnoreCase) || role.Equals("Primary", StringComparison.OrdinalIgnoreCase);
            if (!okRole)
            {
                ShowMsg($"Invalid role. Got '{role}'. Choose Guardian or Primary.", false);
                return;
            }

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var tx = con.BeginTransaction();

                try
                {
                    if (!UserHasRole(inviterId, GroupId, "GroupOwner", con, tx))
                    {
                        ShowMsg("Only the Group Owner can send invites.", false);
                        tx.Rollback();
                        return;
                    }

                    // find user by email
                    int inviteeId;
                    using (var cmd = new SqlCommand("SELECT Id FROM Users WHERE Email=@em", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@em", email);
                        object r = cmd.ExecuteScalar();
                        if (r == null)
                        {
                            ShowMsg("That email is not registered.", false);
                            tx.Rollback();
                            return;
                        }
                        inviteeId = Convert.ToInt32(r);
                    }

                    if (inviteeId == inviterId)
                    {
                        ShowMsg("You’re already the Group Owner of this group.", false);
                        tx.Rollback();
                        return;
                    }

                    // existing membership?
                    using (var cmd = new SqlCommand(@"
                        SELECT Status FROM FamilyGroupMembers
                        WHERE GroupId=@gid AND UserId=@uid", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@gid", GroupId);
                        cmd.Parameters.AddWithValue("@uid", inviteeId);
                        var status = cmd.ExecuteScalar() as string;
                        if (status == "Active")
                        {
                            ShowMsg("That user is already an active member.", false);
                            tx.Rollback();
                            return;
                        }
                        if (status == "Pending")
                        {
                            ShowMsg("That user already has a pending invite.", false);
                            tx.Rollback();
                            return;
                        }
                    }

                    // insert pending invite
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO FamilyGroupMembers
                          (GroupId, UserId, GroupRole, Status, InvitedByUserId, InvitedAt)
                        VALUES
                          (@gid, @uid, @role, 'Pending', @inviter, GETDATE())", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@gid", GroupId);
                        cmd.Parameters.AddWithValue("@uid", inviteeId);
                        cmd.Parameters.AddWithValue("@role", role);
                        cmd.Parameters.AddWithValue("@inviter", inviterId);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    ShowMsg("Invite sent (now Pending).", true);
                    txtInviteEmail.Text = "";
                    ddlInviteRole.SelectedIndex = 0;
                    BindMembers();
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    ShowMsg("Could not send invite. " + ex.Message, false);
                }
            }
        }

        // --------------- Restrictions UI (Primary only) ---------------
        private void LoadRestrictionsFor(int userId)
        {
            // permission gate: only Owner or Guardian may edit restrictions
            if (!(MyRole == "GroupOwner" || MyRole == "Guardian"))
            {
                ShowMsg("You don’t have permission to edit restrictions.", false);
                return;
            }

            // confirm target is a Primary in this group
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
                    if (role != "Primary")
                    {
                        ShowMsg("Only Primary members can have spending restrictions.", false);
                        return;
                    }
                    lblEditingUser.Text = username;
                    EditingUserId = userId;
                }
            }

            // load existing values (if any) using PrimaryUserId
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT SingleTransactionLimit, MaxDailyTransfer, AllowedStartHour, AllowedEndHour
                FROM FamilyGroupMemberRestrictions
                WHERE GroupId=@gid AND PrimaryUserId=@uid", con))
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

            // permission: Owner or Guardian only
            if (!(MyRole == "GroupOwner" || MyRole == "Guardian"))
            {
                ShowMsg("You don’t have permission to save restrictions.", false);
                return;
            }

            decimal? single = TryParseDecimal(txtSingleLimit.Text);
            decimal? daily = TryParseDecimal(txtDailyLimit.Text);
            int? startHr = TryParseInt(ddlStartHour.SelectedValue);
            int? endHr = TryParseInt(ddlEndHour.SelectedValue);
            int actorId = Convert.ToInt32(Session["UserId"]);

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var tx = con.BeginTransaction();
                try
                {
                    using (var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM FamilyGroupMemberRestrictions WHERE GroupId=@gid AND PrimaryUserId=@uid)
BEGIN
    UPDATE FamilyGroupMemberRestrictions
    SET SingleTransactionLimit=@single,
        MaxDailyTransfer=@daily,
        AllowedStartHour=@sh,
        AllowedEndHour=@eh,
        UpdatedByUserId=@actor,
        UpdatedAt=GETDATE()
    WHERE GroupId=@gid AND PrimaryUserId=@uid;
END
ELSE
BEGIN
    INSERT INTO FamilyGroupMemberRestrictions
        (GroupId, PrimaryUserId, SingleTransactionLimit, MaxDailyTransfer,
         AllowedStartHour, AllowedEndHour, UpdatedByUserId)
    VALUES
        (@gid, @uid, @single, @daily, @sh, @eh, @actor);
END", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@gid", GroupId);
                        cmd.Parameters.AddWithValue("@uid", EditingUserId.Value);
                        cmd.Parameters.AddWithValue("@single", (object)single ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@daily", (object)daily ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@sh", (object)startHr ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@eh", (object)endHr ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@actor", actorId);
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

        // --------------- Owner: rename/description + delete ---------------
        protected void btnSaveGroupMeta_Click(object sender, EventArgs e)
        {
            if (MyRole != "GroupOwner")
            {
                ShowMsg("Only the Group Owner can edit group info.", false);
                return;
            }
            string n = (txtGroupName.Text ?? "").Trim();
            string d = (txtGroupDesc.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(n)) { ShowMsg("Group name cannot be empty.", false); return; }

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("UPDATE FamilyGroups SET Name=@n, [Description]=@d WHERE GroupId=@g AND OwnerUserId=@o", con))
            {
                cmd.Parameters.AddWithValue("@n", n);
                cmd.Parameters.AddWithValue("@d", (object)d ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@g", GroupId);
                cmd.Parameters.AddWithValue("@o", Convert.ToInt32(Session["UserId"]));
                con.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) { ShowMsg("Group updated.", true); BindGroupHeader(); }
                else ShowMsg("Update failed.", false);
            }
        }

        protected void btnDeleteGroup_Click(object sender, EventArgs e)
        {
            if (MyRole != "GroupOwner")
            {
                ShowMsg("Only the Group Owner can delete the group.", false);
                return;
            }

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var tx = con.BeginTransaction();
                try
                {
                    // If whitelist doesn’t have FK cascade, clean it (safe even if FKs are added later)
                    using (var cmd = new SqlCommand(@"DELETE FROM FamilyGroupMemberWhitelistedRecipients WHERE GroupId=@g;", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@g", GroupId);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SqlCommand("DELETE FROM FamilyGroups WHERE GroupId=@g AND OwnerUserId=@o", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@g", GroupId);
                        cmd.Parameters.AddWithValue("@o", Convert.ToInt32(Session["UserId"]));
                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0) { ShowMsg("Delete failed.", false); tx.Rollback(); return; }
                    }

                    tx.Commit();
                    Response.Redirect("~/User/MyGroups.aspx?msg=deleted");
                }
                catch (Exception ex) { try { tx.Rollback(); } catch { } ShowMsg("Delete failed. " + ex.Message, false); }
            }
        }

        // --------------- Helpers ---------------
        private string GetRole(int userId, int groupId)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"SELECT GroupRole FROM FamilyGroupMembers
                                              WHERE GroupId=@g AND UserId=@u AND Status='Active'", con))
            {
                cmd.Parameters.AddWithValue("@g", groupId);
                cmd.Parameters.AddWithValue("@u", userId);
                con.Open();
                var r = cmd.ExecuteScalar();
                return r == null ? "" : r.ToString();
            }
        }

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
    }
}
