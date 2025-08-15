using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace SpotTheScam.Staff
{
    public partial class ManageStaff : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Admin-only
            if (Session["StaffName"] == null || !IsAdmin())
            {
                Response.Redirect("StaffDashboard.aspx");
                return;
            }
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private bool IsAdmin()
        {
            var role = Session["StaffRole"] as string;
            return !string.IsNullOrEmpty(role) && role.Equals("admin", StringComparison.OrdinalIgnoreCase);
        }

        private void BindGrid()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT Id, Username, Email, PhoneNumber, Status FROM Users WHERE Role = 'staff' ORDER BY Username", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                gvStaff.DataSource = dt;
                gvStaff.DataBind();
            }
        }

        protected void gvStaff_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditStaff")
            {
                Response.Redirect("EditUsers.aspx?user_id=" + e.CommandArgument);
            }
            else if (e.CommandName == "DeleteStaff")
            {
                DeleteStaff(Convert.ToInt32(e.CommandArgument));
                BindGrid();
            }
            else if (e.CommandName == "ToggleStatus")
            {
                var parts = e.CommandArgument.ToString().Split('|');
                int userId = Convert.ToInt32(parts[0]);
                string current = parts.Length > 1 ? parts[1] : "active";
                string newStatus = current.Equals("active", StringComparison.OrdinalIgnoreCase) ? "suspended" : "active";
                UpdateStaffStatus(userId, newStatus);
                BindGrid();
            }
        }

        private void DeleteStaff(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("DELETE FROM Users WHERE Id = @Id AND Role = 'staff'", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateStaffStatus(int id, string status)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("UPDATE Users SET Status=@s WHERE Id=@Id AND Role='staff'", conn))
            {
                cmd.Parameters.AddWithValue("@s", status);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected string GetStaffConfirmScript(object statusObj)
        {
            string status = statusObj == null ? "" : statusObj.ToString();
            bool isActive = status.Equals("active", StringComparison.OrdinalIgnoreCase);
            string message = isActive ? "Do you want to suspend this staff account?" : "Do you want to activate this staff account?";
            return "return confirm('" + message.Replace("'", "\\'") + "');";
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"SELECT Id, Username, Email, PhoneNumber, Status
                                              FROM Users
                                              WHERE Role = 'staff' AND (
                                                    @search = '' OR Username LIKE @like OR Email LIKE @like OR PhoneNumber LIKE @like OR Status LIKE @like
                                              )
                                              ORDER BY Username", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@search", searchTerm);
                cmd.Parameters.AddWithValue("@like", "%" + searchTerm + "%");
                var dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                gvStaff.DataSource = dt;
                gvStaff.DataBind();
            }
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            BindGrid();
        }
    }
}