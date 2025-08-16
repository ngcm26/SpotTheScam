using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class Approvals : Page
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/User/UserLogin.aspx"); return; }

            if (!IsPostBack)
            {
                var gid = GetGroupId();
                pnlNoGroup.Visible = gid == null;
                pnlMain.Visible = gid != null;
                if (gid == null) return;

                BindMembers(gid.Value);
                hfCanAct.Value = UserCanAct(gid.Value) ? "1" : "0";
                BindList(gid.Value);
            }
        }

        private int CurrentUserId => Convert.ToInt32(Session["UserId"]);
        private int? GetGroupId()
        {
            var o = Session["CurrentGroupId"];
            if (o == null || o == DBNull.Value) return null;
            int gid; return int.TryParse(o.ToString(), out gid) ? (int?)gid : null;
        }

        private bool UserCanAct(int groupId)
        {
            const string sql = @"SELECT COUNT(*) FROM FamilyGroupMembers
                                 WHERE GroupId=@G AND UserId=@U AND Status='Active' AND GroupRole IN ('Guardian','GroupOwner')";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@G", groupId);
                cmd.Parameters.AddWithValue("@U", CurrentUserId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void BindMembers(int groupId)
        {
            const string sql = @"SELECT u.Id, u.Username
                                 FROM FamilyGroupMembers m
                                 JOIN Users u ON u.Id = m.UserId
                                 WHERE m.GroupId=@G AND m.Status='Active' AND m.GroupRole='Primary'
                                 ORDER BY u.Username";
            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@G", groupId);
                var dt = new DataTable();
                da.Fill(dt);
                ddlMember.DataSource = dt;
                ddlMember.DataTextField = "Username";
                ddlMember.DataValueField = "Id";
                ddlMember.DataBind();
                ddlMember.Items.Insert(0, new ListItem("All members", "")); // value "" = no filter
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            var gid = GetGroupId();
            if (gid == null) return;
            BindList(gid.Value);
        }

        private void BindList(int groupId)
        {
            string sql = @"
SELECT
  t.TransactionId, t.TransactionDate, t.TransactionTime, t.Amount, t.TransactionType,
  t.SenderRecipient, t.Description, t.ReviewStatus, t.IsHeld,
  a.AccountNickname, a.AccountNumberMasked,
  u.Username AS MemberName
FROM BankTransactions t
JOIN FamilyGroupMembers m ON m.UserId = t.UserId AND m.GroupId = @G AND m.GroupRole='Primary' AND m.Status='Active'
JOIN BankAccounts a ON a.AccountId = t.AccountId
JOIN Users u ON u.Id = t.UserId
WHERE 1=1";

            // Status filter
            if (ddlStatus.SelectedValue == "Pending")
                sql += " AND (t.IsHeld = 1 AND (t.ReviewStatus IS NULL OR t.ReviewStatus='Pending'))";
            else if (ddlStatus.SelectedValue == "Approved")
                sql += " AND t.ReviewStatus = 'Approved'";
            else if (ddlStatus.SelectedValue == "Denied")
                sql += " AND t.ReviewStatus = 'Denied'";

            // Member filter
            if (!string.IsNullOrEmpty(ddlMember.SelectedValue))
                sql += " AND t.UserId = @MemberId";

            // Date range (date only)
            DateTime from, to;
            bool hasFrom = DateTime.TryParse(txtFrom.Text, out from);
            bool hasTo = DateTime.TryParse(txtTo.Text, out to);

            if (hasFrom) sql += " AND t.TransactionDate >= @From";
            if (hasTo) sql += " AND t.TransactionDate <= @To";

            sql += " ORDER BY t.TransactionDate DESC, t.TransactionTime DESC;";

            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@G", groupId);
                if (!string.IsNullOrEmpty(ddlMember.SelectedValue))
                    da.SelectCommand.Parameters.AddWithValue("@MemberId", Convert.ToInt32(ddlMember.SelectedValue));
                if (hasFrom) da.SelectCommand.Parameters.AddWithValue("@From", from.Date);
                if (hasTo) da.SelectCommand.Parameters.AddWithValue("@To", to.Date);

                var dt = new DataTable();
                da.Fill(dt);
                rptApprovals.DataSource = dt;
                rptApprovals.DataBind();
            }
        }

        protected void rptApprovals_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (hfCanAct.Value != "1") { Show("You don’t have permission to approve/deny."); return; }

            int txnId;
            if (!int.TryParse(e.CommandArgument.ToString(), out txnId)) return;

            if (e.CommandName == "Approve")
                Approve(txnId);
            else if (e.CommandName == "Deny")
                Deny(txnId);

            var gid = GetGroupId();
            if (gid != null) BindList(gid.Value);
        }

        private void Approve(int txnId)
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    // Get txn
                    int accountId = 0; decimal amount = 0; string type = null; bool isHeld = false; string status = null;
                    using (var cmd = new SqlCommand(@"
SELECT AccountId, Amount, TransactionType, IsHeld, ReviewStatus
FROM BankTransactions WITH (UPDLOCK, ROWLOCK)
WHERE TransactionId=@Id;
", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Id", txnId);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) { Show("Transaction not found."); tx.Rollback(); return; }
                            accountId = r.GetInt32(0);
                            amount = r.GetDecimal(1);
                            type = r.GetString(2);
                            isHeld = r.GetBoolean(3);
                            status = r.IsDBNull(4) ? null : r.GetString(4);
                        }
                    }

                    // if already processed, skip
                    if (!isHeld || (status != null && status != "" && status != "Pending"))
                    {
                        Show("This transaction has already been processed.");
                        tx.Rollback(); return;
                    }

                    // Update status
                    using (var cmd = new SqlCommand(@"
UPDATE BankTransactions SET IsHeld=0, ReviewStatus='Approved' WHERE TransactionId=@Id;", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Id", txnId);
                        cmd.ExecuteNonQuery();
                    }

                    // Apply to account balance
                    string balSql = (type == "Withdrawal")
                        ? "UPDATE BankAccounts SET Balance = Balance - @Amt WHERE AccountId=@A;"
                        : "UPDATE BankAccounts SET Balance = Balance + @Amt WHERE AccountId=@A;";

                    using (var cmd = new SqlCommand(balSql, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Amt", amount);
                        cmd.Parameters.AddWithValue("@A", accountId);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    Show("Approved.");
                }
            }
        }

        private void Deny(int txnId)
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(
                "UPDATE BankTransactions SET IsHeld=0, ReviewStatus='Denied' WHERE TransactionId=@Id;", conn))
            {
                cmd.Parameters.AddWithValue("@Id", txnId);
                conn.Open();
                cmd.ExecuteNonQuery();
                Show("Denied.");
            }
        }

        private void Show(string msg)
        {
            pnlMsg.Visible = true;
            lblMsg.Text = msg;
        }

        // Helper to render a status chip from data item
        protected string GetStatusChip(object dataItem)
        {
            var row = (dataItem as DataRowView);
            string status = row["ReviewStatus"] as string;
            bool held = (row["IsHeld"] != DBNull.Value) && (bool)row["IsHeld"];

            if (held || string.IsNullOrEmpty(status))
                return "<span class='chip chip-held'>Pending</span>";
            if (string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase))
                return "<span class='chip chip-approved'>Approved</span>";
            if (string.Equals(status, "Denied", StringComparison.OrdinalIgnoreCase))
                return "<span class='chip chip-denied'>Denied</span>";
            return "<span class='chip'>" + status + "</span>";
        }

        protected void rptApprovals_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var row = (System.Data.DataRowView)e.Item.DataItem;

            bool canAct = hfCanAct.Value == "1"; // set in Page_Load via UserCanAct(...)
            bool isHeld = row["IsHeld"] != DBNull.Value && (bool)row["IsHeld"];
            string status = (row["ReviewStatus"] as string ?? "").Trim();
            bool isPending = string.IsNullOrEmpty(status) || status.Equals("Pending", StringComparison.OrdinalIgnoreCase);

            // Only show buttons if user can act AND item is held/pending
            bool showButtons = canAct && isHeld && isPending;

            var btnApprove = (LinkButton)e.Item.FindControl("btnApprove");
            var btnDeny = (LinkButton)e.Item.FindControl("btnDeny");
            if (btnApprove != null) btnApprove.Visible = showButtons;
            if (btnDeny != null) btnDeny.Visible = showButtons;
        }

    }
}
