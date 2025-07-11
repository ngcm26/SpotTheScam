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

                // Load user's CURRENT total points from database (separate from quiz points)
                LoadUserCurrentTotalPoints();

                // Generate achievement badges based on performance
                GenerateAchievementBadges(correctAnswers, totalQuestions, pointsEarnedFromQuiz);
            }
            catch (Exception ex)
            {
                // Log error and set default values
                lblQuizName.Text = "Quiz";
                lblPointsEarned.Text = "0";
                lblCorrectAnswers.Text = "0";
                lblTotalQuestions.Text = "10";
                lblAccuracy.Text = "0";
                lblTotalPoints.Text = "0";
            }
        }

        private void LoadUserCurrentTotalPoints()
        {
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from Username
                    string getUserIdQuery = "SELECT UserId FROM Users WHERE Username = @Username";
                    int userId = 0;

                    using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                        object userIdResult = cmd.ExecuteScalar();
                        if (userIdResult != null)
                        {
                            userId = Convert.ToInt32(userIdResult);
                        }
                    }

                    if (userId > 0)
                    {
                        // Get CURRENT total points for this user (all their points combined)
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
                                lblTotalPoints.Text = result.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                lblTotalPoints.Text = "0";
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