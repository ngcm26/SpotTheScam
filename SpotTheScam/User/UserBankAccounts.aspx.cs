using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text; // Required for StringBuilder
using System.Collections.Generic; // Required for List<DataRow>

namespace SpotTheScam.User
{
    public partial class UserBankAccounts : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        // This list will now be populated from Session or DB on every load
        private List<DataRow> userAccounts;

        // Use a ViewState-backed property to reliably track the current account index
        private int CurrentAccountIndex
        {
            get
            {
                // Retrieve from ViewState, default to 0 if not set
                if (ViewState["CurrentAccountIndex"] == null)
                    return 0;
                return (int)ViewState["CurrentAccountIndex"];
            }
            set
            {
                ViewState["CurrentAccountIndex"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            // Always attempt to load the user accounts data
            // It will either come from Session (if already loaded) or from the DB
            LoadUserAccountsForOverview(); // This method is now responsible for populating 'userAccounts' from Session or DB

            // Load accounts for the GridView (Transaction Logs) - separate data source
            LoadBankAccounts();
        }

        private void LoadUserAccountsForOverview()
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);
            decimal totalBalance = 0;
            DataTable dtAccounts;

            // Check if account data is already in Session
            if (Session["UserBankAccountsData"] != null)
            {
                dtAccounts = (DataTable)Session["UserBankAccountsData"];
            }
            else // If not in Session, load from database
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT AccountId, BankName, AccountType, AccountNumber, AccountNickname, Balance FROM BankAccounts WHERE UserId = @UserId ORDER BY DateAdded ASC";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@UserId", currentUserId);

                    dtAccounts = new DataTable();
                    da.Fill(dtAccounts);

