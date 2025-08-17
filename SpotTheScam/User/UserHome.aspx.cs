using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;

namespace SpotTheScam
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected class TrendItem { public string Label { get; set; } public int Count { get; set; } }
        protected List<TrendItem> topTrends = new List<TrendItem>();
        protected string chartLabelsJson = "[]";
        protected string chartDataJson = "[]";
        protected string suggestedUrl = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Username"] != null)
                {
                    // User is logged in
                    phWelcome.Visible = true;
                    lblName.Text = Session["Username"].ToString();

                    phHero.Visible = false;
                    LoadUserCards();
                }
                else
                {
                    // User is NOT logged in
                    phHero.Visible = true;
                    phWelcome.Visible = false;
                }

                LoadTrendRadar();
            }
        }

        private void LoadTrendRadar()
        {
            int days = 7;
            if (ddUserTrendRange != null && !string.IsNullOrEmpty(ddUserTrendRange.SelectedValue))
            {
                int.TryParse(ddUserTrendRange.SelectedValue, out days);
                if (days <= 0) days = 7;
            }

            // Top patterns in selected range
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"
                WITH w AS (
                  SELECT TOP 5 scam_type, channel, COUNT(*) cnt
                  FROM dbo.ScanEvents
                  WHERE CreatedAt >= DATEADD(DAY,-@d,GETUTCDATE())
                  GROUP BY scam_type, channel
                  ORDER BY COUNT(*) DESC
                )
                SELECT scam_type, channel, cnt FROM w ORDER BY cnt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@d", days);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        string label = string.Format("{0} via {1}", r["scam_type"], r["channel"]);
                        int count = Convert.ToInt32(r["cnt"]);
                        topTrends.Add(new TrendItem { Label = label, Count = count });
                    }
                }
            }

            // Serialize for Chart.js
            chartLabelsJson = Newtonsoft.Json.JsonConvert.SerializeObject(topTrends.Select(t => t.Label));
            chartDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(topTrends.Select(t => t.Count));

            // Optional: Recommend a module via ScamTypeModule
            if (topTrends.Count > 0)
            {
                string topType = topTrends[0].Label.Split(new[] { " via " }, StringSplitOptions.None)[0];
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 m.module_id
                    FROM dbo.ScamTypeModule stm
                    JOIN dbo.Modules m ON m.module_id = stm.module_id AND m.status='published'
                    WHERE stm.scam_type = @t
                    ORDER BY m.date_updated DESC;", conn))
                {
                    cmd.Parameters.AddWithValue("@t", topType);
                    conn.Open();
                    object modId = cmd.ExecuteScalar();
                    if (modId != null)
                    {
                        suggestedUrl = "~/User/ModuleInformation.aspx?module_id=" + modId.ToString();
                    }
                }
            }

            // Bind top scam types table
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"SELECT TOP 5 scam_type, COUNT(*) cnt 
FROM dbo.ScanEvents 
WHERE CreatedAt >= DATEADD(DAY,-@d,GETUTCDATE())
GROUP BY scam_type
ORDER BY COUNT(*) DESC;", conn))
            using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@d", days);
                var dt = new System.Data.DataTable();
                da.Fill(dt);
                if (gvTopScamTypes != null) { gvTopScamTypes.DataSource = dt; gvTopScamTypes.DataBind(); }
            }
        }

        protected void ddUserTrendRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            topTrends.Clear();
            LoadTrendRadar();
        }

        protected void gvLeaderboard_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                int rank = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Rank"));
                if (rank == 1) e.Row.CssClass = "medal-gold";
                else if (rank == 2) e.Row.CssClass = "medal-silver";
                else if (rank == 3) e.Row.CssClass = "medal-bronze";
            }
        }

        protected void gvMySessions_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                var btn = (System.Web.UI.WebControls.LinkButton)e.Row.FindControl("btnJoin");
                DateTime date = Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "SessionDate"));
                bool isTodayOrFuture = date.Date >= DateTime.Today;
                if (btn != null)
                {
                    btn.Enabled = isTodayOrFuture;
                    btn.CssClass = isTodayOrFuture ? "btn" : "btn disabled";
                    if (!isTodayOrFuture) btn.ForeColor = System.Drawing.Color.Gray;
                }
            }
        }

        protected void gvMySessions_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "join")
            {
                int id;
                if (int.TryParse(Convert.ToString(e.CommandArgument), out id))
                {
                    Response.Redirect("UserMySessions.aspx?sessionId=" + id);
                }
            }
        }

        protected void gvRecSessions_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                var link = (System.Web.UI.WebControls.HyperLink)e.Row.FindControl("lnkRegister");
                int id = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Id"));
                if (link != null)
                {
                    link.NavigateUrl = ResolveUrl("~/User/UserWebinarRegistration.aspx?sessionId=" + id);
                }
            }
        }

        private void LoadUserCards()
        {
            try
            {
                int userId = 0;
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Resolve user id: prefer session, then username, then email
                    if (Session["UserID"] != null)
                    {
                        int.TryParse(Session["UserID"].ToString(), out userId);
                    }
                    if (userId == 0)
                    {
                        using (var getId = new SqlCommand("SELECT Id FROM dbo.Users WHERE Username=@u", conn))
                        {
                            getId.Parameters.AddWithValue("@u", Session["Username"] ?? (object)DBNull.Value);
                            object id = getId.ExecuteScalar();
                            if (id != null) userId = Convert.ToInt32(id);
                        }
                    }
                    if (userId == 0 && Session["Email"] != null)
                    {
                        using (var getIdByEmail = new SqlCommand("SELECT Id FROM dbo.Users WHERE Email=@e", conn))
                        {
                            getIdByEmail.Parameters.AddWithValue("@e", Session["Email"]);
                            object id2 = getIdByEmail.ExecuteScalar();
                            if (id2 != null) userId = Convert.ToInt32(id2);
                        }
                    }

                    // Current points
                    using (var cmd = new SqlCommand("SELECT ISNULL(SUM(Points),0) FROM dbo.PointsTransactions WHERE UserId=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        int pts = Convert.ToInt32(cmd.ExecuteScalar());
                        lblCurrentPoints.Text = pts.ToString();
                    }

                    // My sessions (both tables) -> bind to GridView
                    if (userId > 0)
                    {
                        // Video call bookings
                        var dtMy = new System.Data.DataTable();
                        using (var cmd = new SqlCommand(@"SELECT es.Id, es.SessionTitle, es.SessionDate, CONVERT(varchar, es.StartTime, 108) as StartTime
FROM dbo.ExpertSessions es
WHERE es.Id IN (SELECT SessionId FROM dbo.VideoCallBookings WHERE UserId=@id)
AND es.SessionDate >= CAST(GETDATE() AS date)
ORDER BY es.SessionDate ASC, es.StartTime ASC;", conn))
                        using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            da.Fill(dtMy);
                        }
                        // Try add webinar registrations if table exists
                        try
                        {
                            using (var cmd2 = new SqlCommand(@"SELECT es.Id, es.SessionTitle, es.SessionDate, CONVERT(varchar, es.StartTime, 108) as StartTime
FROM dbo.ExpertSessions es
WHERE es.Id IN (SELECT SessionId FROM dbo.WebinarRegistrations WHERE UserId=@id AND IsActive=1)
AND es.SessionDate >= CAST(GETDATE() AS date)
ORDER BY es.SessionDate ASC, es.StartTime ASC;", conn))
                            using (var da2 = new System.Data.SqlClient.SqlDataAdapter(cmd2))
                            {
                                cmd2.Parameters.AddWithValue("@id", userId);
                                da2.Fill(dtMy);
                            }
                        }
                        catch { /* table may not exist; ignore */ }

                        gvMySessions.DataSource = dtMy;
                        gvMySessions.DataBind();
                    }
                    else
                    {
                        gvMySessions.DataSource = null; gvMySessions.DataBind();
                    }

                    // Recommended sessions (available soon) -> bind to GridView
                    using (var cmd = new SqlCommand(@"SELECT TOP 5 es.Id, es.SessionTitle, es.SessionDate, CONVERT(varchar, es.StartTime, 108) as StartTime
FROM dbo.ExpertSessions es
WHERE es.Status='Available' AND es.SessionDate >= CAST(GETDATE() AS date)
ORDER BY es.SessionDate ASC, es.StartTime ASC;", conn))
                    using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        var dt = new System.Data.DataTable();
                        da.Fill(dt);
                        gvRecSessions.DataSource = dt;
                        gvRecSessions.DataBind();
                    }

                    // Leaderboard (top by total points) with rank
                    using (var cmd = new SqlCommand(@"WITH totals AS (
    SELECT u.Username, SUM(pt.Points) AS total_points
    FROM dbo.PointsTransactions pt
    JOIN dbo.Users u ON u.Id = pt.UserId
    GROUP BY u.Username
), ranked AS (
    SELECT ROW_NUMBER() OVER (ORDER BY total_points DESC) AS Rank, Username, total_points
    FROM totals
)
SELECT TOP 10 Rank, Username, total_points FROM ranked ORDER BY Rank;", conn))
                    using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        var dt = new System.Data.DataTable();
                        da.Fill(dt);
                        gvLeaderboard.DataSource = dt;
                        gvLeaderboard.DataBind();
                    }

                    // New modules (recent published) -> bind to GridView
                    using (var cmd = new SqlCommand(@"SELECT TOP 10 
    module_id AS ModuleId,
    module_name AS ModuleName,
    date_created AS DateAdded
FROM dbo.Modules
WHERE status = 'published'
ORDER BY date_created DESC;", conn))
                    using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        var dt = new System.Data.DataTable();
                        da.Fill(dt);
                        gvNewModules.DataSource = dt;
                        gvNewModules.DataBind();
                    }
                }
            }
            catch
            {
                // non-blocking
            }
        }
    }
}
