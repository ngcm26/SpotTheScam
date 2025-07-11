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

namespace SpotTheScam.User
{
    public partial class PhoneScamsQuiz : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in using Username session variable
                if (Session["Username"] == null || string.IsNullOrEmpty(Session["Username"].ToString()))
                {
                    Response.Redirect("UserLogin.aspx");
                    return;
                }

                // Initialize quiz
                InitializeQuiz();
                LoadUserPoints();
            }
            // Don't reload points on postback - let JavaScript maintain them
        }

        private void InitializeQuiz()
        {
            // Set quiz metadata
            lblTotalQuestions.Text = "10";
            lblCurrentQuestion.Text = "1";

            // Initialize hidden fields
            hdnCurrentQuestionIndex.Value = "0";
            hdnSelectedAnswer.Value = "";
            hdnHintUsed.Value = "false";

            // Check if user has already taken this quiz
            CheckQuizProgress();
        }

        private void LoadUserPoints()
        {
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from Username first
                    string getUserIdQuery = "SELECT UserId FROM Users WHERE Username = @Username";
                    int userId = 0;

                    using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                        object userIdResult = cmd.ExecuteScalar();
                        if (userIdResult != null)
                        {
                            userId = Convert.ToInt32(userIdResult);
                            // Store UserID in session for use in WebMethods
                            Session["UserID"] = userId;
                            System.Diagnostics.Debug.WriteLine($"User found: {Session["Username"]}, UserID: {userId}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"User not found: {Session["Username"]}");
                            // User not found, redirect to login
                            Response.Redirect("UserLogin.aspx");
                            return;
                        }
                    }

                    // Now get points for this user
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
                            lblCurrentPoints.Text = result.ToString();
                            System.Diagnostics.Debug.WriteLine($"Total points loaded: {result}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and set default points
                System.Diagnostics.Debug.WriteLine($"Error loading user points: {ex.Message}");
                lblCurrentPoints.Text = "0";
            }
        }

        private int GetCurrentUserId()
        {
            // First check if UserID is in session
            if (Session["UserID"] != null)
            {
                return Convert.ToInt32(Session["UserID"]);
            }

            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string getUserIdQuery = "SELECT UserId FROM Users WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            int userId = Convert.ToInt32(result);
                            Session["UserID"] = userId; // Store for future use
                            return userId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error: System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return 0;
        }

        private void CheckQuizProgress()
        {
            try
            {
                int userId = GetCurrentUserId();
                if (userId == 0) return;

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get quiz ID for Phone Scams
                    string getQuizIdQuery = "SELECT QuizId FROM Quizzes WHERE QuizTitle = 'Phone Scams'";
                    int quizId = 0;

                    using (SqlCommand cmd = new SqlCommand(getQuizIdQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            quizId = Convert.ToInt32(result);
                        }
                        else
                        {
                            // Create quiz record if it doesn't exist
                            CreateQuizRecord(conn);
                            using (SqlCommand cmd2 = new SqlCommand(getQuizIdQuery, conn))
                            {
                                result = cmd2.ExecuteScalar();
                                if (result != null)
                                {
                                    quizId = Convert.ToInt32(result);
                                }
                            }
                        }
                    }

                    // Check if user has already completed this quiz
                    string checkCompletionQuery = @"
                        SELECT COUNT(*) 
                        FROM UserQuizSessions 
                        WHERE UserId = @UserId AND QuizId = @QuizId AND SessionStatus = 'Completed'";

                    using (SqlCommand cmd = new SqlCommand(checkCompletionQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@QuizId", quizId);

                        int completedCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (completedCount > 0)
                        {
                            // User has already completed this quiz - show retake message
                            string script = @"
                                if (confirm('You have already completed this quiz! You can retake it for practice, but you won\'t earn points again. Do you want to continue?')) {
                                    // Continue with quiz
                                } else {
                                    window.location.href = 'Quizzes.aspx';
                                }
                            ";
                            ClientScript.RegisterStartupScript(this.GetType(), "RetakeAlert", script, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with quiz
                // System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void CreateQuizRecord(SqlConnection conn)
        {
            string createQuizQuery = @"
                INSERT INTO Quizzes (CategoryId, QuizTitle, QuizDescription, TotalQuestions, PointsPerCorrect, BonusPointsNoHint, TimeLimit, CreatedDate)
                VALUES (@CategoryId, @QuizTitle, @QuizDescription, @TotalQuestions, @PointsPerCorrect, @BonusPointsNoHint, @TimeLimit, @CreatedDate)";

            using (SqlCommand cmd = new SqlCommand(createQuizQuery, conn))
            {
                // Get or create category for Phone Scams (assuming CategoryId 1 for beginner)
                cmd.Parameters.AddWithValue("@CategoryId", 1);
                cmd.Parameters.AddWithValue("@QuizTitle", "Phone Scams");
                cmd.Parameters.AddWithValue("@QuizDescription", "Learn to identify suspicious calls and protect yourself from phone scams");
                cmd.Parameters.AddWithValue("@TotalQuestions", 10);
                cmd.Parameters.AddWithValue("@PointsPerCorrect", 10);
                cmd.Parameters.AddWithValue("@BonusPointsNoHint", 5);
                cmd.Parameters.AddWithValue("@TimeLimit", 600); // 10 minutes
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }

        [System.Web.Services.WebMethod]
        public static string SaveQuizProgress(int questionNumber, string selectedAnswer, bool isCorrect, bool hintUsed, int pointsEarned)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context.Session["UserID"] == null)
                {
                    return "Error: User not logged in";
                }

                int userId = Convert.ToInt32(context.Session["UserID"]);

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get quiz ID
                    string getQuizIdQuery = "SELECT QuizId FROM Quizzes WHERE QuizTitle = 'Phone Scams'";
                    int quizId = 0;

                    using (SqlCommand cmd = new SqlCommand(getQuizIdQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            quizId = Convert.ToInt32(result);
                        }
                        else
                        {
                            return "Error: Quiz not found";
                        }
                    }

                    // Get or create session
                    int sessionId = GetOrCreateSession(conn, userId, quizId);

                    // Check if this question was already answered (prevent duplicates)
                    string checkExistingQuery = @"
                        SELECT COUNT(*) FROM UserQuizAnswers 
                        WHERE SessionId = @SessionId AND QuestionId = @QuestionId";

                    using (SqlCommand cmd = new SqlCommand(checkExistingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@QuestionId", questionNumber);

                        int existingCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (existingCount > 0)
                        {
                            return "Already answered"; // Don't save duplicate answers
                        }
                    }

                    // Save user answer
                    string saveAnswerQuery = @"
                        INSERT INTO UserQuizAnswers (SessionId, QuestionId, SelectedOption, IsCorrect, HintUsed, PointsEarned, AnswerTime)
                        VALUES (@SessionId, @QuestionId, @SelectedOption, @IsCorrect, @HintUsed, @PointsEarned, @AnswerTime)";

                    using (SqlCommand cmd = new SqlCommand(saveAnswerQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@QuestionId", questionNumber);
                        cmd.Parameters.AddWithValue("@SelectedOption", selectedAnswer);
                        cmd.Parameters.AddWithValue("@IsCorrect", isCorrect);
                        cmd.Parameters.AddWithValue("@HintUsed", hintUsed);
                        cmd.Parameters.AddWithValue("@PointsEarned", pointsEarned);
                        cmd.Parameters.AddWithValue("@AnswerTime", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }

                    // Add points transaction if points were earned
                    if (pointsEarned > 0)
                    {
                        AddPointsTransaction(conn, userId, pointsEarned, $"Phone Scams Quiz - Question {questionNumber}");
                    }

                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        private static int GetOrCreateSession(SqlConnection conn, int userId, int quizId)
        {
            // Check for existing active session
            string checkSessionQuery = @"
                SELECT SessionId 
                FROM UserQuizSessions 
                WHERE UserId = @UserId AND QuizId = @QuizId AND SessionStatus = 'InProgress'";

            using (SqlCommand cmd = new SqlCommand(checkSessionQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@QuizId", quizId);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
            }

            // Create new session
            string createSessionQuery = @"
                INSERT INTO UserQuizSessions (UserId, QuizId, StartTime, SessionStatus)
                OUTPUT INSERTED.SessionId
                VALUES (@UserId, @QuizId, @StartTime, @SessionStatus)";

            using (SqlCommand cmd = new SqlCommand(createSessionQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@QuizId", quizId);
                cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@SessionStatus", "InProgress");

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static void AddPointsTransaction(SqlConnection conn, int userId, int points, string description)
        {
            try
            {
                string addPointsQuery = @"
                    INSERT INTO PointsTransactions (UserId, TransactionType, Points, Description, TransactionDate)
                    VALUES (@UserId, @TransactionType, @Points, @Description, @TransactionDate)";

                using (SqlCommand cmd = new SqlCommand(addPointsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@TransactionType", "Quiz");
                    cmd.Parameters.AddWithValue("@Points", points);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine($"Points transaction saved: {rowsAffected} rows affected. UserId: {userId}, Points: {points}, Description: {description}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving points transaction: {ex.Message}");
                throw; // Re-throw to let calling method handle it
            }
        }

        [System.Web.Services.WebMethod]
        public static string CompleteQuiz(int totalPointsEarned, int correctAnswers)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context.Session["UserID"] == null)
                {
                    return "Error: User not logged in";
                }

                int userId = Convert.ToInt32(context.Session["UserID"]);

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get quiz and session info
                    string getQuizIdQuery = "SELECT QuizId FROM Quizzes WHERE QuizTitle = 'Phone Scams'";
                    object quizIdResult = new SqlCommand(getQuizIdQuery, conn).ExecuteScalar();
                    if (quizIdResult == null) return "Error: Quiz not found";

                    int quizId = Convert.ToInt32(quizIdResult);
                    int sessionId = GetOrCreateSession(conn, userId, quizId);

                    // Update session as completed
                    string updateSessionQuery = @"
                        UPDATE UserQuizSessions 
                        SET EndTime = @EndTime, 
                            TotalQuestions = @TotalQuestions, 
                            CorrectAnswers = @CorrectAnswers, 
                            TotalPointsEarned = @TotalPointsEarned,
                            SessionStatus = @SessionStatus
                        WHERE SessionId = @SessionId";

                    using (SqlCommand cmd = new SqlCommand(updateSessionQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EndTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@TotalQuestions", 10);
                        cmd.Parameters.AddWithValue("@CorrectAnswers", correctAnswers);
                        cmd.Parameters.AddWithValue("@TotalPointsEarned", totalPointsEarned);
                        cmd.Parameters.AddWithValue("@SessionStatus", "Completed");
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);

                        cmd.ExecuteNonQuery();
                    }

                    // Check if this is first completion for bonus points
                    string checkFirstTimeQuery = @"
                        SELECT COUNT(*) 
                        FROM UserQuizSessions 
                        WHERE UserId = @UserId AND QuizId = @QuizId AND SessionStatus = 'Completed'";

                    using (SqlCommand cmd = new SqlCommand(checkFirstTimeQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@QuizId", quizId);

                        int completedCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (completedCount == 1) // First completion
                        {
                            int completionBonus = 10;
                            int firstTimeBonus = 20;
                            int perfectScoreBonus = (correctAnswers == 10) ? 50 : 0;

                            int totalBonus = completionBonus + firstTimeBonus + perfectScoreBonus;

                            if (totalBonus > 0)
                            {
                                AddPointsTransaction(conn, userId, totalBonus, "Phone Scams Quiz - Completion Bonus");
                            }
                        }
                    }

                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}