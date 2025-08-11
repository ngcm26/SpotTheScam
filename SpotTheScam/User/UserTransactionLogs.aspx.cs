using AngleSharp.Dom;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SpotTheScam.User
{
    public partial class UserTransactionLogs : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
        private List<DataRow> userAccounts; // To store account data for the carousel
        private int currentAccountIndex = 0; // To track the current account displayed in the carousel

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/User/UserLogin.aspx");
                    return;
                }

                LoadUserAccountsForOverview();
                PopulateAccountDropdown();
                LoadTransactions();
            }
        }

        private void LoadUserAccountsForOverview()
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);
            decimal totalBalance = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT AccountId, BankName, AccountType, AccountNumber, AccountNickname, Balance FROM BankAccounts WHERE UserId = @UserId ORDER BY DateAdded ASC";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@UserId", currentUserId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                userAccounts = new List<DataRow>();
                foreach (DataRow row in dt.Rows)
                {
                    userAccounts.Add(row);
                    totalBalance += Convert.ToDecimal(row["Balance"]);
                }

                ltTotalBalance.Text = totalBalance.ToString("C2"); // Format as currency

                if (userAccounts.Count > 0)
                {
                    pnlNoAccounts.Visible = false;
                    pnlAccountCards.Visible = true;
                    DisplayCurrentAccount();
                    RenderPaginationDots();
                }
                else
                {
                    pnlNoAccounts.Visible = true;
                    pnlAccountCards.Visible = false;
                }
            }
        }

        private void DisplayCurrentAccount()
        {
            if (userAccounts != null && userAccounts.Count > 0)
            {
                // Ensure currentAccountIndex is within bounds
                if (currentAccountIndex < 0) currentAccountIndex = userAccounts.Count - 1;
                if (currentAccountIndex >= userAccounts.Count) currentAccountIndex = 0;

                DataTable dt = new DataTable();
                dt.Columns.Add("AccountId", typeof(int));
                dt.Columns.Add("BankName", typeof(string));
                dt.Columns.Add("AccountType", typeof(string));
                dt.Columns.Add("AccountNumber", typeof(string));
                dt.Columns.Add("AccountNickname", typeof(string));
                dt.Columns.Add("Balance", typeof(decimal));

                dt.ImportRow(userAccounts[currentAccountIndex]);

                rptAccounts.DataSource = dt;
                rptAccounts.DataBind();
            }
        }

        private void RenderPaginationDots()
        {
            if (userAccounts != null && userAccounts.Count > 1)
            {
                StringBuilder dotsHtml = new StringBuilder();
                for (int i = 0; i < userAccounts.Count; i++)
                {
                    string activeClass = (i == currentAccountIndex) ? " active" : "";
                    dotsHtml.Append($"<span class=\"pagination-dot{activeClass}\" onclick=\"__doPostBack('AccountDotClick', '{i}')\"></span>");
                }
                accountDots.InnerHtml = dotsHtml.ToString();
            }
            else
            {
                accountDots.InnerHtml = ""; // Hide dots if only one or no accounts
            }
        }

        protected void btnPrevAccount_Click(object sender, EventArgs e)
        {
            currentAccountIndex--;
            DisplayCurrentAccount();
            RenderPaginationDots();
        }

        protected void btnNextAccount_Click(object sender, EventArgs e)
        {
            currentAccountIndex++;
            DisplayCurrentAccount();
            RenderPaginationDots();
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            if (Request["__EVENTTARGET"] == "AccountDotClick")
            {
                if (int.TryParse(Request["__EVENTARGUMENT"], out int index))
                {
                    currentAccountIndex = index;
                    LoadUserAccountsForOverview();
                }
            }
        }

        private void PopulateAccountDropdown()
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT AccountId, AccountNickname, BankName, AccountNumber FROM BankAccounts WHERE UserId = @UserId ORDER BY BankName, AccountNickname";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@UserId", currentUserId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlAccount.DataSource = dt;
                ddlAccount.DataTextField = "AccountNickname";
                ddlAccount.DataValueField = "AccountId";
                ddlAccount.DataBind();

                ddlAccount.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Accounts", "All"));

            }
        }

        private void LoadTransactions()
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append(@"
            SELECT 
            bt.TransactionId,
            bt.TransactionDate,
            bt.TransactionTime,
            bt.Description,
            bt.TransactionType,
            bt.Amount,
            bt.SenderRecipient,
            bt.BalanceAfterTransaction,
            bt.IsFlagged,
            bt.Severity,
            ba.BankName,
            ba.AccountNickname
            FROM BankTransactions bt
            INNER JOIN BankAccounts ba ON bt.AccountId = ba.AccountId
            WHERE bt.UserId = @UserId
        ");

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserId", currentUserId);

                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    queryBuilder.Append(" AND bt.TransactionDate >= @StartDate");
                    cmd.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(txtStartDate.Text));
                }
                if (!string.IsNullOrEmpty(txtEndDate.Text))
                {
                    queryBuilder.Append(" AND bt.TransactionDate <= @EndDate");
                    cmd.Parameters.AddWithValue("@EndDate", Convert.ToDateTime(txtEndDate.Text));
                }

                if (ddlTransactionType.SelectedValue != "All")
                {
                    queryBuilder.Append(" AND bt.TransactionType = @TransactionType");
                    cmd.Parameters.AddWithValue("@TransactionType", ddlTransactionType.SelectedValue);
                }

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    queryBuilder.Append(" AND (bt.Description LIKE @SearchTerm OR bt.SenderRecipient LIKE @SearchTerm)");
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + txtSearch.Text.Trim() + "%");
                }

                if (ddlAccount.SelectedValue != "All")
                {
                    queryBuilder.Append(" AND bt.AccountId = @AccountId");
                    cmd.Parameters.AddWithValue("@AccountId", Convert.ToInt32(ddlAccount.SelectedValue));
                }

                // --- NEW: Amount Range Filtering ---
                decimal minAmount;
                if (decimal.TryParse(txtMinAmount.Text, out minAmount))
                {
                    queryBuilder.Append(" AND bt.Amount >= @MinAmount");
                    cmd.Parameters.AddWithValue("@MinAmount", minAmount);
                }

                decimal maxAmount;
                if (decimal.TryParse(txtMaxAmount.Text, out maxAmount))
                {
                    queryBuilder.Append(" AND bt.Amount <= @MaxAmount");
                    cmd.Parameters.AddWithValue("@MaxAmount", maxAmount);
                }
                // --- END NEW ---

                queryBuilder.Append(" ORDER BY bt.TransactionDate DESC, bt.TransactionTime DESC");

                cmd.CommandText = queryBuilder.ToString();
                cmd.Connection = conn;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // --- Inject a "spacer" row after every real row ---
                if (dt.Rows.Count > 0)
                {
                    DataTable dtWithSpacers = dt.Clone();
                    foreach (DataRow row in dt.Rows)
                    {
                        dtWithSpacers.ImportRow(row);
                        // Insert an empty "spacer" row with NULL TransactionId (the DataKey)
                        DataRow spacer = dtWithSpacers.NewRow();
                        spacer["TransactionId"] = DBNull.Value; // This is what you'll check for
                        dtWithSpacers.Rows.Add(spacer);
                    }
                    gvTransactions.DataSource = dtWithSpacers;
                }
                else
                {
                    gvTransactions.DataSource = dt;
                }

                gvTransactions.DataBind();
            }
        }


        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            LoadTransactions();
            ShowAlert("Filters applied successfully!", "success");
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtStartDate.Text = string.Empty;
            txtEndDate.Text = string.Empty;
            ddlTransactionType.SelectedValue = "All";
            txtSearch.Text = string.Empty;
            ddlAccount.SelectedValue = "All";
            // --- NEW: Clear Amount Filters ---
            txtMinAmount.Text = string.Empty;
            txtMaxAmount.Text = string.Empty;
            // --- END NEW ---
            LoadTransactions();
            ShowAlert("Filters cleared.", "success");
        }

        private DataTable GetFilteredTransactions()
        {
            DataTable dt = new DataTable();
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append(@"
            SELECT 
                bt.TransactionDate,
                bt.TransactionTime,
                bt.Description,
                bt.TransactionType,
                bt.Amount,
                bt.SenderRecipient,
                bt.BalanceAfterTransaction,
                ba.BankName,
                ba.AccountNickname
            FROM BankTransactions bt
            INNER JOIN BankAccounts ba ON bt.AccountId = ba.AccountId
            WHERE bt.UserId = @UserId
        ");

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@UserId", currentUserId);

                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    queryBuilder.Append(" AND bt.TransactionDate >= @StartDate");
                    cmd.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(txtStartDate.Text));
                }
                if (!string.IsNullOrEmpty(txtEndDate.Text))
                {
                    queryBuilder.Append(" AND bt.TransactionDate <= @EndDate");
                    cmd.Parameters.AddWithValue("@EndDate", Convert.ToDateTime(txtEndDate.Text));
                }
                if (ddlTransactionType.SelectedValue != "All")
                {
                    queryBuilder.Append(" AND bt.TransactionType = @TransactionType");
                    cmd.Parameters.AddWithValue("@TransactionType", ddlTransactionType.SelectedValue);
                }
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    queryBuilder.Append(" AND (bt.Description LIKE @SearchTerm OR bt.SenderRecipient LIKE @SearchTerm)");
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + txtSearch.Text.Trim() + "%");
                }
                if (ddlAccount.SelectedValue != "All")
                {
                    queryBuilder.Append(" AND bt.AccountId = @AccountId");
                    cmd.Parameters.AddWithValue("@AccountId", Convert.ToInt32(ddlAccount.SelectedValue));
                }

                decimal minAmount;
                if (decimal.TryParse(txtMinAmount.Text, out minAmount))
                {
                    queryBuilder.Append(" AND bt.Amount >= @MinAmount");
                    cmd.Parameters.AddWithValue("@MinAmount", minAmount);
                }

                decimal maxAmount;
                if (decimal.TryParse(txtMaxAmount.Text, out maxAmount))
                {
                    queryBuilder.Append(" AND bt.Amount <= @MaxAmount");
                    cmd.Parameters.AddWithValue("@MaxAmount", maxAmount);
                }

                queryBuilder.Append(" ORDER BY bt.TransactionDate DESC, bt.TransactionTime DESC");

                cmd.CommandText = queryBuilder.ToString();
                cmd.Connection = conn;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        protected void gvTransactions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTransactions.PageIndex = e.NewPageIndex;
            LoadTransactions();
        }

        private void ShowAlert(string message, string type)
        {
            string cssClass = type == "success" ? "alert alert-success" : "alert alert-error";
            AlertPanel.CssClass = cssClass;
            AlertMessage.Text = message;
            AlertPanel.Visible = true;

            string script = @"
                setTimeout(function() {
                    var alertPanel = document.getElementById('" + AlertPanel.ClientID + @"');
                    if (alertPanel) {
                        alertPanel.style.display = 'none';
                    }
                }, 5000);
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideAlert", script, true);
        }

        protected void gvTransactions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Detect spacer row by TransactionId == null/DBNull
                var data = DataBinder.Eval(e.Row.DataItem, "TransactionId");
                if (data == DBNull.Value || data == null)
                {
                    int columns = ((GridView)sender).Columns.Count;
                    e.Row.Cells.Clear();
                    for (int i = 0; i < columns; i++)
                        e.Row.Cells.Add(new TableCell());
                    e.Row.CssClass = "gv-row-spacer";
                    return;
                }



                // --- Your normal row logic below ---
                bool isFlagged = false;
                string severity = "";

                if (DataBinder.Eval(e.Row.DataItem, "IsFlagged") != DBNull.Value)
                    isFlagged = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "IsFlagged"));
                if (DataBinder.Eval(e.Row.DataItem, "Severity") != DBNull.Value)
                    severity = DataBinder.Eval(e.Row.DataItem, "Severity").ToString();

                if (isFlagged)
                {
                    if (severity == "red")
                        e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#fee2e2");
                    else if (severity == "yellow")
                        e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#fefcbf");
                }

                string txnId = DataBinder.Eval(e.Row.DataItem, "TransactionId").ToString();
                e.Row.Attributes["data-id"] = txnId;
                e.Row.CssClass += " transaction-row cursor-pointer";
            }
        }


        [System.Web.Services.WebMethod]
        public static string GetTransactionDetails(int id)
        {
            string html = "";
            string connStr = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM BankTransactions WHERE TransactionId = @TransactionId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TransactionId", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    html += $"<p><strong>Date:</strong> {Convert.ToDateTime(reader["TransactionDate"]).ToShortDateString()}</p>";
                    html += $"<p><strong>Time:</strong> {reader["TransactionTime"]}</p>";
                    html += $"<p><strong>Type:</strong> {reader["TransactionType"]}</p>";
                    html += $"<p><strong>Amount:</strong> {reader["Amount"]}</p>";
                    html += $"<p><strong>Description:</strong> {reader["Description"]}</p>";
                    html += $"<p><strong>Sender/Recipient:</strong> {reader["SenderRecipient"]}</p>";
                }
            }
            return html;
        }

        protected void chkFlag_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chk.NamingContainer;
            string transactionId = ((CheckBox)sender).ToolTip;


            if (!int.TryParse(transactionId, out int tid)) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE BankTransactions SET IsFlagged = @IsFlagged WHERE TransactionId = @TransactionId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IsFlagged", chk.Checked);
                cmd.Parameters.AddWithValue("@TransactionId", tid);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadTransactions();
        }

        protected void btnBulkFlag_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvTransactions.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkRow");
                if (chk != null && chk.Checked)
                {
                    int transactionId = Convert.ToInt32(gvTransactions.DataKeys[row.RowIndex].Value);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE BankTransactions SET IsFlagged = 1, Severity = 'yellow' WHERE TransactionId = @TransactionId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            ShowAlert("Selected transactions flagged as yellow.", "success");
            LoadTransactions();
        }

        protected void btnBulkUnflag_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvTransactions.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkRow");
                if (chk != null && chk.Checked)
                {
                    int transactionId = Convert.ToInt32(gvTransactions.DataKeys[row.RowIndex].Value);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE BankTransactions SET IsFlagged = 0, Severity = NULL WHERE TransactionId = @TransactionId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            ShowAlert("Selected transactions unflagged.", "success");
            LoadTransactions();
        }

        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            DataTable dt = GetFilteredTransactions();

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=TransactionLogs.pdf");
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            using (MemoryStream ms = new MemoryStream())
            {
                iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                PdfPTable table = new PdfPTable(dt.Columns.Count);
                table.WidthPercentage = 100;

                // Add headers
                foreach (DataColumn col in dt.Columns)
                {
                    table.AddCell(new Phrase(col.ColumnName));
                }

                // Add data rows
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        table.AddCell(new Phrase(row[col].ToString()));
                    }
                }

                doc.Add(table);
                doc.Close();

                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = GetFilteredTransactions();

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=TransactionLogs.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    // Create a GridView for export
                    GridView exportGrid = new GridView();
                    exportGrid.DataSource = dt;
                    exportGrid.DataBind();

                    exportGrid.RenderControl(hw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
        }

        // Required for exporting controls to Excel
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Do nothing, required for GridView export
        }

    }
}