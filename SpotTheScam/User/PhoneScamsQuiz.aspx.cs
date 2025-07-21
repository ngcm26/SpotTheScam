using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Configuration;
using System.Web.Services;

namespace SpotTheScam.User
{
    public partial class PhoneScamsQuiz : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Username"] == null || string.IsNullOrEmpty(Session["Username"].ToString()))
                {
                    Response.Redirect("UserLogin.aspx");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"=== DEBUGGING PhoneScamsQuiz Page_Load ===");
                System.Diagnostics.Debug.WriteLine($"Session Username: '{Session["Username"]}'");

                // Load user's current points from PointsTransactions table
                LoadUserCurrentPoints();
                InitializeQuiz();
            }
        }

        private void LoadUserCurrentPoints()
        {
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("✅ Database connection successful");

                    // Get UserID from Username - using CORRECT column name "Id"
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
                            System.Diagnostics.Debug.WriteLine($"❌ User not found: {Session["Username"]}");
                            lblCurrentPoints.Text = "User not found";
                            return;
                        }
                    }

                    // Get current total points from PointsTransactions table (authoritative source)
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

                        System.Diagnostics.Debug.WriteLine($"✅ Current points loaded from PointsTransactions: {currentUserPoints}");
                        lblCurrentPoints.Text = currentUserPoints.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DATABASE ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                lblCurrentPoints.Text = "Error loading points";
            }

            System.Diagnostics.Debug.WriteLine($"🔍 FINAL CHECK - lblCurrentPoints.Text: '{lblCurrentPoints.Text}'");
            System.Diagnostics.Debug.WriteLine($"🔍 FINAL CHECK - Session CurrentPoints: {Session["CurrentPoints"]}");
        }

        private void InitializeQuiz()
        {
            // Initialize quiz values
            lblTotalQuestions.Text = "10";
            lblCurrentQuestion.Text = "1";

            // Initialize hidden fields
            hdnCurrentQuestionIndex.Value = "0";
            hdnSelectedAnswer.Value = "";
            hdnHintUsed.Value = "false";

            System.Diagnostics.Debug.WriteLine("Quiz initialized successfully");
        }

        [WebMethod]
        public static string SaveQuizProgress(int questionNumber, string selectedAnswer, bool isCorrect, bool hintUsed, int pointsEarned)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context.Session["Username"] == null)
                {
                    return "Error: User not logged in";
                }

                // Get userId from session or database
                int userId = 0;
                if (context.Session["UserID"] != null)
                {
                    userId = Convert.ToInt32(context.Session["UserID"]);
                }
                else
                {
                    // Look up user ID
                    string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";
                        using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", context.Session["Username"]);
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                userId = Convert.ToInt32(result);
                                context.Session["UserID"] = userId;
                            }
                            else
                            {
                                return "Error: User not found";
                            }
                        }
                    }
                }

                string connectionString2 = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString2))
                {
                    conn.Open();

                    // CRITICAL: If points were earned, add to PointsTransactions
                    if (pointsEarned > 0)
                    {
                        string addPointsQuery = @"
                            INSERT INTO PointsTransactions (UserId, TransactionType, Points, Description, TransactionDate)
                            VALUES (@UserId, @TransactionType, @Points, @Description, @TransactionDate)";

                        using (SqlCommand pointsCmd = new SqlCommand(addPointsQuery, conn))
                        {
                            pointsCmd.Parameters.AddWithValue("@UserId", userId);
                            pointsCmd.Parameters.AddWithValue("@TransactionType", "Quiz Question");
                            pointsCmd.Parameters.AddWithValue("@Points", pointsEarned);
                            pointsCmd.Parameters.AddWithValue("@Description", $"Phone Scams Quiz - Question {questionNumber}");
                            pointsCmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);

                            int rowsAffected = pointsCmd.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine($"✅ Added to PointsTransactions: UserId={userId}, +{pointsEarned} points, rows affected: {rowsAffected}");
                        }

                        // Update session with new total
                        string getNewTotalQuery = @"
                            SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                            FROM PointsTransactions 
                            WHERE UserId = @UserId";
                        using (SqlCommand cmd = new SqlCommand(getNewTotalQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                int newTotal = Convert.ToInt32(result);
                                context.Session["CurrentPoints"] = newTotal;
                                System.Diagnostics.Debug.WriteLine($"✅ Updated session CurrentPoints to: {newTotal}");
                            }
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"📝 Quiz progress saved: Question {questionNumber}, Answer: {selectedAnswer}, Correct: {isCorrect}, Points: {pointsEarned}");
                }

                return "Success";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error saving quiz progress: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        [WebMethod]
        public static string CompleteQuiz(int totalPointsEarned, int correctAnswers)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context.Session["Username"] == null)
                {
                    return "Error: User not logged in";
                }

                int userId = Convert.ToInt32(context.Session["UserID"]);
                System.Diagnostics.Debug.WriteLine($"🏁 Quiz completed: UserId={userId}, Points={totalPointsEarned}, Correct={correctAnswers}/10");

                return "Success";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error completing quiz: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}