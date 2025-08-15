using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class MemberTransactionLogs : Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        private int GroupId => int.TryParse(Request.QueryString["groupId"], out var g) ? g : 0;
        private int MemberUserId => int.TryParse(Request.QueryString["userId"], out var u) ? u : 0;
        private int AccountId => int.TryParse(Request.QueryString["accountId"], out var a) ? a : 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/User/UserLogin.aspx"); return; }
            if (GroupId <= 0 || MemberUserId <= 0 || AccountId <= 0)
            { Show("Missing parameters.", false); gvTxns.Visible = false; return; }

            if (!IsPostBack)
            {
                int viewer = Convert.ToInt32(Session["UserId"]);
                if (!ViewerIsGuardianOrOwner(viewer, GroupId) || !TargetIsActivePrimary(MemberUserId, GroupId))
                { Show("You do not have permission to view this member.", false); gvTxns.Visible = false; return; }

                lblMember.Text = GetUsername(MemberUserId);
                lblAccount.Text = GetAccountNickname(AccountId, MemberUserId);
                hlBack.NavigateUrl = $"~/User/MemberAccounts.aspx?groupId={GroupId}&userId={MemberUserId}";
                BindGrid();
            }
        }

        private void BindGrid()
        {
            string sql = @"
                SELECT TOP 50
                    t.TransactionId, t.TransactionDate, t.TransactionTime,
                    t.TransactionType, t.Amount, t.Description, t.SenderRecipient,
                    t.IsFlagged, t.Severity, t.ReviewStatus, t.Notes,
                    t.IsHeld 
                FROM BankTransactions t
                WHERE t.UserId = @u AND t.AccountId = @a";

            if (chkFlaggedOnly.Checked) sql += " AND t.IsFlagged = 1";
            sql += " ORDER BY t.TransactionDate DESC, t.TransactionTime DESC, t.TransactionId DESC";

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add("@u", SqlDbType.Int).Value = MemberUserId;
                cmd.Parameters.Add("@a", SqlDbType.Int).Value = AccountId;

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                dt.Columns.Add("WhenText", typeof(string));
                foreach (DataRow r in dt.Rows)
                {
                    var d = r.Field<DateTime>("TransactionDate");
                    var ts = r.IsNull("TransactionTime") ? (TimeSpan?)null : (TimeSpan)r["TransactionTime"];
                    string time = ts.HasValue ? DateTime.Today.Add(ts.Value).ToString("HH:mm") : "";
                    r["WhenText"] = d.ToString("dd MMM yyyy") + (time == "" ? "" : (" " + time));
                }

                gvTxns.DataSource = dt;
                gvTxns.DataBind();
            }
        }

        protected void FilterChanged(object sender, EventArgs e) { gvTxns.PageIndex = 0; BindGrid(); }
        protected void gvTxns_PageIndexChanging(object sender, GridViewPageEventArgs e) { gvTxns.PageIndex = e.NewPageIndex; BindGrid(); }

        protected void gvTxns_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var data = (DataRowView)e.Row.DataItem;

            var amtCell = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Row.FindControl("lblAmt");
            if (amtCell != null)
            {
                decimal amount = Convert.ToDecimal(data["Amount"]);
                string type = Convert.ToString(data["TransactionType"]);
                bool deposit = string.Equals(type, "Deposit", StringComparison.OrdinalIgnoreCase);
                amtCell.InnerHtml = $"<span class='{(deposit ? "amt-pos" : "amt-neg")}'>{(deposit ? "+" : "-")}{amount:C}</span>";
            }

            bool isFlagged = data["IsFlagged"] != DBNull.Value && Convert.ToBoolean(data["IsFlagged"]);
            string review = Convert.ToString(data["ReviewStatus"] ?? "").Trim();

            var lblSev = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Row.FindControl("lblSev");
            if (lblSev != null)
            {
                string sev = Convert.ToString(data["Severity"] ?? "").Trim();
                string notes = Convert.ToString(data["Notes"] ?? "");
                lblSev.InnerText = string.IsNullOrEmpty(sev) ? "—" : sev;

                string sevClass = "sev-none";
                if (string.Equals(sev, "Red", StringComparison.OrdinalIgnoreCase)) sevClass = "sev-red";
                else if (string.Equals(sev, "Yellow", StringComparison.OrdinalIgnoreCase)) sevClass = "sev-yellow";
                lblSev.Attributes["class"] = "sev-pill " + sevClass;
                if (!string.IsNullOrEmpty(notes)) lblSev.Attributes["title"] = notes;
            }

            var lblStatus = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Row.FindControl("lblStatus");
            if (lblStatus != null)
            {
                string text = "Normal";
                string css = "chip chip-normal";
                if (isFlagged)
                {
                    if (review.Equals("Approved", StringComparison.OrdinalIgnoreCase)) text = "Approved";
                    else if (review.Equals("Denied", StringComparison.OrdinalIgnoreCase)) text = "Denied";
                    else if (review.Equals("Reviewed", StringComparison.OrdinalIgnoreCase)) text = "Reviewed";
                    else text = "Pending";
                    css = "chip chip-" + text.ToLowerInvariant();
                }
                lblStatus.Attributes["class"] = css;
                lblStatus.InnerText = text;
            }
            var lblHeld = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Row.FindControl("lblHeld");
            if (lblHeld != null)
            {
                bool isHeldRow = data["IsHeld"] != DBNull.Value && Convert.ToBoolean(data["IsHeld"]);
                lblHeld.Style["display"] = isHeldRow ? "inline-block" : "none";

                var btnApprove = (LinkButton)e.Row.FindControl("btnApprove");
                var btnDeny = (LinkButton)e.Row.FindControl("btnDeny");
                if (btnApprove != null) btnApprove.Visible = isHeldRow;
                if (btnDeny != null) btnDeny.Visible = isHeldRow;

            }

        }


        protected void gvTxns_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;

            int viewer = Convert.ToInt32(Session["UserId"]);
            if (!ViewerIsGuardianOrOwner(viewer, GroupId) || !TargetIsActivePrimary(MemberUserId, GroupId))
            {
                Show("You do not have permission to perform this action.", false);
                return;
            }

            int tid = Convert.ToInt32(e.CommandArgument);

            try
            {
                using (var con = new SqlConnection(cs))
                {
                    con.Open();
                    var tx = con.BeginTransaction();
                    try
                    {
                        int acctId = 0, uid = 0;
                        string type = "";
                        decimal amount = 0m;
                        bool isHeld = false;

                        // Fetch txn and ensure it belongs to the target member + account
                        using (var cmd = new SqlCommand(@"
                                SELECT TransactionId, AccountId, UserId, TransactionType, Amount, IsHeld
                                FROM dbo.BankTransactions
                                WHERE TransactionId=@id AND UserId=@u AND AccountId=@a;", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", tid);
                            cmd.Parameters.AddWithValue("@u", MemberUserId);
                            cmd.Parameters.AddWithValue("@a", AccountId);

                            using (var rd = cmd.ExecuteReader())
                            {
                                if (!rd.Read())
                                {
                                    Show("Transaction not found.", false);
                                    tx.Rollback();
                                    return;
                                }
                                acctId = Convert.ToInt32(rd["AccountId"]);
                                uid = Convert.ToInt32(rd["UserId"]);
                                type = Convert.ToString(rd["TransactionType"]);
                                amount = Convert.ToDecimal(rd["Amount"]);
                                isHeld = Convert.ToBoolean(rd["IsHeld"]);
                            }
                        }

                        if (!isHeld)
                        {
                            Show("This transaction is not held.", false);
                            tx.Rollback();
                            return;
                        }

                        if (e.CommandName == "Approve")
                        {
                            // Current account balance
                            decimal currBal = 0m;
                            using (var cmd = new SqlCommand(
                                "SELECT Balance FROM dbo.BankAccounts WHERE AccountId=@a AND UserId=@u;", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@a", acctId);
                                cmd.Parameters.AddWithValue("@u", uid);
                                object r = cmd.ExecuteScalar();
                                if (r == null) { Show("Account not found.", false); tx.Rollback(); return; }
                                currBal = Convert.ToDecimal(r);
                            }

                            // Compute new balance (guard against insufficient funds)
                            decimal newBal = currBal;
                            if (string.Equals(type, "Withdrawal", StringComparison.OrdinalIgnoreCase))
                            {
                                if (currBal < amount)
                                {
                                    Show("Cannot approve: insufficient funds.", false);
                                    tx.Rollback();
                                    return;
                                }
                                newBal = currBal - amount;
                            }
                            else
                            {
                                newBal = currBal + amount; // Deposit/Transfer-in
                            }

                            // Apply balance, clear hold, stamp ReviewStatus & BalanceAfterTransaction
                            using (var updA = new SqlCommand(
                                "UPDATE dbo.BankAccounts SET Balance=@b WHERE AccountId=@a;", con, tx))
                            {
                                updA.Parameters.AddWithValue("@b", newBal);
                                updA.Parameters.AddWithValue("@a", acctId);
                                updA.ExecuteNonQuery();
                            }

                            using (var updT = new SqlCommand(@"
                                    UPDATE dbo.BankTransactions
                                    SET IsHeld=0, ReviewStatus='Approved', BalanceAfterTransaction=@b
                                    WHERE TransactionId=@id AND IsHeld=1;
                                    ", con, tx))
                            {
                                updT.Parameters.AddWithValue("@b", newBal);
                                updT.Parameters.AddWithValue("@id", tid);
                                updT.ExecuteNonQuery();
                            }

                            tx.Commit();
                            Show("Approved. Balance updated.", true);
                        }
                        else if (e.CommandName == "Deny")
                        {
                            using (var upd = new SqlCommand(@"
                                    UPDATE dbo.BankTransactions
                                    SET ReviewStatus='Denied'
                                    WHERE TransactionId=@id AND IsHeld=1;
                                    ", con, tx))
                            {
                                upd.Parameters.AddWithValue("@id", tid);
                                upd.ExecuteNonQuery();
                            }

                            tx.Commit();
                            Show("Denied. This transfer will not affect balance.", true);
                        }
                        else
                        {
                            tx.Rollback();
                            return;
                        }

                        // refresh
                        BindGrid();
                    }
                    catch (Exception ex1)
                    {
                        try { tx.Rollback(); } catch { }
                        Show("Action failed: " + ex1.Message, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Show("Unexpected error: " + ex.Message, false);
            }
        }

        private string GetUsername(int uid)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT Username FROM Users WHERE Id=@u", con))
            { cmd.Parameters.Add("@u", SqlDbType.Int).Value = uid; con.Open(); return Convert.ToString(cmd.ExecuteScalar()); }
        }

        private string GetAccountNickname(int acctId, int ownerId)
        {
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT AccountNickname FROM BankAccounts WHERE AccountId=@a AND UserId=@u", con))
            { cmd.Parameters.Add("@a", SqlDbType.Int).Value = acctId; cmd.Parameters.Add("@u", SqlDbType.Int).Value = ownerId; con.Open(); return Convert.ToString(cmd.ExecuteScalar()); }
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
