using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

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
            // Top 3 patterns in last 7 days
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"
                WITH w AS (
                  SELECT TOP 3 scam_type, channel, COUNT(*) cnt
                  FROM dbo.ScanEvents
                  WHERE created_at >= DATEADD(DAY,-7,GETDATE())
                  GROUP BY scam_type, channel
                  ORDER BY COUNT(*) DESC
                )
                SELECT scam_type, channel, cnt FROM w ORDER BY cnt DESC;", conn))
            {
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
        }
    }
}
