using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace SpotTheScam.User
{
    public partial class FamilyOverview : Page
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                int? groupId = GetCurrentGroupId();
                pnlNoGroupSelected.Visible = groupId == null;
                pnlHasGroup.Visible = groupId != null;

                if (groupId != null)
                {
                    BindKpis(groupId.Value);
                    BindRiskFeed(groupId.Value);
                    BindApprovals(groupId.Value);
                    BindAccounts(groupId.Value);
                    BindSafety(groupId.Value);
                }
            }
        }

        private int? GetCurrentGroupId()
        {
            object o = Session["CurrentGroupId"];
            if (o == null || o == DBNull.Value) return null;
            int parsed;
            return int.TryParse(o.ToString(), out parsed) ? (int?)parsed : null;
        }

        // ---- KPIs ----

        private void BindKpis(int groupId)
        {
            litKpiRed24h.Text = GetRedAlerts24h(groupId).ToString();
            litKpiPending.Text = GetPendingApprovals(groupId).ToString();

            decimal outflow7 = GetOutflow(groupId, 7, false);
            decimal outflowPrev7 = GetOutflow(groupId, 7, true);
            litKpiOutflow7d.Text = "$" + outflow7.ToString("N2");

            string deltaText;
            if (outflowPrev7 <= 0)
                deltaText = "—";
            else
            {
                decimal deltaPct = (outflow7 - outflowPrev7) / outflowPrev7 * 100m;
                deltaText = (deltaPct >= 0 ? "↑ " : "↓ ") + Math.Abs(deltaPct).ToString("0.#") + "%";
            }
            litKpiOutflowDelta.Text = deltaText;

            litKpiNewPayees7d.Text = GetNewPayees7d(groupId).ToString();
        }

        private int GetRedAlerts24h(int groupId)
        {
            const string sql = @"
SELECT COUNT(*) 
FROM BankTransactions t
JOIN FamilyGroupMembers m ON m.UserId = t.UserId AND m.GroupId = @GroupId AND m.GroupRole = 'Primary' AND m.Status='Active'
WHERE ((t.IsHeld = 1) OR (t.IsFlagged = 1 AND t.Severity = 'Red'))
  AND (CAST(t.TransactionDate AS DATETIME) + CAST(t.TransactionTime AS DATETIME)) >= DATEADD(HOUR, -24, GETDATE());";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@GroupId", groupId);
                conn.Open();
                object o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
            }
        }

        private int GetPendingApprovals(int groupId)
        {
            const string sql = @"
SELECT COUNT(*)
FROM BankTransactions t
JOIN FamilyGroupMembers m ON m.UserId = t.UserId AND m.GroupId = @GroupId AND m.GroupRole = 'Primary' AND m.Status='Active'
WHERE t.IsHeld = 1 AND (t.ReviewStatus IS NULL OR t.ReviewStatus = 'Pending');";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@GroupId", groupId);
                conn.Open();
                object o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
            }
        }

        private decimal GetOutflow(int groupId, int days, bool previousWindow)
        {
            // Uses TransactionType='Withdrawal' as outflow
            string sql = @"
SELECT SUM(t.Amount)
FROM BankTransactions t
JOIN FamilyGroupMembers m ON m.UserId = t.UserId AND m.GroupId = @GroupId AND m.GroupRole = 'Primary' AND m.Status='Active'
WHERE t.TransactionType = 'Withdrawal'
  AND t.TransactionDate >= @Start
  AND t.TransactionDate <  @End;";

            DateTime today = DateTime.Today;
            DateTime end = today.AddDays(previousWindow ? -7 : 1);
            DateTime start = end.AddDays(-days);

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@GroupId", groupId);
                cmd.Parameters.AddWithValue("@Start", start);
                cmd.Parameters.AddWithValue("@End", end);
                conn.Open();
                object o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? 0m : Convert.ToDecimal(o);
            }
        }

        private int GetNewPayees7d(int groupId)
        {
            const string sql = @"
WITH FirstSeen AS (
   SELECT t.SenderRecipient, MIN(t.TransactionDate) AS FirstDate
   FROM BankTransactions t
   JOIN FamilyGroupMembers m ON m.UserId = t.UserId AND m.GroupId = @GroupId AND m.GroupRole='Primary' AND m.Status='Active'
   WHERE t.SenderRecipient IS NOT NULL AND LTRIM(RTRIM(t.SenderRecipient)) <> ''
   GROUP BY t.SenderRecipient
)
SELECT COUNT(*) FROM FirstSeen WHERE FirstDate >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE));";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@GroupId", groupId);
                conn.Open();
                object o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
            }
        }

        // ---- Lists ----

        private void BindRiskFeed(int groupId)
        {
            const string sql = @"
SELECT TOP 8
  t.TransactionId, t.TransactionDate, t.TransactionTime, t.Amount, t.Severity, t.Description, t.SenderRecipient, t.IsHeld,
  a.AccountNickname, a.AccountNumberMasked,
  u.Username AS MemberName
FROM BankTransactions t
JOIN FamilyGroupMembers m ON m.UserId = t.UserId AND m.GroupId = @GroupId AND m.GroupRole='Primary' AND m.Status='Active'
JOIN BankAccounts a ON a.AccountId = t.AccountId
JOIN Users u ON u.Id = t.UserId
WHERE (t.IsHeld = 1) OR (t.IsFlagged = 1)
ORDER BY t.TransactionDate DESC, t.TransactionTime DESC;";

            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@GroupId", groupId);
                var dt = new DataTable();
                da.Fill(dt);
                rptRiskFeed.DataSource = dt;
                rptRiskFeed.DataBind();
            }
        }

        private void BindApprovals(int groupId)
        {
            const string sql = @"
SELECT TOP 5
  t.TransactionId, t.TransactionDate, t.TransactionTime, t.Amount, t.SenderRecipient, t.ReviewStatus,
  a.AccountNickname, a.AccountNumberMasked,
  u.Username AS MemberName
FROM BankTransactions t
JOIN FamilyGroupMembers m ON m.UserId = t.UserId AND m.GroupId = @GroupId AND m.GroupRole='Primary' AND m.Status='Active'
JOIN BankAccounts a ON a.AccountId = t.AccountId
JOIN Users u ON u.Id = t.UserId
WHERE t.IsHeld = 1 AND (t.ReviewStatus IS NULL OR t.ReviewStatus = 'Pending')
ORDER BY t.TransactionDate DESC, t.TransactionTime DESC;";

            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@GroupId", groupId);
                var dt = new DataTable();
                da.Fill(dt);
                rptApprovals.DataSource = dt;
                rptApprovals.DataBind();
            }
        }

        private void BindAccounts(int groupId)
        {
            const string sql = @"
SELECT 
    a.AccountId,
    a.BankName,                -- add this
    a.AccountNickname,
    a.AccountNumberMasked,
    a.Balance,
    u.Username AS MemberName
FROM BankAccounts a
JOIN FamilyGroupMembers m 
      ON m.UserId = a.UserId
     AND m.GroupId = @GroupId
     AND m.GroupRole = 'Primary'
     AND m.Status = 'Active'
JOIN Users u 
      ON u.Id = a.UserId
ORDER BY u.Username, a.AccountNickname;";

            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@GroupId", groupId);
                var dt = new DataTable();
                da.Fill(dt);
                rptAccounts.DataSource = dt;
                rptAccounts.DataBind();
            }
        }


        private void BindSafety(int groupId)
        {
            const string sql = @"
SELECT
   r.PrimaryUserId,
   u.Username AS MemberName,
   r.SingleTransactionLimit,
   r.MaxDailyTransfer,
   CASE 
     WHEN r.AllowedStartHour IS NULL OR r.AllowedEndHour IS NULL THEN 'Not set'
     ELSE RIGHT('0'+CAST(r.AllowedStartHour AS VARCHAR(2)),2) + ':00 - ' + RIGHT('0'+CAST(r.AllowedEndHour AS VARCHAR(2)),2) + ':00'
   END AS TimeWindow
FROM FamilyGroupMemberRestrictions r
JOIN Users u ON u.Id = r.PrimaryUserId
WHERE r.GroupId = @GroupId
ORDER BY u.Username;";

            using (var conn = new SqlConnection(_cs))
            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@GroupId", groupId);
                var dt = new DataTable();
                da.Fill(dt);
                rptSafety.DataSource = dt;
                rptSafety.DataBind();
            }
        }
    }
}
