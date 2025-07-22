using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class UserWebinarSessionListing : System.Web.UI.Page
    {
        private string connectionString;

        public UserWebinarSessionListing()
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
                LoadUserCurrentPoints();
                LoadSessions();
                ShowAllSessions();
            }
        }

        private void LoadUserCurrentPoints()
        {
            try
            {
                // Check if user is logged in
                if (Session["Username"] == null || string.IsNullOrEmpty(Session["Username"].ToString()))
                {
                    Response.Redirect("UserLogin.aspx");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"✅ Webinar page - Loading points for user: {Session["Username"]}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from Username
                    string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";
                    int userId = 0;

                    using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                        object userIdResult = cmd.ExecuteScalar();
                        if (userIdResult != null)
                        {
                            userId = Convert.ToInt32(userIdResult);
                            Session["UserID"] = userId;
                        }
                        else
                        {
                            lblCurrentPoints.Text = "0";
                            return;
                        }
                    }

                    // Get current total points from PointsTransactions table
                    string getPointsQuery = @"
                        SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                        FROM PointsTransactions 
                        WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(getPointsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        int currentUserPoints = result != null ? Convert.ToInt32(result) : 0;
                        Session["CurrentPoints"] = currentUserPoints;
                        lblCurrentPoints.Text = currentUserPoints.ToString();

                        System.Diagnostics.Debug.WriteLine($"✅ Webinar page - Current points loaded: {currentUserPoints}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading current points in webinar page: {ex.Message}");
                lblCurrentPoints.Text = "0";
            }
        }

        private void LoadSessions()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("📋 Loading sessions from database...");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First, check if ExpertSessions table exists and has data
                    string checkQuery = "SELECT COUNT(*) FROM ExpertSessions WHERE Status = 'Available'";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        int sessionCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        System.Diagnostics.Debug.WriteLine($"📊 Found {sessionCount} available sessions in ExpertSessions table");

                        if (sessionCount == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("⚠️ No sessions in database, creating sample sessions...");
                            CreateSampleSessions(conn);
                        }
                    }

                    // Now load the sessions
                    string query = @"
                        SELECT 
                            es.Id as SessionId,
                            es.SessionTitle as Title,
                            es.SessionDescription as Description,
                            es.SessionDate,
                            es.StartTime,
                            es.EndTime,
                            es.MaxParticipants,
                            es.PointsCost as PointsRequired,
                            es.SessionType,
                            'General' as Category,
                            ISNULL(es.ExpertName, 'Expert') as ExpertName,
                            ISNULL(es.ExpertTitle, 'Specialist') as ExpertTitle,
                            ISNULL(es.CurrentParticipants, 0) as CurrentRegistrations,
                            (es.MaxParticipants - ISNULL(es.CurrentParticipants, 0)) as AvailableSpots
                        FROM ExpertSessions es
                        WHERE es.Status = 'Available' AND es.SessionDate >= CAST(GETDATE() AS DATE)
                        ORDER BY es.SessionDate, es.StartTime";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"✅ Loaded {dt.Rows.Count} sessions from ExpertSessions table");

                                // Store in ViewState for filtering
                                ViewState["SessionsData"] = dt;

                                // Bind to repeater
                                rptSessions.DataSource = dt;
                                rptSessions.DataBind();
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("⚠️ No sessions found, loading fallback data");
                                LoadFallbackSessions();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading sessions: {ex.Message}");
                System.Diagnostics.Debug.WriteLine("📦 Loading fallback sessions instead");
                LoadFallbackSessions();
            }
        }

        // REPLACE your CreateSampleSessions method with this COMPLETE fix:

        private void CreateSampleSessions(SqlConnection conn)
        {
            try
            {
                // Get the current user ID for CreatedBy field
                int createdByUserId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 1;

                // Insert sessions one by one to handle any individual errors

                // Session 1 - Free Banking Security Session
                string insertSession1 = @"
            INSERT INTO ExpertSessions (
                SessionDate, StartTime, EndTime, SessionType, SessionTitle, SessionDescription, 
                ExpertName, ExpertTitle, MaxParticipants, CurrentParticipants, PointsCost, 
                Status, SessionTopic, CreatedBy, CreatedDate
            ) VALUES (
                @SessionDate, @StartTime, @EndTime, @SessionType, @SessionTitle, @SessionDescription,
                @ExpertName, @ExpertTitle, @MaxParticipants, @CurrentParticipants, @PointsCost,
                @Status, @SessionTopic, @CreatedBy, @CreatedDate
            )";

                using (SqlCommand cmd = new SqlCommand(insertSession1, conn))
                {
                    cmd.Parameters.AddWithValue("@SessionDate", DateTime.Today.AddDays(7));
                    cmd.Parameters.AddWithValue("@StartTime", TimeSpan.Parse("14:00:00")); // 2:00 PM
                    cmd.Parameters.AddWithValue("@EndTime", TimeSpan.Parse("15:00:00"));   // 3:00 PM
                    cmd.Parameters.AddWithValue("@SessionType", "Free");
                    cmd.Parameters.AddWithValue("@SessionTitle", "Protecting Your Online Banking");
                    cmd.Parameters.AddWithValue("@SessionDescription", "Learn essential security practices for online banking, how to spot fraudulent websites, and what to do if your account is compromised.");
                    cmd.Parameters.AddWithValue("@ExpertName", "Dr Harvey Blue");
                    cmd.Parameters.AddWithValue("@ExpertTitle", "Cybersecurity Specialist, 15+ years experience");
                    cmd.Parameters.AddWithValue("@MaxParticipants", 100);
                    cmd.Parameters.AddWithValue("@CurrentParticipants", 0);
                    cmd.Parameters.AddWithValue("@PointsCost", 0);
                    cmd.Parameters.AddWithValue("@Status", "Available");
                    cmd.Parameters.AddWithValue("@SessionTopic", "Banking");
                    cmd.Parameters.AddWithValue("@CreatedBy", createdByUserId);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("✅ Created Session 1: Banking Security (Free)");
                }

                // Session 2 - Premium Phone Scams Session
                string insertSession2 = @"
            INSERT INTO ExpertSessions (
                SessionDate, StartTime, EndTime, SessionType, SessionTitle, SessionDescription, 
                ExpertName, ExpertTitle, MaxParticipants, CurrentParticipants, PointsCost, 
                Status, SessionTopic, CreatedBy, CreatedDate
            ) VALUES (
                @SessionDate, @StartTime, @EndTime, @SessionType, @SessionTitle, @SessionDescription,
                @ExpertName, @ExpertTitle, @MaxParticipants, @CurrentParticipants, @PointsCost,
                @Status, @SessionTopic, @CreatedBy, @CreatedDate
            )";

                using (SqlCommand cmd = new SqlCommand(insertSession2, conn))
                {
                    cmd.Parameters.AddWithValue("@SessionDate", DateTime.Today.AddDays(9));
                    cmd.Parameters.AddWithValue("@StartTime", TimeSpan.Parse("10:00:00")); // 10:00 AM
                    cmd.Parameters.AddWithValue("@EndTime", TimeSpan.Parse("11:30:00"));   // 11:30 AM
                    cmd.Parameters.AddWithValue("@SessionType", "Premium");
                    cmd.Parameters.AddWithValue("@SessionTitle", "Small Group: Latest Phone Scam Tactics");
                    cmd.Parameters.AddWithValue("@SessionDescription", "Intimate session with max 10 participants. Deep dive into current phone scam methods with personalized Q&A time.");
                    cmd.Parameters.AddWithValue("@ExpertName", "Officer James Wilson");
                    cmd.Parameters.AddWithValue("@ExpertTitle", "Fraud Investigation Specialist, 10+ years");
                    cmd.Parameters.AddWithValue("@MaxParticipants", 10);
                    cmd.Parameters.AddWithValue("@CurrentParticipants", 0);
                    cmd.Parameters.AddWithValue("@PointsCost", 50);
                    cmd.Parameters.AddWithValue("@Status", "Available");
                    cmd.Parameters.AddWithValue("@SessionTopic", "Phone");
                    cmd.Parameters.AddWithValue("@CreatedBy", createdByUserId);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("✅ Created Session 2: Phone Scams (Premium - 50 points)");
                }

                // Session 3 - VIP One-on-One Session
                string insertSession3 = @"
            INSERT INTO ExpertSessions (
                SessionDate, StartTime, EndTime, SessionType, SessionTitle, SessionDescription, 
                ExpertName, ExpertTitle, MaxParticipants, CurrentParticipants, PointsCost, 
                Status, SessionTopic, CreatedBy, CreatedDate
            ) VALUES (
                @SessionDate, @StartTime, @EndTime, @SessionType, @SessionTitle, @SessionDescription,
                @ExpertName, @ExpertTitle, @MaxParticipants, @CurrentParticipants, @PointsCost,
                @Status, @SessionTopic, @CreatedBy, @CreatedDate
            )";

                using (SqlCommand cmd = new SqlCommand(insertSession3, conn))
                {
                    cmd.Parameters.AddWithValue("@SessionDate", DateTime.Today.AddDays(12));
                    cmd.Parameters.AddWithValue("@StartTime", TimeSpan.Parse("15:00:00")); // 3:00 PM
                    cmd.Parameters.AddWithValue("@EndTime", TimeSpan.Parse("16:00:00"));   // 4:00 PM
                    cmd.Parameters.AddWithValue("@SessionType", "Premium");
                    cmd.Parameters.AddWithValue("@SessionTitle", "VIP One-on-One Safety Consultation");
                    cmd.Parameters.AddWithValue("@SessionDescription", "Private consultation to review your personal digital security, analyze any suspicious communications, and create a personalized safety plan.");
                    cmd.Parameters.AddWithValue("@ExpertName", "Maria Rodriguez");
                    cmd.Parameters.AddWithValue("@ExpertTitle", "Digital Safety Educator, Senior Specialist");
                    cmd.Parameters.AddWithValue("@MaxParticipants", 1);
                    cmd.Parameters.AddWithValue("@CurrentParticipants", 0);
                    cmd.Parameters.AddWithValue("@PointsCost", 100);
                    cmd.Parameters.AddWithValue("@Status", "Available");
                    cmd.Parameters.AddWithValue("@SessionTopic", "Social");
                    cmd.Parameters.AddWithValue("@CreatedBy", createdByUserId);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("✅ Created Session 3: VIP Consultation (Premium - 100 points)");
                }

                System.Diagnostics.Debug.WriteLine("✅ Successfully created 3 sample sessions");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error creating sample sessions: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            }
        }

        private void LoadFallbackSessions()
        {
            // Create fallback data
            DataTable dt = new DataTable();
            dt.Columns.Add("SessionId", typeof(int));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("SessionDate", typeof(DateTime));
            dt.Columns.Add("StartTime", typeof(string));
            dt.Columns.Add("EndTime", typeof(string));
            dt.Columns.Add("MaxParticipants", typeof(int));
            dt.Columns.Add("PointsRequired", typeof(int));
            dt.Columns.Add("SessionType", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("ExpertName", typeof(string));
            dt.Columns.Add("ExpertTitle", typeof(string));
            dt.Columns.Add("CurrentRegistrations", typeof(int));
            dt.Columns.Add("AvailableSpots", typeof(int));

            // Add fallback sessions
            dt.Rows.Add(1, "Protecting Your Online Banking",
                "Learn essential security practices for online banking, how to spot fraudulent websites, and what to do if your account is compromised.",
                new DateTime(2025, 8, 15), "2:00 PM", "3:00 PM", 100, 0, "Free", "Banking",
                "Dr Harvey Blue", "Cybersecurity Specialist, 15+ years experience", 0, 100);

            dt.Rows.Add(2, "Small Group: Latest Phone Scam Tactics",
                "Intimate session with max 10 participants. Deep dive into current phone scam methods with personalized Q&A time.",
                new DateTime(2025, 8, 17), "10:00 AM", "11:30 AM", 10, 50, "Premium", "Phone",
                "Officer James Wilson", "Investigating phone and romance scams for 10+ years, helping victims recover", 0, 10);

            dt.Rows.Add(3, "VIP One-on-One Safety Consultation",
                "Private consultation to review your personal digital security, analyze any suspicious communications, and create a personalized safety plan.",
                new DateTime(2025, 8, 19), "3:00 PM", "4:00 PM", 1, 100, "Premium", "Social",
                "Maria Rodriguez", "Digital Safety Educator, Senior Specialist", 0, 1);

            ViewState["SessionsData"] = dt;
            rptSessions.DataSource = dt;
            rptSessions.DataBind();
        }

        protected void FilterSessions_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string filter = btn.CommandArgument;

            // Update button styles
            btnAllSessions.CssClass = "filter-btn " + (filter == "All" ? "active" : "inactive");
            btnFreeOnly.CssClass = "filter-btn " + (filter == "Free" ? "active" : "inactive");
            btnPremium.CssClass = "filter-btn " + (filter == "Premium" ? "active" : "inactive");

            // Reset topic filter when session filter changes
            ddlTopics.SelectedValue = "All";

            // Apply filter
            ApplyFilters(filter, "All");
        }

        protected void ddlTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            string topicFilter = ddlTopics.SelectedValue;
            string sessionFilter = "All";

            // Determine current session filter
            if (btnFreeOnly.CssClass.Contains("active"))
                sessionFilter = "Free";
            else if (btnPremium.CssClass.Contains("active"))
                sessionFilter = "Premium";

            ApplyFilters(sessionFilter, topicFilter);
        }

        private void ApplyFilters(string sessionFilter, string topicFilter)
        {
            DataTable originalData = (DataTable)ViewState["SessionsData"];
            if (originalData == null) return;

            DataTable filteredData = originalData.Clone();

            foreach (DataRow row in originalData.Rows)
            {
                bool includeSession = true;

                // Apply session type filter
                if (sessionFilter != "All")
                {
                    string sessionType = row["SessionType"].ToString();
                    if (sessionFilter == "Free" && Convert.ToInt32(row["PointsRequired"]) > 0)
                        includeSession = false;
                    else if (sessionFilter == "Premium" && Convert.ToInt32(row["PointsRequired"]) == 0)
                        includeSession = false;
                }

                // Apply topic filter
                if (topicFilter != "All" && includeSession)
                {
                    string category = row["Category"]?.ToString() ?? "";
                    if (!string.Equals(category, topicFilter, StringComparison.OrdinalIgnoreCase))
                        includeSession = false;
                }

                if (includeSession)
                {
                    filteredData.ImportRow(row);
                }
            }

            rptSessions.DataSource = filteredData;
            rptSessions.DataBind();
        }

        private void ShowAllSessions()
        {
            DataTable data = (DataTable)ViewState["SessionsData"];
            if (data != null)
            {
                rptSessions.DataSource = data;
                rptSessions.DataBind();
            }
        }

        // Helper methods for the repeater
        protected string GetSessionTypeClass(object pointsRequired)
        {
            int points = Convert.ToInt32(pointsRequired);
            return points > 0 ? "type-premium" : "type-free";
        }

        protected string GetSessionTypeText(object pointsRequired, object sessionType)
        {
            int points = Convert.ToInt32(pointsRequired);
            return points > 0 ? "PREMIUM" : "FREE";
        }

        protected string GetParticipantText(object currentReg, object maxParticipants)
        {
            int current = Convert.ToInt32(currentReg);
            int max = Convert.ToInt32(maxParticipants);
            int remaining = max - current;

            if (max == 1)
                return "One-on-one session";
            else if (max <= 10)
                return $"Max {max} Participants ({remaining} spots left)";
            else
                return $"Up to {max} Participants ({remaining} spots left)";
        }

        protected string GetPointsText(object pointsRequired, object sessionType)
        {
            int points = Convert.ToInt32(pointsRequired);
            if (points > 0)
                return $"{points} POINTS";
            else
                return "Free Session";
        }

        protected string GetButtonText(object availableSpots, object pointsRequired)
        {
            int available = Convert.ToInt32(availableSpots);
            int points = Convert.ToInt32(pointsRequired);

            if (available <= 0)
                return "Session Full";
            else if (points > 0)
                return "Reserve Spot";
            else
                return "Register Now";
        }

        protected string GetButtonClass(object availableSpots, object pointsRequired)
        {
            int available = Convert.ToInt32(availableSpots);
            int points = Convert.ToInt32(pointsRequired);

            if (available <= 0)
                return "need-points-btn";
            else if (points > 0)
                return "reserve-btn";
            else
                return "register-btn";
        }

        protected bool IsSessionAvailable(object availableSpots)
        {
            return Convert.ToInt32(availableSpots) > 0;
        }

        protected string GetExpertImage(object sessionId)
        {
            // Since we can't access ExpertName directly in this method,
            // we'll use the sessionId but handle the dynamic IDs created by the database

            try
            {
                // Get the expert name for this session from the current data
                DataTable sessionData = (DataTable)ViewState["SessionsData"];
                if (sessionData != null)
                {
                    int id = Convert.ToInt32(sessionId);

                    // Find the row with this SessionId
                    DataRow[] rows = sessionData.Select($"SessionId = {id}");
                    if (rows.Length > 0)
                    {
                        string expertName = rows[0]["ExpertName"]?.ToString() ?? "";

                        // Return image based on expert name
                        switch (expertName.ToLower())
                        {
                            case string name when name.Contains("harvey") && name.Contains("blue"):
                                return "/Images/expert2.jpg";
                            case string name when name.Contains("james") && name.Contains("wilson"):
                                return "/Images/expert3.jpg";
                            case string name when name.Contains("maria") && name.Contains("rodriguez"):
                                return "/Images/expert1.jpg";
                            default:
                                // Cycle through images based on sessionId to ensure variety
                                int imageIndex = (id % 3) + 1;
                                return $"/Images/expert{imageIndex}.jpg";
                        }
                    }
                }

                // Fallback: cycle through images based on ID
                int fallbackId = Convert.ToInt32(sessionId);
                int fallbackImageIndex = ((fallbackId - 1) % 3) + 1;
                return $"/Images/expert{fallbackImageIndex}.jpg";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting expert image: {ex.Message}");
                // Return a random image as last resort
                return "/Images/expert2.jpg";
            }
        }
    }
}