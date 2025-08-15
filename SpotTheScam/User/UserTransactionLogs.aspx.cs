using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class UserTransactionLogs : Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/User/UserLogin.aspx"); return; }
            if (!IsPostBack)
            {
                BindAccountsFilter();
                BindGrid();
            }
        }

        private void BindAccountsFilter()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(
                "SELECT AccountId, AccountNickname FROM BankAccounts WHERE UserId=@u ORDER BY AccountNickname", conn))
            {
                cmd.Parameters.Add("@u", SqlDbType.Int).Value = userId;
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                ddlAccount.DataSource = dt;
                ddlAccount.DataTextField = "AccountNickname";
                ddlAccount.DataValueField = "AccountId";
                ddlAccount.DataBind();

                // “All” option
                ddlAccount.Items.Insert(0, new ListItem("All", ""));
            }
        }

        private void BindGrid()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string accountFilter = ddlAccount.SelectedValue ?? "";
            string typeFilter = ddlType.SelectedValue ?? "";

            string sql = @"
                SELECT TOP 50
                    t.TransactionId,
                    t.TransactionDate,
                    t.TransactionTime,
                    t.TransactionType,
                    t.Amount,
                    t.Description,
                    t.SenderRecipient,
                    t.IsFlagged,
                    t.Severity,
                    t.ReviewStatus,
                    t.Notes,
                    t.IsHeld,
                    t.BalanceAfterTransaction,
                    a.AccountNickname
                FROM BankTransactions t
                INNER JOIN BankAccounts a ON a.AccountId = t.AccountId
                WHERE t.UserId = @u";

            if (!string.IsNullOrEmpty(accountFilter)) sql += " AND t.AccountId = @a";
            if (!string.IsNullOrEmpty(typeFilter)) sql += " AND t.TransactionType = @tt";
            if (chkFlaggedOnly.Checked) sql += " AND t.IsFlagged = 1";

            sql += " ORDER BY t.TransactionDate DESC, t.TransactionTime DESC, t.TransactionId DESC";

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@u", SqlDbType.Int).Value = userId;

                if (int.TryParse(accountFilter, out var acctId))
                    cmd.Parameters.Add("@a", SqlDbType.Int).Value = acctId;

                if (!string.IsNullOrEmpty(typeFilter))
                    cmd.Parameters.Add("@tt", SqlDbType.NVarChar, 20).Value = typeFilter;

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                // Friendly Date/Time
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

        protected void gvTxns_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTxns.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void FilterChanged(object sender, EventArgs e)
        {
            gvTxns.PageIndex = 0;
            BindGrid();
        }

        protected void gvTxns_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var data = (DataRowView)e.Row.DataItem;

            // Amount styling (+/-)
            var amtCell = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Row.FindControl("lblAmt");
            if (amtCell != null)
            {
                decimal amount = Convert.ToDecimal(data["Amount"]);
                string type = Convert.ToString(data["TransactionType"]);
                bool deposit = string.Equals(type, "Deposit", StringComparison.OrdinalIgnoreCase);
                amtCell.InnerHtml = $"<span class='{(deposit ? "amt-pos" : "amt-neg")}'>{(deposit ? "+" : "-")}{amount:C}</span>";
            }

            // Precompute once (used by status chip)
            bool isFlagged = data["IsFlagged"] != DBNull.Value && Convert.ToBoolean(data["IsFlagged"]);
            string review = Convert.ToString(data["ReviewStatus"] ?? "").Trim();

            // Severity pill (always safe to set)
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

            // Status chip (only if element exists)
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
                    else { text = "Pending"; }
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
            }

        }
    }
}