                    // Store the DataTable in Session for subsequent postbacks
                    Session["UserBankAccountsData"] = dtAccounts;
                }
            }

            // Populate the local userAccounts list and calculate total balance
            userAccounts = new List<DataRow>();
            foreach (DataRow row in dtAccounts.Rows)
            {
                userAccounts.Add(row);
                totalBalance += Convert.ToDecimal(row["Balance"]);
            }

            ltTotalBalance.Text = totalBalance.ToString("C2"); // Format as currency

            if (userAccounts.Count > 0)
            {
                pnlNoAccounts.Visible = false;
                pnlAccountOverview.Visible = true;
                DisplayCurrentAccount(); // Display the account based on CurrentAccountIndex
                RenderPaginationDots(); // Render pagination dots
            }
            else
            {
                pnlNoAccounts.Visible = true;
                pnlAccountOverview.Visible = false;
            }
        }

        private void DisplayCurrentAccount()
        {
            if (userAccounts != null && userAccounts.Count > 0)
            {
                // Ensure CurrentAccountIndex is within bounds after navigation
                if (CurrentAccountIndex < 0) CurrentAccountIndex = userAccounts.Count - 1;
                if (CurrentAccountIndex >= userAccounts.Count) CurrentAccountIndex = 0;

                // Create a new DataTable containing only the current account's data for the Repeater
                // This is how your current Repeater binding is set up to show one card at a time.
                DataTable dtCurrentAccount = new DataTable();
                dtCurrentAccount.Columns.Add("AccountId", typeof(int));
                dtCurrentAccount.Columns.Add("BankName", typeof(string));
                dtCurrentAccount.Columns.Add("AccountType", typeof(string));
                dtCurrentAccount.Columns.Add("AccountNumber", typeof(string));
                dtCurrentAccount.Columns.Add("AccountNickname", typeof(string));
                dtCurrentAccount.Columns.Add("Balance", typeof(decimal));

                dtCurrentAccount.ImportRow(userAccounts[CurrentAccountIndex]);

                rptAccounts.DataSource = dtCurrentAccount;
                rptAccounts.DataBind();
            }
            else
            {
                rptAccounts.DataSource = null; // Clear repeater if no accounts
                rptAccounts.DataBind();
            }
        }

        private void RenderPaginationDots()
        {
            // Clear existing dots first
            accountDots.InnerHtml = "";

            if (userAccounts != null && userAccounts.Count > 1)
            {
                StringBuilder dotsHtml = new StringBuilder();
                for (int i = 0; i < userAccounts.Count; i++)
                {
                    string activeClass = (i == CurrentAccountIndex) ? " active" : "";
                    // Using __doPostBack to handle dot clicks server-side
                    dotsHtml.Append($"<span class=\"pagination-dot{activeClass}\" onclick=\"__doPostBack('AccountDotClick', '{i}')\"></span>");
                }
                accountDots.InnerHtml = dotsHtml.ToString();
            }
            else
            {
                // Ensure dots are hidden if 0 or 1 account
                accountDots.InnerHtml = "";
            }
        }

        protected void btnPrevAccount_Click(object sender, EventArgs e)
        {
            if (userAccounts != null && userAccounts.Count > 0)
            {
                CurrentAccountIndex--; // Decrement index
                DisplayCurrentAccount();
                RenderPaginationDots();
            }
        }

        protected void btnNextAccount_Click(object sender, EventArgs e)
        {
            if (userAccounts != null && userAccounts.Count > 0)
            {
                CurrentAccountIndex++; // Increment index
                DisplayCurrentAccount();
                RenderPaginationDots();
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            // This handles clicks on the pagination dots
            if (Request["__EVENTTARGET"] == "AccountDotClick")
            {
                if (int.TryParse(Request["__EVENTARGUMENT"], out int index))
                {
                    CurrentAccountIndex = index;
                    // Reload overview, which will re-display current account and dots
                    LoadUserAccountsForOverview();
                }
            }
        }

        private void LoadBankAccounts()
        {
            if (Session["UserId"] == null)
            {
                ShowAlert("User ID not found in session. Please log in again.", "danger");
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            int currentUserId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT AccountId, BankName, AccountType, AccountNumber, AccountNickname, Balance, DateAdded FROM BankAccounts WHERE UserId = @UserId ORDER BY DateAdded DESC";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@UserId", currentUserId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvBankAccounts.DataSource = dt;
                gvBankAccounts.DataBind();
            }
        }

        private void ShowAlert(string message, string type)
        {
            string cssClass = type == "success" ? "alert alert-success" : "alert alert-danger"; // Changed 'error' to 'danger' for consistency with Bootstrap-like alerts
            AlertPanel.CssClass = cssClass;
            AlertMessage.Text = message;
            AlertPanel.Visible = true;

            if (type == "success")
            {
                AlertIcon.Attributes["class"] = "alert-icon fas fa-check-circle";
            }
            else
            {
                AlertIcon.Attributes["class"] = "alert-icon fas fa-times-circle";
            }

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

        protected void gvBankAccounts_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvBankAccounts.EditIndex = e.NewEditIndex;
            LoadBankAccounts();
        }

        protected void gvBankAccounts_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvBankAccounts.EditIndex = -1;
            LoadBankAccounts();
        }

        protected void gvBankAccounts_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int accountId = Convert.ToInt32(gvBankAccounts.DataKeys[e.RowIndex].Value);
            GridViewRow row = gvBankAccounts.Rows[e.RowIndex];

            string updatedBankName = ((TextBox)row.FindControl("txtBankNameEdit")).Text.Trim();
            string updatedAccountType = ((TextBox)row.FindControl("txtAccountTypeEdit")).Text.Trim();
            string updatedAccountNumber = ((TextBox)row.FindControl("txtAccountNumberEdit")).Text.Trim();
            string updatedAccountNickname = ((TextBox)row.FindControl("txtAccountNicknameEdit")).Text.Trim();
            decimal updatedBalance = Convert.ToDecimal(((TextBox)row.FindControl("txtBalanceEdit")).Text.Trim());

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE BankAccounts SET BankName = @BankName, AccountType = @AccountType,
                                   AccountNumber = @AccountNumber, AccountNickname = @AccountNickname,
                                   Balance = @Balance WHERE AccountId = @AccountId AND UserId = @UserId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@BankName", updatedBankName);
                    cmd.Parameters.AddWithValue("@AccountType", updatedAccountType);
                    cmd.Parameters.AddWithValue("@AccountNumber", updatedAccountNumber);
                    cmd.Parameters.AddWithValue("@AccountNickname", updatedAccountNickname);
                    cmd.Parameters.AddWithValue("@Balance", updatedBalance);
                    cmd.Parameters.AddWithValue("@AccountId", accountId);
                    cmd.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Account updated successfully!", "success");
                    }
                    else
                    {
                        ShowAlert("Failed to update account or account not found for your user.", "danger");
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    ShowAlert("An account with this number already exists. Please use a different account number.", "danger");
                }
                else
                {
                    ShowAlert("A database error occurred while updating the account: " + ex.Message, "danger");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating account: " + ex.Message, "danger");
            }

            gvBankAccounts.EditIndex = -1;
            LoadBankAccounts();
            LoadUserAccountsForOverview(); // Reload the overview to reflect changes
        }

        protected void gvBankAccounts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int accountId = Convert.ToInt32(gvBankAccounts.DataKeys[e.RowIndex].Value);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM BankAccounts WHERE AccountId = @AccountId AND UserId = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountId", accountId);
                    cmd.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Account deleted successfully!", "success");
                    }
                    else
                    {
                        ShowAlert("Failed to delete account or account not found for your user.", "danger");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error deleting account: " + ex.Message, "danger");
            }

            LoadBankAccounts();
            LoadUserAccountsForOverview(); // Reload the overview to reflect changes
        }

        protected void gvBankAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState.HasFlag(DataControlRowState.Edit))
            {
                TextBox txtBankNameEdit = new TextBox { ID = "txtBankNameEdit", CssClass = "gv-edit-input" };
                txtBankNameEdit.Text = DataBinder.Eval(e.Row.DataItem, "BankName").ToString();
                e.Row.Cells[0].Controls.Clear();
                e.Row.Cells[0].Controls.Add(txtBankNameEdit);

                TextBox txtAccountTypeEdit = new TextBox { ID = "txtAccountTypeEdit", CssClass = "gv-edit-input" };
                txtAccountTypeEdit.Text = DataBinder.Eval(e.Row.DataItem, "AccountType").ToString();
                e.Row.Cells[1].Controls.Clear();
                e.Row.Cells[1].Controls.Add(txtAccountTypeEdit);

                TextBox txtAccountNumberEdit = new TextBox { ID = "txtAccountNumberEdit", CssClass = "gv-edit-input" };
                txtAccountNumberEdit.Text = DataBinder.Eval(e.Row.DataItem, "AccountNumber").ToString();
                e.Row.Cells[2].Controls.Clear();
                e.Row.Cells[2].Controls.Add(txtAccountNumberEdit);

                TextBox txtAccountNicknameEdit = new TextBox { ID = "txtAccountNicknameEdit", CssClass = "gv-edit-input" };
                txtAccountNicknameEdit.Text = DataBinder.Eval(e.Row.DataItem, "AccountNickname").ToString();
                e.Row.Cells[3].Controls.Clear();
                e.Row.Cells[3].Controls.Add(txtAccountNicknameEdit);

                TextBox txtBalanceEdit = new TextBox { ID = "txtBalanceEdit", CssClass = "gv-edit-input", TextMode = TextBoxMode.Number };
                txtBalanceEdit.Text = DataBinder.Eval(e.Row.DataItem, "Balance").ToString();
                e.Row.Cells[4].Controls.Clear();
                e.Row.Cells[4].Controls.Add(txtBalanceEdit);
            }
        }
    }
}