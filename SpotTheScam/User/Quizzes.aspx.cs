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
    public partial class Quizzes : System.Web.UI.Page
    {
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

                System.Diagnostics.Debug.WriteLine($"=== Quizzes Page Load START ===");
                System.Diagnostics.Debug.WriteLine($"Session Username: {Session["Username"]}");

                // CRITICAL: Clear any cached points data to force fresh load
                Session["CurrentPoints"] = null;
                Session.Remove("CurrentPoints");

                // Add cache-busting headers
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();

                // Load user's current points from PointsTransactions table with retry logic
                LoadUserCurrentPointsWithRetry();

                // Update the UI with the current points
                UpdatePointsDisplay();

                System.Diagnostics.Debug.WriteLine($"=== Quizzes Page Load END ===");
            }
        }

        // FIXED METHOD: Load user points with retry logic
        private void LoadUserCurrentPointsWithRetry()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== LoadUserCurrentPointsWithRetry in Quizzes ===");
                System.Diagnostics.Debug.WriteLine($"Session Username: {Session["Username"]}");

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

                int maxRetries = 3;
                bool pointsLoaded = false;
                int currentPoints = 0;
                int userId = 0;

                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            // Get UserID from Username - using correct column name from your Users table
                            if (userId == 0) // Only get UserID on first attempt
                            {
                                string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";
                                using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                                    object userIdResult = cmd.ExecuteScalar();
                                    if (userIdResult != null)
                                    {
                                        userId = Convert.ToInt32(userIdResult);
                                        // Store UserID in session for use throughout the application
                                        Session["UserID"] = userId;
                                        System.Diagnostics.Debug.WriteLine($"User found: {Session["Username"]}, UserID: {userId}");
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine($"User not found: {Session["Username"]}");
                                        Response.Redirect("UserLogin.aspx");
                                        return;
                                    }
                                }
                            }

                            // Get current total points from PointsTransactions table (the authoritative source)
                            string getPointsQuery = @"
                                SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                                FROM PointsTransactions 
                                WHERE UserId = @UserId";

                            using (SqlCommand cmd = new SqlCommand(getPointsQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                object result = cmd.ExecuteScalar();

                                if (result != null)
                                {
                                    currentPoints = Convert.ToInt32(result);
                                    pointsLoaded = true;

                                    System.Diagnostics.Debug.WriteLine($"Retry {retry + 1}: Current points loaded: {currentPoints}");

                                    // Store points in session to share across pages
                                    Session["CurrentPoints"] = currentPoints;

                                    // Debug: Show recent transactions to verify data
                                    DebugRecentTransactions(conn, userId);

                                    break; // Success, exit retry loop
                                }
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
                    currentPoints = 0;
                    Session["CurrentPoints"] = 0;
                }

                System.Diagnostics.Debug.WriteLine($"Final points value: {currentPoints}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user points in Quizzes: {ex.Message}");
                Session["CurrentPoints"] = 0;
            }
        }

        // BACKWARD COMPATIBILITY: Keep old method name
        private void LoadUserCurrentPoints()
        {
            LoadUserCurrentPointsWithRetry();
        }

        private void UpdatePointsDisplay()
        {
            // Get the current points from session
            int currentPoints = Session["CurrentPoints"] != null ? Convert.ToInt32(Session["CurrentPoints"]) : 0;

            System.Diagnostics.Debug.WriteLine($"UpdatePointsDisplay in Quizzes called with currentPoints: {currentPoints}");

            // ENHANCED: More comprehensive points display update for Quizzes page
            string script = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    console.log('=== Quizzes UpdatePointsDisplay START ===');
                    console.log('Target points value: {currentPoints}');

                    // STEP 1: Find and update all common points display elements
                    var pointsElements = document.querySelectorAll('.current-points, .points-display, .points-badge, .user-points, .points-counter, .points-value, .quiz-points');
                    console.log('Found ' + pointsElements.length + ' points elements');
                    
                    for (var i = 0; i < pointsElements.length; i++) {{
                        var originalText = pointsElements[i].textContent;
                        if (originalText.includes('Current Points') || 
                            originalText.includes('Points') ||
                            originalText.includes('⭐')) {{
                            pointsElements[i].innerHTML = 'Current Points: {currentPoints}';
                            console.log('Updated element ' + i + ': ' + originalText + ' -> Current Points: {currentPoints}');
                        }}
                    }}
                    
                    // STEP 2: Update spans inside points badges (very common pattern)
                    var badgeSpans = document.querySelectorAll('.points-badge span, .badge span, .user-badge span, .quiz-header span');
                    console.log('Found ' + badgeSpans.length + ' badge spans');
                    
                    for (var j = 0; j < badgeSpans.length; j++) {{
                        var originalText = badgeSpans[j].textContent;
                        if (originalText.includes('Points') || originalText.includes('⭐') || originalText.includes('30')) {{
                            badgeSpans[j].innerHTML = 'Current Points: {currentPoints} ⭐';
                            console.log('Updated badge span ' + j + ': ' + originalText + ' -> Current Points: {currentPoints} ⭐');
                        }}
                    }}
                    
                    // STEP 3: Aggressive search and replace for hardcoded values
                    var allElements = document.querySelectorAll('*');
                    var replacementCount = 0;
                    
                    for (var k = 0; k < allElements.length; k++) {{
                        var element = allElements[k];
                        if (element.children.length === 0) {{ // Only leaf elements (text nodes)
                            var text = element.textContent;
                            if (text) {{
                                var trimmedText = text.trim();
                                var shouldReplace = false;
                                
                                // Check for exact hardcoded matches
                                if (trimmedText === 'Current Points: 30') {{
                                    element.innerHTML = 'Current Points: {currentPoints}';
                                    replacementCount++;
                                    shouldReplace = true;
                                    console.log('Replaced exact match: Current Points: 30 -> Current Points: {currentPoints}');
                                }}
                                else if (trimmedText === '30') {{
                                    var parent = element.parentElement;
                                    // Check if this is a points display based on parent context
                                    if (parent && (parent.className.includes('points') || 
                                                 parent.className.includes('badge') ||
                                                 parent.className.includes('quiz') ||
                                                 parent.textContent.includes('Current Points') ||
                                                 parent.textContent.includes('Points:'))) {{
                                        element.textContent = '{currentPoints}';
                                        replacementCount++;
                                        shouldReplace = true;
                                        console.log('Replaced standalone 30 in points context -> {currentPoints}');
                                    }}
                                }}
                                
                                // Check for partial matches in longer text
                                if (!shouldReplace && text.includes('Current Points: 30')) {{
                                    element.innerHTML = text.replace('Current Points: 30', 'Current Points: {currentPoints}');
                                    replacementCount++;
                                    console.log('Replaced partial match: ' + text + ' -> ' + element.innerHTML);
                                }}
                            }}
                        }}
                    }}
                    
                    // STEP 4: Specifically target ASP.NET labels and quiz-specific elements
                    var aspLabels = document.querySelectorAll('[id*=""lblCurrentPoints""], [id*=""CurrentPoints""], [id*=""Points""], .quiz-points-display, .user-points-display');
                    console.log('Found ' + aspLabels.length + ' ASP.NET labels and quiz elements');
                    
                    for (var l = 0; l < aspLabels.length; l++) {{
                        var originalText = aspLabels[l].textContent;
                        if (originalText === '30' || originalText === 'Current Points: 30' || originalText.includes('30')) {{
                            aspLabels[l].textContent = '{currentPoints}';
                            console.log('Updated ASP.NET/quiz label: ' + originalText + ' -> {currentPoints}');
                        }}
                    }}
                    
                    // STEP 5: Check for any quiz-specific points displays
                    var quizElements = document.querySelectorAll('.quiz-card .points, .quiz-item .points, .quiz-header .points');
                    console.log('Found ' + quizElements.length + ' quiz-specific point elements');
                    
                    for (var m = 0; m < quizElements.length; m++) {{
                        var originalText = quizElements[m].textContent;
                        if (originalText === '30' || originalText.includes('30')) {{
                            quizElements[m].textContent = 'Current Points: {currentPoints}';
                            console.log('Updated quiz element: ' + originalText + ' -> Current Points: {currentPoints}');
                        }}
                    }}
                    
                    console.log('Total replacements made: ' + replacementCount);
                    console.log('Quizzes points display update completed for: {currentPoints}');
                    console.log('=== Quizzes UpdatePointsDisplay END ===');
                    
                    // STEP 6: Delayed check for dynamically loaded content
                    setTimeout(function() {{
                        console.log('Running delayed quiz points update check...');
                        var delayedElements = document.querySelectorAll('*');
                        var delayedReplacements = 0;
                        
                        for (var n = 0; n < delayedElements.length; n++) {{
                            var element = delayedElements[n];
                            if (element.children.length === 0) {{
                                var text = element.textContent;
                                if (text && (text.trim() === '30' || text.trim() === 'Current Points: 30')) {{
                                    var parent = element.parentElement;
                                    if (parent && (parent.className.includes('points') || 
                                                 parent.className.includes('badge') || 
                                                 parent.className.includes('quiz'))) {{
                                        if (text.trim() === '30') {{
                                            element.textContent = '{currentPoints}';
                                        }} else {{
                                            element.textContent = 'Current Points: {currentPoints}';
                                        }}
                                        delayedReplacements++;
                                    }}
                                }}
                            }}
                        }}
                        
                        if (delayedReplacements > 0) {{
                            console.log('Made ' + delayedReplacements + ' delayed quiz replacements');
                        }}
                    }}, 3000); // Check again after 3 seconds for quiz page
                }});
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "UpdatePointsDisplay", script, true);
        }

        private void DebugRecentTransactions(SqlConnection conn, int userId)
        {
            try
            {
                string debugQuery = @"
                    SELECT TOP 5 TransactionType, Points, Description, TransactionDate
                    FROM PointsTransactions 
                    WHERE UserId = @UserId
                    ORDER BY TransactionDate DESC";

                using (SqlCommand cmd = new SqlCommand(debugQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        System.Diagnostics.Debug.WriteLine("=== Recent Transactions in Quizzes ===");
                        int transactionCount = 0;
                        while (reader.Read())
                        {
                            transactionCount++;
                            System.Diagnostics.Debug.WriteLine($"{transactionCount}. {reader["TransactionType"]}: {reader["Points"]} points - {reader["Description"]} ({reader["TransactionDate"]})");
                        }
                        System.Diagnostics.Debug.WriteLine($"Total transactions shown: {transactionCount}");

                        if (transactionCount == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("No transactions found for this user - this might be why points show as 0 or default value");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recent transactions: {ex.Message}");
            }
        }

        // NEW METHOD: Get current points as string for use in markup
        public string GetCurrentPointsText()
        {
            int currentPoints = Session["CurrentPoints"] != null ? Convert.ToInt32(Session["CurrentPoints"]) : 0;
            return currentPoints.ToString();
        }
    }
}