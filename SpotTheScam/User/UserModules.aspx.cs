using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SpotTheScam.User
{
    public partial class UserModules : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPublishedModules();
            }
        }

        private void LoadPublishedModules()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT module_id, module_name, cover_image FROM Modules WHERE status = 'published'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    rptModules.DataSource = dt;
                    rptModules.DataBind();
                }
                else
                {
                    lblMessage.Text = "No published modules available.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }
    }
}
