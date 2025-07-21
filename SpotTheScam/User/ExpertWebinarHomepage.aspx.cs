using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace SpotTheScam.User
{
    public partial class ExpertWebinarHomepage : System.Web.UI.Page
    {
        private string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
        private int currentUserPoints = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                if (Session["Username"] == null || string.IsNullOrEmpty(Session["Username"].ToString()))
                {
                    Response.Redirect("UserLogin.aspx");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"=== ExpertWebinarHomepage Page_Load START ===");

                // CRITICAL: Clear any cached points data to force fresh load
                Session["CurrentPoints"] = null;
                Session.Remove("CurrentPoints");

                // Add cache-busting headers
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();

                // Load user points FIRST - this is critical
                LoadUserCurrentPointsWithRetry();

                // Load upcoming sessions
                LoadUpcomingSessions();

                // Update points display AFTER loading actual points
                UpdatePointsDisplay();

                System.Diagnostics.Debug.WriteLine($"=== ExpertWebinarHomepage Page_Load END - Final Points: {currentUserPoints} ===");
            }
        }

        // FIXED METHOD: Load user points with retry logic to ensure accuracy
        private void LoadUserCurrentPointsWithRetry()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== LoadUserCurrentPointsWithRetry in ExpertWebinarHomepage ===");
                System.Diagnostics.Debug.WriteLine($"Session Username: {Session["Username"]}");

                int maxRetries = 3;
                bool pointsLoaded = false;

                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            // Get UserID from Username first
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
                                    System.Diagnostics.Debug.WriteLine($"User found: {Session["Username"]}, UserID: {userId}");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"User not found: {Session["Username"]}");
                                    currentUserPoints = 0;
                                    Session["CurrentPoints"] = 0;
                                    return;
                                }
                            }

                            // CRITICAL: Get current total points from PointsTransactions table (authoritative source)
                            string getPointsQuery = @"
                                SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                                FROM PointsTransactions 
                                WHERE UserId = @UserId";

                            using (SqlCommand cmd = new SqlCommand(getPointsQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                object result = cmd.ExecuteScalar();

                                currentUserPoints = result != null ? Convert.ToInt32(result) : 0;
                                Session["CurrentPoints"] = currentUserPoints;
                                pointsLoaded = true;

                                System.Diagnostics.Debug.WriteLine($"Retry {retry + 1}: Current points loaded: {currentUserPoints}");
                                break;
                            }
                        }
                    }
                    catch (Exception retryEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Retry {retry + 1} failed: {retryEx.Message}");
                        if (retry < maxRetries - 1)
                        {
                            System.Threading.Thread.Sleep(500); // Wait before retry
                        }
                    }
                }

                if (!pointsLoaded)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to load points after all retries - setting to 0");
                    currentUserPoints = 0;
                    Session["CurrentPoints"] = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user points in ExpertWebinarHomepage: {ex.Message}");
                currentUserPoints = 0;
                Session["CurrentPoints"] = 0;
            }
        }

        // BACKWARD COMPATIBILITY METHOD: Keep old method name for any existing calls
        private void LoadUserCurrentPoints()
        {
            LoadUserCurrentPointsWithRetry();
        }

        private void UpdatePointsDisplay()
        {
            System.Diagnostics.Debug.WriteLine($"UpdatePointsDisplay called with currentUserPoints: {currentUserPoints}");

            // ENHANCED: More comprehensive points display update
            string script = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    console.log('=== ExpertWebinarHomepage UpdatePointsDisplay START ===');
                    console.log('Target points value: {currentUserPoints}');

                    // STEP 1: Update common points display elements
                    var pointsElements = document.querySelectorAll('.current-points, .points-display, .points-badge, .user-points, .points-counter, .points-value');
                    console.log('Found ' + pointsElements.length + ' points elements');
                    
                    for (var i = 0; i < pointsElements.length; i++) {{
                        var originalText = pointsElements[i].textContent;
                        pointsElements[i].textContent = 'Current Points: {currentUserPoints}';
                        console.log('Updated element ' + i + ': ' + originalText + ' -> Current Points: {currentUserPoints}');
                    }}
                    
                    // STEP 2: Update spans inside points badges (common pattern)
                    var badgeSpans = document.querySelectorAll('.points-badge span, .badge span, .user-badge span');
                    console.log('Found ' + badgeSpans.length + ' badge spans');
                    
                    for (var j = 0; j < badgeSpans.length; j++) {{
                        var originalText = badgeSpans[j].textContent;
                        if (originalText.includes('Points') || originalText.includes('⭐') || originalText.includes('30')) {{
                            badgeSpans[j].innerHTML = 'Current Points: {currentUserPoints} ⭐';
                            console.log('Updated badge span ' + j + ': ' + originalText + ' -> Current Points: {currentUserPoints} ⭐');
                        }}
                    }}
                    
                    // STEP 3: Aggressive search and replace for hardcoded values
                    var allElements = document.querySelectorAll('*');
                    var replacementCount = 0;
                    
                    for (var k = 0; k < allElements.length; k++) {{
                        var element = allElements[k];
                        if (element.children.length === 0) {{ // Only text nodes, avoid nested elements
                            var text = element.textContent;
                            if (text) {{
                                var trimmedText = text.trim();
                                
                                // Replace exact matches
                                if (trimmedText === 'Current Points: 30' || trimmedText === '30') {{
                                    var parent = element.parentElement;
                                    if (parent && (parent.className.includes('points') || 
                                                 parent.className.includes('badge') ||
                                                 parent.className.includes('user') ||
                                                 text.includes('Current Points') ||
                                                 text.includes('Points:'))) {{
                                        element.textContent = 'Current Points: {currentUserPoints}';
                                        replacementCount++;
                                        console.log('Replaced hardcoded value: ' + trimmedText + ' -> Current Points: {currentUserPoints}');
                                    }}
                                }}
                                
                                // Replace partial matches
                                else if (text.includes('Current Points: 30')) {{
                                    element.textContent = text.replace('Current Points: 30', 'Current Points: {currentUserPoints}');
                                    replacementCount++;
                                    console.log('Replaced partial match in: ' + text);
                                }}
                            }}
                        }}
                    }}
                    
                    // STEP 4: Force update any ASP.NET labels that might have hardcoded values
                    var aspLabels = document.querySelectorAll('[id*=""lblCurrentPoints""], [id*=""CurrentPoints""], [id*=""Points""]');
                    console.log('Found ' + aspLabels.length + ' ASP.NET labels');
                    
                    for (var l = 0; l < aspLabels.length; l++) {{
                        var originalText = aspLabels[l].textContent;
                        if (originalText === '30' || originalText === 'Current Points: 30' || originalText.includes('30')) {{
                            aspLabels[l].textContent = '{currentUserPoints}';
                            console.log('Updated ASP.NET label: ' + originalText + ' -> {currentUserPoints}');
                        }}
                    }}
                    
                    console.log('Total replacements made: ' + replacementCount);
                    console.log('Points display update completed for: {currentUserPoints}');
                    console.log('=== ExpertWebinarHomepage UpdatePointsDisplay END ===');
                    
                    // STEP 5: Set a delayed check to catch any late-loading elements
                    setTimeout(function() {{
                        console.log('Running delayed points update check...');
                        var delayedElements = document.querySelectorAll('*');
                        var delayedReplacements = 0;
                        
                        for (var m = 0; m < delayedElements.length; m++) {{
                            var element = delayedElements[m];
                            if (element.children.length === 0) {{
                                var text = element.textContent;
                                if (text && text.trim() === '30') {{
                                    var parent = element.parentElement;
                                    if (parent && (parent.className.includes('points') || parent.className.includes('badge'))) {{
                                        element.textContent = '{currentUserPoints}';
                                        delayedReplacements++;
                                    }}
                                }}
                            }}
                        }}
                        
                        if (delayedReplacements > 0) {{
                            console.log('Made ' + delayedReplacements + ' delayed replacements');
                        }}
                    }}, 2000); // Check again after 2 seconds
                }});
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "UpdatePointsDisplay", script, true);
        }

        private void LoadUpcomingSessions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load upcoming sessions from database
                    string query = @"
                        SELECT TOP 6 Id as SessionId, SessionTitle as Title, SessionDescription as Description, 
                               SessionDate, StartTime, ExpertName, PointsCost as PointsRequired, 
                               SessionType, (MaxParticipants - CurrentParticipants) as AvailableSpots, 
                               MaxParticipants
                        FROM ExpertSessions 
                        WHERE SessionDate >= CAST(GETDATE() AS DATE) 
                        AND Status = 'Available'
                        ORDER BY SessionDate ASC, StartTime ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            // Bind to repeater or update individual session controls
                            rptUpcomingSessions.DataSource = dt;
                            rptUpcomingSessions.DataBind();
                        }
                        else
                        {
                            // Show fallback message or load hardcoded sessions
                            LoadFallbackSessions();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading upcoming sessions: {ex.Message}");
                // Load fallback hardcoded sessions if database fails
                LoadFallbackSessions();
            }
        }

        private void LoadFallbackSessions()
        {
            // Create fallback data structure
            DataTable dt = new DataTable();
            dt.Columns.Add("SessionId", typeof(int));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("SessionDate", typeof(DateTime));
            dt.Columns.Add("StartTime", typeof(string));
            dt.Columns.Add("ExpertName", typeof(string));
            dt.Columns.Add("PointsRequired", typeof(int));
            dt.Columns.Add("SessionType", typeof(string));
            dt.Columns.Add("AvailableSpots", typeof(int));
            dt.Columns.Add("MaxParticipants", typeof(int));

            // Add hardcoded fallback sessions
            dt.Rows.Add(1, "Protecting Your Online Banking",
                "Learn secure banking practices and how to spot fraudulent banking websites with cybersecurity expert Dr. Harvey Blue",
                new DateTime(2025, 6, 15), "2:00 PM", "Dr Harvey Blue", 0, "Free", 100, 100);

            dt.Rows.Add(2, "Latest Phone Scam Tactics",
                "Discover the newest phone scam methods and how to protect yourself with Officer James Wilson from the Police Fraud Division",
                new DateTime(2025, 6, 17), "10:00 AM", "Officer James Wilson", 50, "Premium", 10, 10);

            dt.Rows.Add(3, "Safe Social Media for Seniors",
                "Navigate Facebook, Instagram, and other platforms safely while avoiding scammers with digital educator Maria Rodriguez",
                new DateTime(2025, 6, 19), "2:00 PM", "Maria Rodriguez", 0, "Free", 50, 50);

            rptUpcomingSessions.DataSource = dt;
            rptUpcomingSessions.DataBind();
        }

        protected string GetSessionTypeClass(object pointsRequired)
        {
            int points = Convert.ToInt32(pointsRequired);
            return points > 0 ? "premium-session" : "free-session";
        }

        protected string GetSessionTypeText(object pointsRequired, object sessionType)
        {
            int points = Convert.ToInt32(pointsRequired);
            if (points > 0)
            {
                return $"{sessionType} - {points} Points";
            }
            return "Free Session";
        }

        protected string GetAvailabilityText(object availableSpots, object maxParticipants)
        {
            int available = Convert.ToInt32(availableSpots);
            int max = Convert.ToInt32(maxParticipants);

            if (available <= 0)
            {
                return "Session Full";
            }
            else if (max == 1)
            {
                return "One-on-one session";
            }
            else if (available <= 3)
            {
                return $"Only {available} spots left!";
            }
            else
            {
                return $"{available} spots available";
            }
        }

        protected string GetAvailabilityClass(object availableSpots)
        {
            int available = Convert.ToInt32(availableSpots);

            if (available <= 0)
            {
                return "session-full";
            }
            else if (available <= 3)
            {
                return "session-limited";
            }
            else
            {
                return "session-available";
            }
        }

        protected string FormatSessionDate(object sessionDate, object startTime)
        {
            DateTime date = Convert.ToDateTime(sessionDate);
            string time = startTime.ToString();
            return $"{date:MMMM dd}, {time}";
        }

        // NEW METHOD: Get current points as string for use in markup
        public string GetCurrentPointsText()
        {
            return currentUserPoints.ToString();
        }
    }
}