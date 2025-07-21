using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Linq;

namespace SpotTheScam.Staff
{
    public partial class StaffExpertWebinar : System.Web.UI.Page
    {
        private string connectionString;

        public StaffExpertWebinar()
        {
            var connStr = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"];
            if (connStr != null)
            {
                connectionString = connStr.ConnectionString;
            }
            else
            {
                throw new ConfigurationErrorsException("Connection string 'SpotTheScamConnectionString' not found in web.config");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadOverviewData();
                LoadSessionFilters();
                LoadParticipantFilters();
            }
        }

        #region Tab Management
        protected void TabButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string tab = btn.CommandArgument;

            // Reset all tab buttons
            btnOverviewTab.CssClass = "tab-btn";
            btnSessionsTab.CssClass = "tab-btn";
            btnParticipantsTab.CssClass = "tab-btn";

            // Hide all panels
            pnlOverview.Visible = false;
            pnlSessions.Visible = false;
            pnlParticipants.Visible = false;

            // Show selected tab and panel
            switch (tab)
            {
                case "overview":
                    btnOverviewTab.CssClass = "tab-btn active";
                    pnlOverview.Visible = true;
                    LoadOverviewData();
                    break;
                case "sessions":
                    btnSessionsTab.CssClass = "tab-btn active";
                    pnlSessions.Visible = true;
                    LoadSessionDetails();
                    break;
                case "participants":
                    btnParticipantsTab.CssClass = "tab-btn active";
                    pnlParticipants.Visible = true;
                    LoadAllParticipants();
                    break;
            }
        }
        #endregion

        #region Overview Tab
        private void LoadOverviewData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get total sessions
                    string totalSessionsQuery = "SELECT COUNT(*) FROM WebinarSessions WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(totalSessionsQuery, conn))
                    {
                        ltTotalSessions.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Get total registrations
                    string totalRegistrationsQuery = "SELECT COUNT(*) FROM WebinarRegistrations WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(totalRegistrationsQuery, conn))
                    {
                        ltTotalRegistrations.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Get available spots
                    string availableSpotsQuery = @"
                        SELECT SUM(ws.MaxParticipants) - ISNULL(SUM(registrations.RegCount), 0) as AvailableSpots
                        FROM WebinarSessions ws
                        LEFT JOIN (
                            SELECT SessionId, COUNT(*) as RegCount 
                            FROM WebinarRegistrations 
                            WHERE IsActive = 1 
                            GROUP BY SessionId
                        ) registrations ON ws.SessionId = registrations.SessionId
                        WHERE ws.IsActive = 1 AND ws.SessionDate >= CAST(GETDATE() AS DATE)";
                    using (SqlCommand cmd = new SqlCommand(availableSpotsQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        ltAvailableSpots.Text = (result == DBNull.Value ? "0" : result.ToString());
                    }

                    // Get upcoming sessions
                    string upcomingSessionsQuery = "SELECT COUNT(*) FROM WebinarSessions WHERE IsActive = 1 AND SessionDate >= CAST(GETDATE() AS DATE)";
                    using (SqlCommand cmd = new SqlCommand(upcomingSessionsQuery, conn))
                    {
                        ltUpcomingSessions.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Load session overview
                    LoadSessionOverview();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading overview data: {ex.Message}");
            }
        }

        private void LoadSessionOverview()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            ws.SessionId,
                            ws.Title,
                            ws.SessionDate,
                            ws.StartTime,
                            ws.EndTime,
                            ws.MaxParticipants,
                            ws.PointsRequired,
                            ws.SessionType,
                            ISNULL(e.ExpertName, 'Expert') as ExpertName,
                            ISNULL(reg.RegCount, 0) as CurrentRegistrations
                        FROM WebinarSessions ws
                        LEFT JOIN Experts e ON ws.ExpertId = e.ExpertId
                        LEFT JOIN (
                            SELECT SessionId, COUNT(*) as RegCount 
                            FROM WebinarRegistrations 
                            WHERE IsActive = 1 
                            GROUP BY SessionId
                        ) reg ON ws.SessionId = reg.SessionId
                        WHERE ws.IsActive = 1
                        ORDER BY ws.SessionDate, ws.StartTime";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            rptSessionOverview.DataSource = dt;
                            rptSessionOverview.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading session overview: {ex.Message}");
            }
        }
        #endregion

        #region Sessions Tab
        private void LoadSessionFilters()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SessionId, Title FROM WebinarSessions WHERE IsActive = 1 ORDER BY SessionDate, StartTime";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlSessionFilter.Items.Clear();
                            ddlSessionFilter.Items.Add(new ListItem("Select a session...", ""));

