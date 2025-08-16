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

        // REPLACE the old GroupId getter that read from Request.QueryString
        private int GroupId
        {
            get { return (int)(ViewState["GroupId"] ?? 0); }
            set { ViewState["GroupId"] = value; }
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
            if (Session["UserId"] == null) { Response.Redirect("~/User/UserLogin.aspx"); return; }

            var resolved = ResolveGroupId();
            if (resolved == null)                 // ← check the resolved value, not GroupId
            {
                Response.Redirect("~/User/MyGroups.aspx");
                return;                           // (or do a redirect to MyGroups)
            }

            GroupId = resolved.Value;             // ← now persist it
            Session["CurrentGroupId"] = GroupId;  // keep site-wide in sync


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

                // hide invite UI for non-owners
                pnlInvite.Visible = (MyRole == "GroupOwner");

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
                    AND m.Status IN ('Active', 'Pending')    
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

            var hlView = (HyperLink)e.Row.FindControl("hlView");
            if (hlView != null)
            {
                bool canView = (MyRole == "GroupOwner" || MyRole == "Guardian")
                               && role == "Primary" && status == "Active";
                hlView.Visible = canView;
                if (canView)
                {
                    int uid = Convert.ToInt32(drv["UserId"]);
                    hlView.NavigateUrl = $"~/User/MemberAccounts.aspx?groupId={GroupId}&userId={uid}";
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
                        // disallow removing the owner
                        using (var chk = new SqlCommand(
                            "SELECT GroupRole, Status FROM FamilyGroupMembers WHERE GroupId=@g AND UserId=@u",
                            con, tx))
                        {
                            chk.Parameters.AddWithValue("@g", GroupId);
                            chk.Parameters.AddWithValue("@u", targetUserId);
                            using (var rd = chk.ExecuteReader())
                            {
                                if (!rd.Read()) { ShowMsg("Member not found.", false); tx.Rollback(); return; }
                                var role = rd["GroupRole"] as string;
                                var status = rd["Status"] as string;
                                if (role == "GroupOwner") { ShowMsg("You cannot remove the group owner.", false); tx.Rollback(); return; }

                                rd.Close();

                                if (status == "Pending")
                                {
                                    // cancel invite
                                    using (var del = new SqlCommand(
                                        "DELETE FROM FamilyGroupMembers WHERE GroupId=@g AND UserId=@u", con, tx))
                                    { del.Parameters.AddWithValue("@g", GroupId); del.Parameters.AddWithValue("@u", targetUserId); del.ExecuteNonQuery(); }
                                }
                                else
                                {
                                    // mark as removed
                                    using (var upd = new SqlCommand(
                                        "UPDATE FamilyGroupMembers SET Status='Removed' WHERE GroupId=@g AND UserId=@u",
                                        con, tx))
                                    { upd.Parameters.AddWithValue("@g", GroupId); upd.Parameters.AddWithValue("@u", targetUserId); upd.ExecuteNonQuery(); }

                                    // optional cleanup (safe even if no rows)
                                    using (var cleanup = new SqlCommand(@"
                                        DELETE FROM FamilyGroupMemberRestrictions WHERE GroupId=@g AND PrimaryUserId=@u;
                                        DELETE FROM FamilyGroupMemberWhitelistedRecipients WHERE GroupId=@g AND UserId=@u;", con, tx))
                                    { cleanup.Parameters.AddWithValue("@g", GroupId); cleanup.Parameters.AddWithValue("@u", targetUserId); cleanup.ExecuteNonQuery(); }
                                }
                            }
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

            bool okRole = role.Equals("Guardian", StringComparison.OrdinalIgnoreCase)
                       || role.Equals("Primary", StringComparison.OrdinalIgnoreCase);
            if (!okRole)
            {
                ShowMsg($"Invalid role. Got '{role}'. Choose Guardian or Primary.", false);
                return;
            }

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                var tx = con.BeginTransaction();

                Guid token = Guid.Empty;
                DateTime expiresUtc = DateTime.UtcNow.AddDays(7);
                int inviteeId = 0;
                string groupName = "";

                try
                {
                    if (!UserHasRole(inviterId, GroupId, "GroupOwner", con, tx))
                    {
                        ShowMsg("Only the Group Owner can send invites.", false);
                        tx.Rollback();
                        return;
                    }

                    // find user by email
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
                    string existingStatus = null;
                    using (var cmd = new SqlCommand(@"
                SELECT Status FROM FamilyGroupMembers
                WHERE GroupId=@gid AND UserId=@uid", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@gid", GroupId);
                        cmd.Parameters.AddWithValue("@uid", inviteeId);
                        var obj = cmd.ExecuteScalar();
                        existingStatus = obj as string;
                    }

                    if (string.Equals(existingStatus, "Active", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowMsg("That user is already an active member.", false);
                        tx.Rollback();
                        return;
                    }

                    if (string.Equals(existingStatus, "Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowMsg("That user already has a pending invite.", false);
                        tx.Rollback();
                        return;
                    }

                    // Upsert Pending row
                    if (string.Equals(existingStatus, "Removed", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var up = new SqlCommand(@"
                    UPDATE FamilyGroupMembers
                    SET GroupRole=@role,
                        Status='Pending',
                        InvitedByUserId=@inviter,
                        InvitedAt=GETDATE(),
                        AcceptedAt=NULL,
                        InvitationToken=NULL,
                        InvitationExpiresAt=NULL
                    WHERE GroupId=@gid AND UserId=@uid;", con, tx))
                        {
                            up.Parameters.AddWithValue("@gid", GroupId);
                            up.Parameters.AddWithValue("@uid", inviteeId);
                            up.Parameters.AddWithValue("@role", role);
                            up.Parameters.AddWithValue("@inviter", inviterId);
                            up.ExecuteNonQuery();
                        }
                    }
                    else
                    {
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
                    }

                    // Token + expiry
                    token = Guid.NewGuid();
                    using (var up2 = new SqlCommand(@"
                UPDATE FamilyGroupMembers
                  SET InvitationToken=@t, InvitationExpiresAt=@exp
                WHERE GroupId=@gid AND UserId=@uid;", con, tx))
                    {
                        up2.Parameters.AddWithValue("@gid", GroupId);
                        up2.Parameters.AddWithValue("@uid", inviteeId);
                        up2.Parameters.AddWithValue("@t", token);
                        up2.Parameters.AddWithValue("@exp", expiresUtc);
                        up2.ExecuteNonQuery();
                    }

                    // group name for email (reuse same connection/tx)
                    groupName = GetGroupName(GroupId, con, tx);

                    tx.Commit();
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    ShowMsg("Could not send invite. " + ex.Message, false);
                    return;
                }

                // Send email OUTSIDE the transaction (so DB is not held open by SMTP)
                try
                {
                    string acceptUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}{ResolveUrl("~/User/AcceptInvite.aspx")}?token={token}";
                    SendInviteEmail(email, groupName, acceptUrl, expiresUtc);

                    ShowMsg("Invite sent (now Pending).", true);
                    txtInviteEmail.Text = "";
                    ddlInviteRole.SelectedIndex = 0;
                    BindMembers();
                }
                catch (Exception mailEx)
                {
                    // Email failed but DB invite exists; surface a clear message
                    ShowMsg("Invite created, but email delivery failed: " + mailEx.Message, false);
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

        private string GetGroupName(int groupId, SqlConnection con, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand("SELECT Name FROM FamilyGroups WHERE GroupId=@g", con, tx))
            {
                cmd.Parameters.AddWithValue("@g", groupId);
                var r = cmd.ExecuteScalar();
                return r == null ? "Family Group" : r.ToString();
            }
        }

        private void SendInviteEmail(string toEmail, string groupName, string acceptUrl, DateTime expiresUtc)
        {
            var msg = new System.Net.Mail.MailMessage();
            msg.To.Add(toEmail);
            msg.Subject = $"You're invited to join {groupName}";
            msg.IsBodyHtml = true;

            // Convert expiry to SGT for display
            DateTime expiresLocal;
            try
            {
                var sgt = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                expiresLocal = TimeZoneInfo.ConvertTimeFromUtc(expiresUtc, sgt);
            }
            catch
            {
                // fallback if TZ not found on host
                expiresLocal = expiresUtc.AddHours(8);
            }

            msg.Body =
                $"<p>Hello,</p>" +
                $"<p>You’ve been invited to join <strong>{System.Web.HttpUtility.HtmlEncode(groupName)}</strong>.</p>" +
                $"<p>Click the link below to accept:</p>" +
                $"<p><a href=\"{acceptUrl}\">{acceptUrl}</a></p>" +
                $"<p>This link expires on <strong>{expiresLocal:ddd, dd MMM yyyy HH:mm}</strong>.</p>" +
                $"<p>If you didn’t expect this, you can ignore this email.</p>";

            using (var client = new System.Net.Mail.SmtpClient()) // uses <system.net><mailSettings>
            {
                client.Send(msg);
            }
        }

        private int? ResolveGroupId()
        {
            int gid;

            // 1) Try query string
            var qs = Request.QueryString["groupId"];
            if (!string.IsNullOrWhiteSpace(qs) && int.TryParse(qs, out gid))
                return gid;

            // 2) Fall back to the active group in Session
            var s = Session["CurrentGroupId"];
            if (s != null && int.TryParse(s.ToString(), out gid))
                return gid;

            return null;
        }


    }
}
