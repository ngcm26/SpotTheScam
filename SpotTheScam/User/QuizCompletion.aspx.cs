using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace SpotTheScam.User
{
    public partial class QuizCompletion : System.Web.UI.Page
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

                System.Diagnostics.Debug.WriteLine($"=== QuizCompletion Page_Load ===");
                System.Diagnostics.Debug.WriteLine($"Session Username: {Session["Username"]}");

                // Add cache-busting headers to prevent caching of old points data
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();

                // Load completion data from query parameters
                LoadCompletionData();
            }
        }

        private void LoadCompletionData()
        {
            try
            {
                // Get data from query parameters
                string quizName = Request.QueryString["quiz"] ?? "Phone Scams Quiz";
                int pointsEarnedFromQuiz = int.Parse(Request.QueryString["points"] ?? "0");
                int correctAnswers = int.Parse(Request.QueryString["correct"] ?? "0");
                int totalQuestions = int.Parse(Request.QueryString["total"] ?? "10");
                int currentPointsFromJS = int.Parse(Request.QueryString["currentPoints"] ?? "0");

                System.Diagnostics.Debug.WriteLine($"=== QuizCompletion Data ===");
                System.Diagnostics.Debug.WriteLine($"Quiz: {quizName}");
                System.Diagnostics.Debug.WriteLine($"Points earned from quiz: {pointsEarnedFromQuiz}");
                System.Diagnostics.Debug.WriteLine($"Correct answers: {correctAnswers}");
                System.Diagnostics.Debug.WriteLine($"Total questions: {totalQuestions}");
                System.Diagnostics.Debug.WriteLine($"Current points from JS: {currentPointsFromJS}");

                // Update labels with quiz data
                lblQuizName.Text = quizName;
                lblPointsEarned.Text = pointsEarnedFromQuiz.ToString();
                lblCorrectAnswers.Text = correctAnswers.ToString();
                lblTotalQuestions.Text = totalQuestions.ToString();

                // Calculate and display accuracy
                if (totalQuestions > 0)
                {
                    int accuracy = (int)Math.Round((double)correctAnswers / totalQuestions * 100);
                    lblAccuracy.Text = accuracy.ToString();
                }

                // CRITICAL: Always load the FRESH current points from database, not from JavaScript
                LoadUserCurrentPointsWithRetry();

                // Generate achievement badges based on performance
                GenerateAchievementBadges(correctAnswers, totalQuestions, pointsEarnedFromQuiz);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading completion data: {ex.Message}");
                // Set default values on error
                lblQuizName.Text = "Quiz";
                lblPointsEarned.Text = "0";
                lblCorrectAnswers.Text = "0";
                lblTotalQuestions.Text = "0";
                lblAccuracy.Text = "0";
                lblCurrentPoints.Text = "0";
            }
        }

        // FIXED METHOD: Load user's current total points from database with retry logic

        // CORRECTED LoadUserCurrentPointsWithRetry method for QuizCompletion.aspx.cs
        // Replace the existing method with this:

        private void LoadUserCurrentPointsWithRetry()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== LoadUserCurrentPointsWithRetry START ===");

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID
                    int userId = 0;
                    if (Session["UserID"] != null)
                    {
                        userId = Convert.ToInt32(Session["UserID"]);
                    }
                    else
                    {
                        string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";
                        using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", Session["Username"].ToString());
                            object userIdResult = cmd.ExecuteScalar();
                            if (userIdResult != null)
                            {
                                userId = Convert.ToInt32(userIdResult);
                                Session["UserID"] = userId;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("User not found in database");
                                lblCurrentPoints.Text = "0";
                                return;
                            }
                        }
                    }

                    // CRITICAL: Load from PointsTransactions table (same as other pages)
                    string getPointsQuery = @"
                SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                FROM PointsTransactions 
                WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(getPointsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        int currentTotalPoints = result != null ? Convert.ToInt32(result) : 0;

                        // Set the label text with TOTAL points
                        lblCurrentPoints.Text = currentTotalPoints.ToString();
                        Session["CurrentPoints"] = currentTotalPoints;

                        System.Diagnostics.Debug.WriteLine($"✅ Loaded TOTAL points from PointsTransactions: {currentTotalPoints}");
                        System.Diagnostics.Debug.WriteLine($"✅ Set lblCurrentPoints.Text to: '{lblCurrentPoints.Text}'");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading current points: {ex.Message}");
                lblCurrentPoints.Text = "0";
            }
        }
        private void GenerateAchievementBadges(int correctAnswers, int totalQuestions, int pointsEarned)
        {
            var badges = new List<string>();

            // Calculate accuracy
            double accuracy = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;

            // Points Master badge - earned points in this quiz
            if (pointsEarned >= 100)
            {
                badges.Add("Points Master!");
            }

            // Quiz Completed badge - always earned for completing
            badges.Add("Quiz Completed!");

            // Perfect Score badge
            if (accuracy == 100)
            {
                badges.Add("Perfect Score!");
            }

            // High Scorer badge
            else if (accuracy >= 80)
            {
                badges.Add("High Scorer!");
            }

            // First Timer badge (you might want to check if this is their first quiz)
            // This would require checking UserQuizSessions table for previous completed quizzes

            // Generate HTML for badges
            if (badges.Count > 0)
            {
                string badgeHtml = "<div class='achievements-container'>";
                foreach (string badge in badges)
                {
                    string badgeClass = GetBadgeClass(badge);
                    badgeHtml += $"<span class='achievement-badge {badgeClass}'>";
                    badgeHtml += $"<i class='badge-icon'></i> {badge}";
                    badgeHtml += "</span>";
                }
                badgeHtml += "</div>";

                // Add to page
                achievementBadges.InnerHtml = badgeHtml;
            }
        }

        private string GetBadgeClass(string badgeName)
        {
            switch (badgeName.ToLower())
            {
                case "points master!":
                    return "badge-gold";
                case "perfect score!":
                    return "badge-platinum";
                case "high scorer!":
                    return "badge-silver";
                case "quiz completed!":
                    return "badge-bronze";
                default:
                    return "badge-default";
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