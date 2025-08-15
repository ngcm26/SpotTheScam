using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SpotTheScam.Staff
{
    public partial class TrendAnalytics : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["StaffName"] == null)
                {
                    Response.Redirect("StaffLogin.aspx");
                    return;
                }
                BindAll();
            }
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            BindAll();
        }

        private void BindAll()
        {
            int days = 7;
            if (!int.TryParse(ddlRange.SelectedValue, out days)) days = 7;

            BindTopTypes(days);
            BindTopChannels(days);
            BindGaps(days);
        }

        private void BindTopTypes(int days)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"SELECT TOP 10 scam_type, COUNT(*) cnt FROM dbo.ScanEvents WHERE created_at >= DATEADD(DAY, -@d, GETDATE()) GROUP BY scam_type ORDER BY COUNT(*) DESC", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@d", days);
                var dt = new DataTable();
                da.Fill(dt);
                gvTopTypes.DataSource = dt;
                gvTopTypes.DataBind();
            }
        }

        private void BindTopChannels(int days)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"SELECT TOP 10 channel, COUNT(*) cnt FROM dbo.ScanEvents WHERE created_at >= DATEADD(DAY, -@d, GETDATE()) GROUP BY channel ORDER BY COUNT(*) DESC", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@d", days);
                var dt = new DataTable();
                da.Fill(dt);
                gvTopChannels.DataSource = dt;
                gvTopChannels.DataBind();
            }
        }

        private void BindGaps(int days)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"WITH recent AS (
    SELECT scam_type, COUNT(*) cnt
    FROM dbo.ScanEvents
    WHERE created_at >= DATEADD(DAY, -@d, GETDATE())
    GROUP BY scam_type
)
SELECT r.scam_type, r.cnt
FROM recent r
LEFT JOIN dbo.ScamTypeModule stm ON stm.scam_type = r.scam_type
WHERE stm.scam_type IS NULL
ORDER BY r.cnt DESC;", conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@d", days);
                var dt = new DataTable();
                da.Fill(dt);
                gvGaps.DataSource = dt;
                gvGaps.DataBind();
                lblGapsHint.Text = dt.Rows.Count == 0 ? "All trending scam types have module mappings. Great job!" : string.Empty;
            }
        }
    }
}


