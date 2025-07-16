using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class FamilyGroupManagement : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected int CurrentUserId => Convert.ToInt32(Session["UserId"]);
        protected int? CurrentGroupId
        {
            get { return Session["CurrentGroupId"] as int?; }
            set { Session["CurrentGroupId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadUserGroup();
            }
        }

        private void LoadUserGroup()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT fg.GroupId, fg.GroupName
                    FROM FamilyGroups fg
                    INNER JOIN FamilyGroupMembers fgm ON fg.GroupId = fgm.GroupId
                    WHERE fgm.UserId = @UserId
                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    CurrentGroupId = Convert.ToInt32(reader["GroupId"]);
                    ltGroupName.Text = reader["GroupName"].ToString();
                    pnlGroupInfo.Visible = true;
                    LoadGroupMembers();
                }
                else
                {
                    pnlGroupInfo.Visible = false;
                }
                reader.Close();
            }
        }

        protected void btnCreateGroup_Click(object sender, EventArgs e)
        {
            string groupName = txtGroupName.Text.Trim();
            if (string.IsNullOrEmpty(groupName))
            {
                ShowAlert("Group name cannot be empty.", "danger");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    string insertGroup = "INSERT INTO FamilyGroups (GroupName, CreatedByUserId) OUTPUT INSERTED.GroupId VALUES (@GroupName, @CreatedByUserId)";
                    SqlCommand cmdGroup = new SqlCommand(insertGroup, conn, transaction);
                    cmdGroup.Parameters.AddWithValue("@GroupName", groupName);
                    cmdGroup.Parameters.AddWithValue("@CreatedByUserId", CurrentUserId);
                    int groupId = (int)cmdGroup.ExecuteScalar();

                    string insertMember = "INSERT INTO FamilyGroupMembers (GroupId, UserId, Role) VALUES (@GroupId, @UserId, @Role)";
                    SqlCommand cmdMember = new SqlCommand(insertMember, conn, transaction);
                    cmdMember.Parameters.AddWithValue("@GroupId", groupId);
                    cmdMember.Parameters.AddWithValue("@UserId", CurrentUserId);
                    cmdMember.Parameters.AddWithValue("@Role", "admin");
                    cmdMember.ExecuteNonQuery();

                    transaction.Commit();
                    CurrentGroupId = groupId;
                    ShowAlert("Family group created successfully!", "success");
                    LoadUserGroup();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowAlert("Error creating group: " + ex.Message, "danger");
                }
            }
        }

        protected void btnAddMember_Click(object sender, EventArgs e)
        {
            if (CurrentGroupId == null)
            {
                ShowAlert("No group selected.", "danger");
                return;
            }

            string email = txtAddMemberEmail.Text.Trim();
            string role = ddlRole.SelectedValue;

            if (string.IsNullOrEmpty(email))
            {
                ShowAlert("Email cannot be empty.", "danger");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string getUser = "SELECT Id FROM Users WHERE Email = @Email";
                SqlCommand cmdUser = new SqlCommand(getUser, conn);
                cmdUser.Parameters.AddWithValue("@Email", email);

                conn.Open();
                object userIdObj = cmdUser.ExecuteScalar();
                if (userIdObj == null)
                {
                    ShowAlert("User with this email does not exist.", "danger");
                    return;
                }
                int userId = (int)userIdObj;

                string checkMember = "SELECT COUNT(*) FROM FamilyGroupMembers WHERE GroupId = @GroupId AND UserId = @UserId";
                SqlCommand cmdCheck = new SqlCommand(checkMember, conn);
                cmdCheck.Parameters.AddWithValue("@GroupId", CurrentGroupId);
                cmdCheck.Parameters.AddWithValue("@UserId", userId);
                int exists = (int)cmdCheck.ExecuteScalar();
                if (exists > 0)
                {
                    ShowAlert("User is already a member of this group.", "danger");
                    return;
                }

                string insertMember = "INSERT INTO FamilyGroupMembers (GroupId, UserId, Role) VALUES (@GroupId, @UserId, @Role)";
                SqlCommand cmdInsert = new SqlCommand(insertMember, conn);
                cmdInsert.Parameters.AddWithValue("@GroupId", CurrentGroupId);
                cmdInsert.Parameters.AddWithValue("@UserId", userId);
                cmdInsert.Parameters.AddWithValue("@Role", role);
                cmdInsert.ExecuteNonQuery();

                ShowAlert("Member added successfully!", "success");
                LoadGroupMembers();
            }
        }

        private void LoadGroupMembers()
        {
            if (CurrentGroupId == null) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT u.Id AS UserId, u.Username, u.Email, fgm.Role
                    FROM FamilyGroupMembers fgm
                    INNER JOIN Users u ON fgm.UserId = u.Id
                    WHERE fgm.GroupId = @GroupId
                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupId", CurrentGroupId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvMembers.DataSource = dt;
                gvMembers.DataBind();
            }
        }

        protected void btnRemoveMember_Click(object sender, EventArgs e)
        {
            if (CurrentGroupId == null) return;

            var btn = (System.Web.UI.WebControls.Button)sender;
            int userId = Convert.ToInt32(btn.CommandArgument);

            // Prevent removing self as admin
            if (userId == CurrentUserId)
            {
                ShowAlert("You cannot remove yourself from the group.", "danger");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string delete = "DELETE FROM FamilyGroupMembers WHERE GroupId = @GroupId AND UserId = @UserId";
                SqlCommand cmd = new SqlCommand(delete, conn);
                cmd.Parameters.AddWithValue("@GroupId", CurrentGroupId);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            ShowAlert("Member removed successfully.", "success");
            LoadGroupMembers();
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = type == "success" ? "alert alert-success" : "alert alert-danger";
            AlertMessage.Text = message;
            AlertPanel.Visible = true;
        }

        protected int EditingRowIndex
        {
            get { return ViewState["EditingRowIndex"] != null ? (int)ViewState["EditingRowIndex"] : -1; }
            set { ViewState["EditingRowIndex"] = value; }
        }

        protected void gvMembers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRole")
            {
                int index = Convert.ToInt32(((GridViewRow)((Button)e.CommandSource).NamingContainer).RowIndex);
                gvMembers.EditIndex = index;
                EditingRowIndex = index;
                LoadGroupMembers();
            }
            else if (e.CommandName == "SaveRole")
            {
                int index = Convert.ToInt32(((GridViewRow)((Button)e.CommandSource).NamingContainer).RowIndex);
                GridViewRow row = gvMembers.Rows[index];
                int userId = Convert.ToInt32(e.CommandArgument);
                DropDownList ddlEditRole = (DropDownList)row.FindControl("ddlEditRole");
                string newRole = ddlEditRole.SelectedValue;

                if (CurrentGroupId == null) return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string update = "UPDATE FamilyGroupMembers SET Role = @Role WHERE GroupId = @GroupId AND UserId = @UserId";
                    SqlCommand cmd = new SqlCommand(update, conn);
                    cmd.Parameters.AddWithValue("@Role", newRole);
                    cmd.Parameters.AddWithValue("@GroupId", CurrentGroupId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                gvMembers.EditIndex = -1;
                EditingRowIndex = -1;
                ShowAlert("Role updated successfully.", "success");
                LoadGroupMembers();
            }
            else if (e.CommandName == "CancelEdit")
            {
                gvMembers.EditIndex = -1;
                EditingRowIndex = -1;
                LoadGroupMembers();
            }
        }

        protected void gvMembers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Only show Edit/Save/Cancel buttons for admins
                DataRowView drv = (DataRowView)e.Row.DataItem;
                string role = drv["Role"].ToString();
                int userId = Convert.ToInt32(drv["UserId"]);
                bool isAdmin = IsCurrentUserAdmin();

                Button btnEdit = (Button)e.Row.FindControl("btnEdit");
                Button btnSave = (Button)e.Row.FindControl("btnSave");
                Button btnCancel = (Button)e.Row.FindControl("btnCancel");

                if (btnEdit != null) btnEdit.Visible = isAdmin;
                if (btnSave != null) btnSave.Visible = isAdmin;
                if (btnCancel != null) btnCancel.Visible = isAdmin;
            }
        }

        private bool IsCurrentUserAdmin()
        {
            if (CurrentGroupId == null) return false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Role FROM FamilyGroupMembers WHERE GroupId = @GroupId AND UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GroupId", CurrentGroupId);
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                conn.Open();
                object roleObj = cmd.ExecuteScalar();
                return roleObj != null && roleObj.ToString() == "admin";
            }
        }

    }
}
