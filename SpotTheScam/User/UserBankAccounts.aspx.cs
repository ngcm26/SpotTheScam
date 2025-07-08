using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class UserBankAccounts : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Username"] == null)
                {
                    Response.Redirect("~/User/UserLogin.aspx");
                }

                LoadBankAccounts();
            }
        }

        private void LoadBankAccounts()
        {
            if (Session["Username"] == null)
            {
                ShowAlert("You must be logged in to view bank accounts.", "error");
                return;
            }

            int currentUserId;
            if (Session["UserId"] != null)
            {
                currentUserId = Convert.ToInt32(Session["UserId"]);
            }
            else
            {
                ShowAlert("User ID not found in session. Please log in again.", "error");
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT AccountId, BankName, AccountType, AccountNumber, AccountNickname, Balance, DateAdded FROM BankAccounts WHERE UserId = @UserId";
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

            int currentUserId;
            if (Session["UserId"] != null)
            {
                currentUserId = Convert.ToInt32(Session["UserId"]);
            }
            else
            {
                ShowAlert("User ID not found in session. Please log in again.", "error");
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

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
                        ShowAlert("Failed to add bank account. Please try again.", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("An error occurred while adding the account: " + ex.Message, "error");
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
            string cssClass = type == "success" ? "alert alert-success" : "alert alert-danger";
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
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "HideAlert", script, true);
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

            string updatedBankName = ((TextBox)row.Cells[0].Controls[0]).Text.Trim();
            string updatedAccountType = ((TextBox)row.Cells[1].Controls[0]).Text.Trim();
            string updatedAccountNickname = ((TextBox)row.Cells[3].Controls[0]).Text.Trim();
            string updatedAccountNumber = ((TextBox)row.Cells[2].Controls[0]).Text.Trim();
            decimal updatedBalance = Convert.ToDecimal(((TextBox)row.Cells[4].Controls[0]).Text.Trim());

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE BankAccounts SET BankName = @BankName, AccountType = @AccountType,
                                   AccountNumber = @AccountNumber, AccountNickname = @AccountNickname,
                                   Balance = @Balance WHERE AccountId = @AccountId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@BankName", updatedBankName);
                    cmd.Parameters.AddWithValue("@AccountType", updatedAccountType);
                    cmd.Parameters.AddWithValue("@AccountNumber", updatedAccountNumber);
                    cmd.Parameters.AddWithValue("@AccountNickname", updatedAccountNickname);
                    cmd.Parameters.AddWithValue("@Balance", updatedBalance);
                    cmd.Parameters.AddWithValue("@AccountId", accountId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Account updated successfully!", "success");
                    }
                    else
                    {
                        ShowAlert("Failed to update account.", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error updating account: " + ex.Message, "error");
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
                    string query = "DELETE FROM BankAccounts WHERE AccountId = @AccountId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountId", accountId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Account deleted successfully!", "success");
                    }
                    else
                    {
                        ShowAlert("Failed to delete account.", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error deleting account: " + ex.Message, "error");
            }

            LoadBankAccounts();
        }
    }
}