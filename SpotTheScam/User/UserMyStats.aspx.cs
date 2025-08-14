using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace SpotTheScam.User
{
    public partial class UserMyStats : System.Web.UI.Page
    {
        // Connection string with null checking
        private string connectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize connection string
                InitializeConnectionString();

                // Check if user is logged in (following same pattern as UserPointsStore)
                if (Session["Username"] == null || string.IsNullOrEmpty(Session["Username"].ToString()))
                {
                    Response.Redirect("UserLogin.aspx");
                    return;
                }

                // Initialize default values first
                InitializeDefaultValues();

                // Get UserID from Username (same as UserPointsStore)
                GetUserIdFromUsername();

                LoadUserStats();
            }
        }

        private void InitializeConnectionString()
        {
            try
            {
                // Use the correct connection string name from your web.config
                var connString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"];

                if (connString != null && !string.IsNullOrEmpty(connString.ConnectionString))
                {
                    connectionString = connString.ConnectionString;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Warning: SpotTheScamConnectionString not found in web.config");
                    connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\SpotTheScamDB.mdf;Integrated Security=True;Connect Timeout=30";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing connection string: {ex.Message}");
                connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\SpotTheScamDB.mdf;Integrated Security=True;Connect Timeout=30";
            }
        }

        private void InitializeDefaultValues()
        {
            try
            {
                // Set default values for all controls to prevent null reference errors
                if (lblTotalPoints != null) lblTotalPoints.Text = "0";
                if (lblCurrentPoints != null) lblCurrentPoints.Text = "0";
                if (lblQuizzesCompleted != null) lblQuizzesCompleted.Text = "1";
                if (lblTotalQuizzes != null) lblTotalQuizzes.Text = "6";
                if (hiddenQuizProgress != null) hiddenQuizProgress.Value = "17";
                if (lblCurrentStreak != null) lblCurrentStreak.Text = "1";
                if (lblAchievementsUnlocked != null) lblAchievementsUnlocked.Text = "1";
                if (lblMotivationalMessage != null) lblMotivationalMessage.Text = "Welcome! Start your scam protection journey today.";

                System.Diagnostics.Debug.WriteLine("✅ Default values initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error initializing default values: {ex.Message}");
            }
        }

        private void GetUserIdFromUsername()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from Username (same query as UserPointsStore)
                    string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                        object userIdResult = cmd.ExecuteScalar();
                        if (userIdResult != null)
                        {
                            int userId = Convert.ToInt32(userIdResult);
                            Session["UserID"] = userId;
                            System.Diagnostics.Debug.WriteLine($"✅ Stats page - UserID set to: {userId} for username: {Session["Username"]}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ Stats page - No user found for username: {Session["Username"]}");
                            // Don't call ShowDefaultStats here since defaults are already set
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting UserID from Username: {ex.Message}");
                // Don't call ShowDefaultStats here since defaults are already set
            }
        }

        private void LoadUserStats()
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    System.Diagnostics.Debug.WriteLine("Connection string is null or empty. Using default stats.");
                    return; // Default values already set in InitializeDefaultValues
                }

                if (Session["UserID"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("UserID is null. Using default stats.");
                    return; // Default values already set in InitializeDefaultValues
                }

                int userId = Convert.ToInt32(Session["UserID"]);

                // Load all stats
                LoadBasicStats(userId);
                LoadQuizStats(userId);
                LoadAchievementStats(userId);
                LoadActivityStats(userId);
                SetMotivationalMessage();
            }
            catch (Exception ex)
            {
                // Log error and keep default values
                System.Diagnostics.Debug.WriteLine($"Error loading user stats: {ex.Message}");
                // Don't call ShowDefaultStats since defaults are already set
            }
        }

        private void LoadBasicStats(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get total points from PointsTransactions table (same as UserPointsStore)
                    string pointsQuery = @"
                        SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                        FROM PointsTransactions 
                        WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(pointsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int totalPoints = Convert.ToInt32(cmd.ExecuteScalar());

                        // Set both current points and total points to the same value
                        lblTotalPoints.Text = totalPoints.ToString();
                        lblCurrentPoints.Text = totalPoints.ToString();
                        Session["CurrentPoints"] = totalPoints;

                        System.Diagnostics.Debug.WriteLine($"✅ Stats page - Total points loaded: {totalPoints}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle database connection issues with null checks
                System.Diagnostics.Debug.WriteLine($"❌ Error loading basic stats: {ex.Message}");
                if (lblTotalPoints != null) lblTotalPoints.Text = "0";
                if (lblCurrentPoints != null) lblCurrentPoints.Text = "0";
            }
        }

        private void LoadQuizStats(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get quiz completion stats
                    string quizQuery = @"
                        SELECT 
                            COUNT(DISTINCT QuizID) as CompletedQuizzes
                        FROM UserQuizResults 
                        WHERE UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(quizQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int completedQuizzes = reader["CompletedQuizzes"] != DBNull.Value ?
                                    Convert.ToInt32(reader["CompletedQuizzes"]) : 1; // Default to 1

                                lblQuizzesCompleted.Text = completedQuizzes.ToString();
                            }
                        }
                    }

                    // Get total available quizzes
                    string totalQuizzesQuery = "SELECT COUNT(*) FROM Quizzes WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(totalQuizzesQuery, conn))
                    {
                        int totalQuizzes = Convert.ToInt32(cmd.ExecuteScalar());
                        if (totalQuizzes == 0) totalQuizzes = 6; // Default fallback
                        lblTotalQuizzes.Text = totalQuizzes.ToString();

                        // Calculate progress percentage
                        int completed = Convert.ToInt32(lblQuizzesCompleted.Text);
                        double progressPercentage = totalQuizzes > 0 ? (double)completed / totalQuizzes * 100 : 17;
                        hiddenQuizProgress.Value = Math.Round(progressPercentage, 0).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading quiz stats: {ex.Message}");
                if (lblQuizzesCompleted != null) lblQuizzesCompleted.Text = "1";
                if (lblTotalQuizzes != null) lblTotalQuizzes.Text = "6";
                if (hiddenQuizProgress != null) hiddenQuizProgress.Value = "17";
            }
        }

        private void LoadAchievementStats(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get achievement count
                    string achievementQuery = "SELECT COUNT(*) FROM UserAchievements WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(achievementQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        int achievements = Convert.ToInt32(cmd.ExecuteScalar());
                        if (achievements == 0) achievements = 1; // Default to 1
                        lblAchievementsUnlocked.Text = achievements.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading achievement stats: {ex.Message}");
                if (lblAchievementsUnlocked != null) lblAchievementsUnlocked.Text = "1";
            }
        }

        private void LoadActivityStats(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Calculate current streak
                    int currentStreak = CalculateCurrentStreak(userId, conn);
                    if (currentStreak == 0) currentStreak = 1; // Default to 1
                    lblCurrentStreak.Text = currentStreak.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading activity stats: {ex.Message}");
                if (lblCurrentStreak != null) lblCurrentStreak.Text = "1";
            }
        }

        private int CalculateCurrentStreak(int userId, SqlConnection conn)
        {
            try
            {
                // Get distinct activity dates in descending order
                string streakQuery = @"
                    SELECT DISTINCT CAST(ActivityDate as DATE) as ActivityDay
                    FROM UserActivity 
                    WHERE UserID = @UserID 
                    ORDER BY ActivityDay DESC";

                using (SqlCommand cmd = new SqlCommand(streakQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    List<DateTime> activityDays = new List<DateTime>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activityDays.Add(Convert.ToDateTime(reader["ActivityDay"]));
                        }
                    }

                    if (activityDays.Count == 0) return 0;

                    // Calculate streak
                    int streak = 0;
                    DateTime currentDate = DateTime.Today;

                    // Check if user was active today or yesterday
                    if (activityDays.Contains(currentDate))
                    {
                        streak = 1;
                        currentDate = currentDate.AddDays(-1);
                    }
                    else if (activityDays.Contains(currentDate.AddDays(-1)))
                    {
                        streak = 1;
                        currentDate = currentDate.AddDays(-2);
                    }
                    else
                    {
                        return 0; // Streak broken
                    }

                    // Count consecutive days
                    foreach (DateTime day in activityDays.Skip(streak == 1 ? 1 : 0))
                    {
                        if (day == currentDate)
                        {
                            streak++;
                            currentDate = currentDate.AddDays(-1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return streak;
                }
            }
            catch
            {
                return 0;
            }
        }

        private void SetMotivationalMessage()
        {
            try
            {
                if (lblTotalPoints == null || lblQuizzesCompleted == null || lblCurrentStreak == null || lblMotivationalMessage == null)
                {
                    System.Diagnostics.Debug.WriteLine("Some controls are null, skipping motivational message update");
                    return;
                }

                int totalPoints = Convert.ToInt32(lblTotalPoints.Text);
                int completedQuizzes = Convert.ToInt32(lblQuizzesCompleted.Text);
                int currentStreak = Convert.ToInt32(lblCurrentStreak.Text);

                string message = "Keep up the great work protecting yourself from scams!";

                if (totalPoints == 0)
                {
                    message = "Welcome! Start your scam protection journey by taking your first quiz.";
                }
                else if (currentStreak >= 7)
                {
                    message = $"Amazing! You're on a {currentStreak}-day streak! You're becoming a scam detection expert!";
                }
                else if (completedQuizzes >= 5)
                {
                    message = "Great progress! You're well on your way to becoming scam-savvy!";
                }
                else if (totalPoints >= 100)
                {
                    message = "Fantastic! Your dedication to learning is paying off!";
                }

                lblMotivationalMessage.Text = message;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting motivational message: {ex.Message}");
                if (lblMotivationalMessage != null)
                {
                    lblMotivationalMessage.Text = "Keep up the great work protecting yourself from scams!";
                }
            }
        }

        private void ShowDefaultStats()
        {
            // This method is kept for compatibility but may not be used
            try
            {
                // Show default values when database is unavailable
                if (lblTotalPoints != null) lblTotalPoints.Text = "0";
                if (lblCurrentPoints != null) lblCurrentPoints.Text = "0";
                if (lblQuizzesCompleted != null) lblQuizzesCompleted.Text = "1";
                if (lblTotalQuizzes != null) lblTotalQuizzes.Text = "6";
                if (hiddenQuizProgress != null) hiddenQuizProgress.Value = "17"; // 1/6 ≈ 17%
                if (lblCurrentStreak != null) lblCurrentStreak.Text = "1";
                if (lblAchievementsUnlocked != null) lblAchievementsUnlocked.Text = "1";
                if (lblMotivationalMessage != null) lblMotivationalMessage.Text = "Welcome! Start your scam protection journey today.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ShowDefaultStats: {ex.Message}");
            }
        }
    }
}