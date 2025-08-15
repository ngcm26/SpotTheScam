using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SpotTheScam.Staff
{
    public partial class ManageUsers : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["StaffName"] == null)
                {
                    Response.Redirect("StaffDashboard.aspx");
                }
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Username, Email, PhoneNumber, Role, Status FROM Users WHERE Role = 'user'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUsers.DataSource = dt;
                gvUsers.DataBind();
            }
        }

        protected string GetRoleClass(string role)
        {
            switch (role.ToLower())
            {
                case "admin":
                    return "role-admin";
                case "staff":
                    return "role-staff";
                case "user":
                default:
                    return "role-user";
            }
        }

        protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int userId = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Users WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.ExecuteNonQuery();
            }

            LoadUsers();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Username, Email, PhoneNumber, Role, Status FROM Users WHERE Role = 'user'";
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND (Username LIKE @SearchTerm OR Email LIKE @SearchTerm OR PhoneNumber LIKE @SearchTerm OR Status LIKE @SearchTerm)";
                }
                
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    da.SelectCommand.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                }
                
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUsers.DataSource = dt;
                gvUsers.DataBind();
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUser")
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"EditUsers.aspx?user_id={userId}");
            }
            else if (e.CommandName == "ToggleStatus")
            {
                var parts = e.CommandArgument.ToString().Split('|');
                int userId = Convert.ToInt32(parts[0]);
                string current = parts.Length > 1 ? parts[1] : "active";
                string newStatus = current.Equals("active", StringComparison.OrdinalIgnoreCase) ? "suspended" : "active";
                UpdateUserStatus(userId, newStatus);
                LoadUsers();
            }
        }

        private void UpdateUserStatus(int userId, string newStatus)
        {
            string actorRole = (Session["StaffRole"] as string ?? "").ToLower();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string targetRole = "";
                using (SqlCommand rc = new SqlCommand("SELECT Role FROM Users WHERE Id=@Id", conn))
                {
                    rc.Parameters.AddWithValue("@Id", userId);
                    object r = rc.ExecuteScalar();
                    targetRole = r?.ToString().ToLower() ?? "";
                }
                if (targetRole == "staff" && actorRole != "admin") return;

                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET Status=@s WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@s", newStatus);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected string GetUserConfirmScript(object statusObj)
        {
            string status = statusObj == null ? string.Empty : statusObj.ToString();
            bool isActive = status.Equals("active", StringComparison.OrdinalIgnoreCase);
            string message = isActive ? "Do you want to suspend this user?" : "Do you want to activate this user?";
            return "return confirm('" + message.Replace("'", "\\'") + "');";
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadUsers();
        }
    }
}