                            while (reader.Read())
                            {
                                ddlSessionFilter.Items.Add(new ListItem(
                                    reader["Title"].ToString(),
                                    reader["SessionId"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading session filters: {ex.Message}");
            }
        }

        private void LoadSessionDetails()
        {
            if (string.IsNullOrEmpty(ddlSessionFilter.SelectedValue))
            {
                pnlSessionDetails.Visible = false;
                return;
            }

            pnlSessionDetails.Visible = true;
            LoadSelectedSessionInfo();
            LoadSessionParticipants();
        }

        private void LoadSelectedSessionInfo()
        {
            try
            {
                string sessionId = ddlSessionFilter.SelectedValue;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            ws.Title,
                            ws.SessionDate,
                            ws.StartTime,
                            ws.EndTime,
                            ws.MaxParticipants,
                            ISNULL(e.ExpertName, 'Expert') as ExpertName,
                            ISNULL(reg.RegCount, 0) as CurrentRegistrations
                        FROM WebinarSessions ws
                        LEFT JOIN Experts e ON ws.ExpertId = e.ExpertId
                        LEFT JOIN (
                            SELECT SessionId, COUNT(*) as RegCount 
                            FROM WebinarRegistrations 
                            WHERE IsActive = 1 AND SessionId = @SessionId
                            GROUP BY SessionId
                        ) reg ON ws.SessionId = reg.SessionId
                        WHERE ws.SessionId = @SessionId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ltSelectedSessionTitle.Text = reader["Title"].ToString();
                                DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                                ltSelectedSessionDateTime.Text = $"{sessionDate:MMMM dd, yyyy} at {reader["StartTime"]} - {reader["EndTime"]}";
                                ltSelectedSessionExpert.Text = reader["ExpertName"].ToString();
                                int maxParticipants = Convert.ToInt32(reader["MaxParticipants"]);
                                int currentRegistrations = Convert.ToInt32(reader["CurrentRegistrations"]);
                                ltSelectedSessionCapacity.Text = $"{maxParticipants} participants maximum";
                                ltSelectedSessionRegistrations.Text = $"{currentRegistrations} registered ({maxParticipants - currentRegistrations} spots remaining)";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading selected session info: {ex.Message}");
            }
        }

        private void LoadSessionParticipants()
        {
            try
            {
                string sessionId = ddlSessionFilter.SelectedValue;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            RegistrationId,
                            FirstName,
                            LastName,
                            Email,
                            Phone,
                            RegistrationDate,
                            ISNULL(AttendanceStatus, 'Registered') as AttendanceStatus,
                            SecurityConcerns,
                            BankingMethods
                        FROM WebinarRegistrations
                        WHERE SessionId = @SessionId AND IsActive = 1
                        ORDER BY RegistrationDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        conn.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            gvSessionParticipants.DataSource = dt;
                            gvSessionParticipants.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading session participants: {ex.Message}");
            }
        }

        protected void ddlSessionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSessionDetails();
        }

        protected void ddlAttendance_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddl = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddl.NamingContainer;
                int registrationId = Convert.ToInt32(gvSessionParticipants.DataKeys[row.RowIndex].Value);
                string newStatus = ddl.SelectedValue;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE WebinarRegistrations SET AttendanceStatus = @Status WHERE RegistrationId = @RegistrationId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@RegistrationId", registrationId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating attendance: {ex.Message}");
            }
        }

        protected void btnRemoveParticipant_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int registrationId = Convert.ToInt32(btn.CommandArgument);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE WebinarRegistrations SET IsActive = 0 WHERE RegistrationId = @RegistrationId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RegistrationId", registrationId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Reload the participants list
                LoadSessionParticipants();
                LoadSelectedSessionInfo(); // Update the count
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing participant: {ex.Message}");
            }
        }

        protected void btnExportSession_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlSessionFilter.SelectedValue))
            {
                return;
            }

            ExportToExcel("session", ddlSessionFilter.SelectedValue);
        }
        #endregion

        #region Participants Tab
        private void LoadParticipantFilters()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SessionId, Title FROM WebinarSessions WHERE IsActive = 1 ORDER BY SessionDate, StartTime";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlParticipantSessionFilter.Items.Clear();
                            ddlParticipantSessionFilter.Items.Add(new ListItem("All Sessions", ""));

                            while (reader.Read())
                            {
                                ddlParticipantSessionFilter.Items.Add(new ListItem(
                                    reader["Title"].ToString(),
                                    reader["SessionId"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading participant filters: {ex.Message}");
            }
        }

        private void LoadAllParticipants()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append(@"
                        SELECT 
                            wr.RegistrationId,
                            ws.Title as SessionTitle,
                            wr.FirstName,
                            wr.LastName,
                            wr.Email,
                            wr.Phone,
                            wr.RegistrationDate,
                            ISNULL(wr.AttendanceStatus, 'Registered') as AttendanceStatus,
                            wr.SecurityConcerns,
                            wr.BankingMethods
                        FROM WebinarRegistrations wr
                        INNER JOIN WebinarSessions ws ON wr.SessionId = ws.SessionId
                        WHERE wr.IsActive = 1");

                    // Add status filter
                    if (ddlStatusFilter.SelectedValue != "All")
                    {
                        queryBuilder.Append(" AND ISNULL(wr.AttendanceStatus, 'Registered') = @StatusFilter");
                    }

                    // Add session filter
                    if (!string.IsNullOrEmpty(ddlParticipantSessionFilter.SelectedValue))
                    {
                        queryBuilder.Append(" AND wr.SessionId = @SessionFilter");
                    }

                    queryBuilder.Append(" ORDER BY wr.RegistrationDate DESC");

                    using (SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), conn))
                    {
                        if (ddlStatusFilter.SelectedValue != "All")
                        {
                            cmd.Parameters.AddWithValue("@StatusFilter", ddlStatusFilter.SelectedValue);
                        }

                        if (!string.IsNullOrEmpty(ddlParticipantSessionFilter.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@SessionFilter", ddlParticipantSessionFilter.SelectedValue);
                        }

                        conn.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            gvAllParticipants.DataSource = dt;
                            gvAllParticipants.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading all participants: {ex.Message}");
            }
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAllParticipants();
        }

        protected void ddlParticipantSessionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAllParticipants();
        }

        protected void gvAllParticipants_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAllParticipants.PageIndex = e.NewPageIndex;
            LoadAllParticipants();
        }

        protected void btnExportAll_Click(object sender, EventArgs e)
        {
            ExportToExcel("all", "");
        }
        #endregion

        #region Helper Methods
        private void ExportToExcel(string exportType, string sessionId)
        {
            try
            {
                DataTable dt = GetExportData(exportType, sessionId);

                if (dt.Rows.Count == 0)
                {
                    return;
                }

                string fileName = $"WebinarParticipants_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");

                StringBuilder csv = new StringBuilder();

                // Add headers
                string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                csv.AppendLine(string.Join(",", columnNames.Select(name => $"\"{name}\"")));

                // Add data
                foreach (DataRow row in dt.Rows)
                {
                    string[] fields = row.ItemArray.Select(field => $"\"{field?.ToString().Replace("\"", "\"\"")}\"").ToArray();
                    csv.AppendLine(string.Join(",", fields));
                }

                Response.Write(csv.ToString());
                Response.End();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to Excel: {ex.Message}");
            }
        }

        private DataTable GetExportData(string exportType, string sessionId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "";

                    if (exportType == "session" && !string.IsNullOrEmpty(sessionId))
                    {
                        query = @"
                            SELECT 
                                ws.Title as 'Session Title',
                                wr.FirstName as 'First Name',
                                wr.LastName as 'Last Name',
                                wr.Email,
                                wr.Phone,
                                wr.SecurityConcerns as 'Security Concerns',
                                wr.BankingMethods as 'Banking Methods',
                                wr.RegistrationDate as 'Registration Date',
                                ISNULL(wr.AttendanceStatus, 'Registered') as 'Attendance Status'
                            FROM WebinarRegistrations wr
                            INNER JOIN WebinarSessions ws ON wr.SessionId = ws.SessionId
                            WHERE wr.SessionId = @SessionId AND wr.IsActive = 1
                            ORDER BY wr.RegistrationDate DESC";
                    }
                    else
                    {
                        query = @"
                            SELECT 
                                ws.Title as 'Session Title',
                                ws.SessionDate as 'Session Date',
                                wr.FirstName as 'First Name',
                                wr.LastName as 'Last Name',
                                wr.Email,
                                wr.Phone,
                                wr.SecurityConcerns as 'Security Concerns',
                                wr.BankingMethods as 'Banking Methods',
                                wr.RegistrationDate as 'Registration Date',
                                ISNULL(wr.AttendanceStatus, 'Registered') as 'Attendance Status'
                            FROM WebinarRegistrations wr
                            INNER JOIN WebinarSessions ws ON wr.SessionId = ws.SessionId
                            WHERE wr.IsActive = 1
                            ORDER BY ws.SessionDate, wr.RegistrationDate DESC";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (exportType == "session" && !string.IsNullOrEmpty(sessionId))
                        {
                            cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        }

                        conn.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting export data: {ex.Message}");
            }

            return dt;
        }
        #endregion
    }
}