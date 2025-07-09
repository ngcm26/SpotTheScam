using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Collections.Generic;

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
            // This handles the postback from JavaScript for dot clicks
            if (Request["__EVENTTARGET"] == "AccountDotClick")
            {
                if (int.TryParse(Request["__EVENTARGUMENT"], out int index))
                {
                    currentAccountIndex = index;
                    LoadUserAccountsForOverview(); // Reload to display the correct account
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

                ddlAccount.Items.Insert(0, new ListItem("All Accounts", "All"));
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

                queryBuilder.Append(" ORDER BY bt.TransactionDate DESC, bt.TransactionTime DESC");

                cmd.CommandText = queryBuilder.ToString();
                cmd.Connection = conn;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvTransactions.DataSource = dt;
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
            LoadTransactions();
            ShowAlert("Filters cleared.", "success");
        }

        protected void btnExportCsv_Click(object sender, EventArgs e)
        {
            ExportTransactionsToCsv();
        }

        private void ExportTransactionsToCsv()
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

                queryBuilder.Append(" ORDER BY bt.TransactionDate DESC, bt.TransactionTime DESC");

                cmd.CommandText = queryBuilder.ToString();
                cmd.Connection = conn;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            if (dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (DataColumn col in dt.Columns)
                {
                    sb.AppendFormat("\"{0}\",", col.ColumnName);
                }
                sb.AppendLine();

                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        sb.AppendFormat("\"{0}\",", row[col.ColumnName].ToString().Replace("\"", "\"\""));
                    }
                    sb.AppendLine();
                }

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=TransactionLogs_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            else
            {
                ShowAlert("No transactions to export.", "error");
            }
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
    }
}
