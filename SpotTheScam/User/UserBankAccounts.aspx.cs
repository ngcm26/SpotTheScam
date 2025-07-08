using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace SpotTheScam.User
{
    public partial class UserBankAccounts : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/User/UserLogin.aspx");
                    return;
                }

                LoadBankAccounts();
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

        protected void btnAddAccount_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            if (Session["UserId"] == null)
            {
                ShowAlert("User ID not found in session. Please log in again.", "danger");
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            int currentUserId = Convert.ToInt32(Session["UserId"]);

            string bankName = txtBankName.Text.Trim();
            string accountType = txtAccountType.Text.Trim();
            string accountNumber = txtAccountNumber.Text.Trim();
            string accountNickname = txtAccountNickname.Text.Trim();
            decimal balance = Convert.ToDecimal(txtBalance.Text.Trim());

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string checkDuplicateQuery = "SELECT COUNT(*) FROM BankAccounts WHERE AccountNumber = @AccountNumber AND UserId = @UserId";
                    SqlCommand checkCmd = new SqlCommand(checkDuplicateQuery, conn);
                    checkCmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                    checkCmd.Parameters.AddWithValue("@UserId", currentUserId);
                    int existingAccounts = (int)checkCmd.ExecuteScalar();

                    if (existingAccounts > 0)
                    {
                        ShowAlert("An account with this number already exists for your user. Please use a different account number.", "danger");
                        return;
                    }

                    string query = @"INSERT INTO BankAccounts (UserId, BankName, AccountType, AccountNumber, AccountNickname, Balance)
                                   VALUES (@UserId, @BankName, @AccountType, @AccountNumber, @AccountNickname, @Balance)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@UserId", currentUserId);
                    cmd.Parameters.AddWithValue("@BankName", bankName);
                    cmd.Parameters.AddWithValue("@AccountType", accountType);
                    cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                    cmd.Parameters.AddWithValue("@AccountNickname", accountNickname);
                    cmd.Parameters.AddWithValue("@Balance", balance);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Bank account added successfully!", "success");
                        ClearForm();
                        LoadBankAccounts();
                    }
                    else
                    {
                        ShowAlert("Failed to add bank account. Please try again.", "danger");
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
                    ShowAlert("A database error occurred while adding the account: " + ex.Message, "danger");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("An unexpected error occurred while adding the account: " + ex.Message, "danger");
            }
        }

        private void ClearForm()
        {
            txtBankName.Text = string.Empty;
            txtAccountType.Text = "Savings";
            txtAccountNumber.Text = string.Empty;
            txtAccountNickname.Text = string.Empty;
            txtBalance.Text = string.Empty;
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = $"alert alert-{type}";
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
