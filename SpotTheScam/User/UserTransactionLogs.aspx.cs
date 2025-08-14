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
                "SELECT AccountId, AccountNickname FROM BankAccounts WHERE UserId=@u ORDER BY CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                ddlAccount.DataSource = dt;
                ddlAccount.DataTextField = "AccountNickname";
                ddlAccount.DataValueField = "AccountId";
                ddlAccount.DataBind();
                ddlAccount.Items.Insert(0, new ListItem("All Accounts", ""));
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
                    t.BalanceAfterTransaction,
                    a.AccountNickname
                FROM BankTransactions t
                INNER JOIN BankAccounts a ON a.AccountId = t.AccountId
                WHERE t.UserId = @u";

            if (!string.IsNullOrEmpty(accountFilter)) sql += " AND t.AccountId = @a";
            if (!string.IsNullOrEmpty(typeFilter)) sql += " AND t.TransactionType = @tt";

            sql += " ORDER BY t.TransactionDate DESC, t.TransactionTime DESC";

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                if (!string.IsNullOrEmpty(accountFilter)) cmd.Parameters.AddWithValue("@a", accountFilter);
                if (!string.IsNullOrEmpty(typeFilter)) cmd.Parameters.AddWithValue("@tt", typeFilter);

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                // add a friendly combined date/time column for display
                dt.Columns.Add("WhenText", typeof(string));
                foreach (DataRow r in dt.Rows)
                {
                    DateTime d = r.Field<DateTime>("TransactionDate");
                    TimeSpan? t = r.IsNull("TransactionTime") ? (TimeSpan?)null : (TimeSpan)r["TransactionTime"];
                    string time = t.HasValue ? DateTime.Today.Add(t.Value).ToString("HH:mm") : "";
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
            decimal amount = Convert.ToDecimal(data["Amount"]);
            string type = Convert.ToString(data["TransactionType"]);
            bool deposit = string.Equals(type, "Deposit", StringComparison.OrdinalIgnoreCase);
            string amtHtml = string.Format("<span class='{0}'>{1}{2:C}</span>",
                deposit ? "amt-pos" : "amt-neg",
                deposit ? "+" : "-",
                amount);
            amtCell.InnerHtml = amtHtml;

            // Status chip
            bool isFlagged = data["IsFlagged"] != DBNull.Value && Convert.ToBoolean(data["IsFlagged"]);
            string review = Convert.ToString(data["ReviewStatus"] ?? "").Trim();

            var lblStatus = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Row.FindControl("lblStatus");
            if (lblStatus == null) return;

            string text = "Normal";
            string css = "chip chip-normal";

            if (isFlagged)
            {
                if (review.Equals("Approved", StringComparison.OrdinalIgnoreCase)) { text = "Approved"; css = "chip chip-approved"; }
                else if (review.Equals("Denied", StringComparison.OrdinalIgnoreCase)) { text = "Denied"; css = "chip chip-denied"; }
                else if (review.Equals("Reviewed", StringComparison.OrdinalIgnoreCase)) { text = "Reviewed"; css = "chip chip-reviewed"; }
                else { text = "Pending"; css = "chip chip-pending"; }
            }

            lblStatus.Attributes["class"] = css;
            lblStatus.InnerText = text;
        }
    }
}
