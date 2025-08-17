using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SpotTheScam.Staff
{
    public partial class StaffDashboard : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["StaffName"] == null)
            {
                // Not logged in → show access denied placeholder
                phDashboard.Visible = false;
                phPleaseLogin.Visible = true;
            }
            else
            {
                // Logged in → show dashboard
                lblStaffName.Text = Session["StaffName"].ToString();
                phDashboard.Visible = true;
                phPleaseLogin.Visible = false;

                if (!IsPostBack)
                {
                    BindAll();
                }
            }
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            if (phDashboard.Visible)
            {
                BindAll();
            }
        }

        private void BindAll()
        {
            int days = 7;
            if (!int.TryParse(ddlRange.SelectedValue, out days)) days = 7;

            BindTopTypes(days);
            BindTopChannels(days);
            BindGaps(days);
            BindKpis(days);
        }

        private void BindTopTypes(int days)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"SELECT TOP 10 scam_type, COUNT(*) cnt FROM dbo.ScanEvents WHERE CreatedAt >= DATEADD(DAY, -@d, GETUTCDATE()) GROUP BY scam_type ORDER BY COUNT(*) DESC", conn))
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
            using (var cmd = new SqlCommand(@"SELECT TOP 10 channel, COUNT(*) cnt FROM dbo.ScanEvents WHERE CreatedAt >= DATEADD(DAY, -@d, GETUTCDATE()) GROUP BY channel ORDER BY COUNT(*) DESC", conn))
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
    WHERE CreatedAt >= DATEADD(DAY, -@d, GETUTCDATE())
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

        private void BindKpis(int days)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Blogs: total | approved | pending
                    using (var cmd = new SqlCommand(@"SELECT 
    COUNT(*) AS total,
    SUM(CASE WHEN isApproved = 1 THEN 1 ELSE 0 END) AS approved,
    SUM(CASE WHEN isApproved = 0 THEN 1 ELSE 0 END) AS pending
FROM dbo.posts;", conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            int total = Convert.ToInt32(r["total"]);
                            int approved = r["approved"] == DBNull.Value ? 0 : Convert.ToInt32(r["approved"]);
                            int pending = r["pending"] == DBNull.Value ? 0 : Convert.ToInt32(r["pending"]);
                            lblBlogs.Text = string.Format("{0} total • {1} approved • {2} pending", total, approved, pending);
                        }
                    }

                    // Scans in range
                    using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.ScanEvents WHERE CreatedAt >= DATEADD(DAY,-@d,GETUTCDATE());", conn))
                    {
                        cmd.Parameters.AddWithValue("@d", days);
                        int scans = Convert.ToInt32(cmd.ExecuteScalar());
                        lblScans.Text = scans.ToString();
                    }

                    // Quiz attempts and unique users in range
                    using (var cmd = new SqlCommand(@"SELECT COUNT(*) AS attempts, COUNT(DISTINCT UserID) AS users
FROM dbo.UserQuizResults
WHERE CompletedAt >= DATEADD(DAY,-@d,GETUTCDATE());", conn))
                    {
                        cmd.Parameters.AddWithValue("@d", days);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                int attempts = Convert.ToInt32(r["attempts"]);
                                int users = Convert.ToInt32(r["users"]);
                                lblQuiz.Text = string.Format("{0} attempts • {1} users", attempts, users);
                            }
                        }
                    }

                    // Upcoming sessions count
                    using (var cmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.ExpertSessions WHERE SessionDate >= CAST(GETDATE() AS date) AND Status = 'Available';", conn))
                    {
                        int upcoming = Convert.ToInt32(cmd.ExecuteScalar());
                        lblUpcomingSessions.Text = upcoming.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
    }
}
