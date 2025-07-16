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

                System.Diagnostics.Debug.WriteLine($"=== QuizCompletion Page Load ===");
                System.Diagnostics.Debug.WriteLine($"Session Username: {Session["Username"]}");
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

                // Use the current points passed from JavaScript if available, otherwise load from database
                if (currentPointsFromJS > 0)
                {
                    lblCurrentPoints.Text = currentPointsFromJS.ToString();
                    System.Diagnostics.Debug.WriteLine($"Using current points from JavaScript: {currentPointsFromJS}");
                }
                else
                {
                    // Fallback to loading from database
                    LoadUserCurrentPointsWithRetry();
                }

                // Generate achievement badges based on performance
                GenerateAchievementBadges(correctAnswers, totalQuestions, pointsEarnedFromQuiz);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadCompletionData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                // Set default values on error
                lblQuizName.Text = "Quiz";
                lblPointsEarned.Text = "0";
                lblCorrectAnswers.Text = "0";
                lblTotalQuestions.Text = "10";
                lblAccuracy.Text = "0";
                lblCurrentPoints.Text = "0";
            }
        }

        private void LoadUserCurrentPointsWithRetry()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== LoadUserCurrentPointsWithRetry START ===");
                System.Diagnostics.Debug.WriteLine($"Session Username: {Session["Username"]}");

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from Username using correct column name (Id instead of UserId)
                    string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";
                    int userId = 0;

                    using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                        object userIdResult = cmd.ExecuteScalar();
                        if (userIdResult != null)
                        {
                            userId = Convert.ToInt32(userIdResult);
                            System.Diagnostics.Debug.WriteLine($"User ID found: {userId}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("User ID not found in database");
                            lblCurrentPoints.Text = "0";
                            return;
                        }
                    }

                    if (userId > 0)
                    {
                        // Try multiple times to get the latest points (in case of database delay)
                        int currentTotalPoints = 0;
                        int maxRetries = 3;

                        for (int retry = 0; retry < maxRetries; retry++)
                        {
                            System.Diagnostics.Debug.WriteLine($"Attempt {retry + 1} to get current points...");

                            // Get the current total points from the database
                            string query = @"
                                SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                                FROM PointsTransactions 
                                WHERE UserId = @UserId";

                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                object result = cmd.ExecuteScalar();

                                if (result != null)
                                {
                                    currentTotalPoints = Convert.ToInt32(result);
                                    System.Diagnostics.Debug.WriteLine($"Attempt {retry + 1}: Current total points: {currentTotalPoints}");

                                    // If we have points, break out of retry loop
                                    if (currentTotalPoints > 0)
                                    {
                                        break;
                                    }
                                }
                            }

                            // If this isn't the last retry, wait a bit before trying again
                            if (retry < maxRetries - 1)
                            {
                                System.Threading.Thread.Sleep(1000); // Wait 1 second
                            }
                        }

                        lblCurrentPoints.Text = currentTotalPoints.ToString();
                        System.Diagnostics.Debug.WriteLine($"Final current points set to: {currentTotalPoints}");

                        // Debug: Show recent transactions for this user
                        string debugQuery = @"
                            SELECT TOP 10 TransactionType, Points, Description, TransactionDate
                            FROM PointsTransactions 
                            WHERE UserId = @UserId
                            ORDER BY TransactionDate DESC";

                        using (SqlCommand cmd = new SqlCommand(debugQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                System.Diagnostics.Debug.WriteLine("=== Recent Transactions ===");
                                int transactionCount = 0;
                                while (reader.Read())
                                {
                                    transactionCount++;
                                    System.Diagnostics.Debug.WriteLine($"{transactionCount}. {reader["TransactionType"]}: {reader["Points"]} points - {reader["Description"]} ({reader["TransactionDate"]})");
                                }
                                System.Diagnostics.Debug.WriteLine($"Total transactions shown: {transactionCount}");
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"=== LoadUserCurrentPointsWithRetry END - lblCurrentPoints.Text: {lblCurrentPoints.Text} ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading current points: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                // Log error but don't break the page
                lblCurrentPoints.Text = "0";
            }
        }

        private void GenerateAchievementBadges(int correctAnswers, int totalQuestions, int pointsEarned)
        {
            var badges = new List<string>();

            // Calculate accuracy
            double accuracy = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;

            // Perfect score badge
            if (correctAnswers == totalQuestions)
            {
                badges.Add("🏆 Perfect Score!");
            }
            // High accuracy badges
            else if (accuracy >= 90)
            {
                badges.Add("⭐ Excellent Performance!");
            }
            else if (accuracy >= 80)
            {
                badges.Add("👍 Great Job!");
            }
            else if (accuracy >= 70)
            {
                badges.Add("📈 Good Progress!");
            }

            // Points-based badges
            if (pointsEarned >= 150)
            {
                badges.Add("💎 High Scorer!");
            }
            else if (pointsEarned >= 100)
            {
                badges.Add("🎯 Points Master!");
            }

            // Special badges
            if (correctAnswers >= 8)
            {
                badges.Add("🛡️ Scam Detector!");
            }

            // Quiz completion badge
            badges.Add("🎉 Quiz Completed!");

            // Generate HTML for badges
            string badgeHtml = "";
            foreach (string badge in badges)
            {
                badgeHtml += $"<div class='badge-item'>{badge}</div>";
            }

            // Set the innerHTML of the achievement badges container
            achievementBadges.InnerHtml = badgeHtml;
        }
    }
}