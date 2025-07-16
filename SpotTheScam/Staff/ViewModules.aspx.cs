using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.Staff
{
    public partial class ViewModules : System.Web.UI.Page
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["StaffName"] == null)
                {
                    Response.Redirect("StaffDashboard.aspx");
                    return;
                }
                LoadModules();
            }
        }

        private void LoadModules(string search = "", string status = "", string dateSort = "desc")
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT module_id, module_name, cover_image, author, status, date_created, date_updated FROM Modules WHERE 1=1";
                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND module_name LIKE @search";
                }
                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND status = @status";
                }
                if (dateSort == "asc")
                {
                    query += " ORDER BY date_created ASC";
                }
                else
                {
                    query += " ORDER BY date_created DESC";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(search))
                        cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                    if (!string.IsNullOrEmpty(status))
                        cmd.Parameters.AddWithValue("@status", status);

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            gvModules.DataSource = dt;
            gvModules.DataBind();
        }

        protected void btnAddModule_Click(object sender, EventArgs e)
        {
            Response.Redirect("StaffAddModules.aspx");
        }

        protected void gvModules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditModule")
            {
                string moduleId = e.CommandArgument.ToString();
                Response.Redirect($"EditModule.aspx?module_id={moduleId}");
            }
            else if (e.CommandName == "DeleteModule")
            {
                string moduleId = e.CommandArgument.ToString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Only delete from Modules — ModuleInformation will be auto-deleted (ON DELETE CASCADE)
                    string deleteModule = "DELETE FROM Modules WHERE module_id = @module_id";
                    using (SqlCommand cmd = new SqlCommand(deleteModule, conn))
                    {
                        cmd.Parameters.AddWithValue("@module_id", moduleId);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadModules();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            string status = ddlStatus.SelectedValue;
            string dateSort = ddlDateSort.SelectedValue;
            LoadModules(search, status, dateSort);
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlStatus.SelectedIndex = 0;
            ddlDateSort.SelectedIndex = 0;
            LoadModules();
        }
    }
}
