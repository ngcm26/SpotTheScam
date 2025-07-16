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
                // Check if user is logged in using Username session variable
                if (Session["Username"] == null || string.IsNullOrEmpty(Session["Username"].ToString()))
                {
                    Response.Redirect("UserLogin.aspx");
                    return;
                }

                // CRITICAL: Ensure quiz exists before starting
                EnsureQuizAndCategoryExist();

                // Initialize quiz
                InitializeQuiz();
                LoadUserPoints();

                // Debug session state
                System.Diagnostics.Debug.WriteLine($"Page_Load - Session UserID: {Session["UserID"]}");
                System.Diagnostics.Debug.WriteLine($"Page_Load - Session Username: {Session["Username"]}");
            }
        }

        private void EnsureQuizAndCategoryExist()
        {
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First ensure category exists
                    int categoryId = EnsureCategoryExists(conn);
                    System.Diagnostics.Debug.WriteLine($"Category ID ensured: {categoryId}");

                    // Then check if "Phone Scams" quiz exists
                    string checkQuizQuery = "SELECT COUNT(*) FROM Quizzes WHERE QuizTitle = 'Phone Scams'";
                    using (SqlCommand cmd = new SqlCommand(checkQuizQuery, conn))
                    {
                        int quizCount = Convert.ToInt32(cmd.ExecuteScalar());
                        System.Diagnostics.Debug.WriteLine($"Phone Scams quiz count: {quizCount}");

                        if (quizCount == 0)
                        {
                            // Create the quiz record with the correct category
                            System.Diagnostics.Debug.WriteLine("Creating Phone Scams quiz record...");
                            CreateQuizRecord(conn, categoryId);
                            System.Diagnostics.Debug.WriteLine("Phone Scams quiz record created successfully");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Phone Scams quiz already exists");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ensuring quiz exists: {ex.Message}");
                // Don't throw - let the page continue
            }
        }

        private int EnsureCategoryExists(SqlConnection conn)
        {
            try
            {
                // Check if any category exists
                string checkCategoryQuery = "SELECT TOP 1 CategoryId FROM QuizCategories";
                using (SqlCommand cmd = new SqlCommand(checkCategoryQuery, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        int existingId = Convert.ToInt32(result);
                        System.Diagnostics.Debug.WriteLine($"Found existing category: {existingId}");
                        return existingId;
                    }
                }

                // If no category exists, create one
                System.Diagnostics.Debug.WriteLine("No category found, creating default category...");
                string createCategoryQuery = @"
                    INSERT INTO QuizCategories (CategoryName, Description, CreatedDate)
                    OUTPUT INSERTED.CategoryId
                    VALUES (@CategoryName, @Description, @CreatedDate)";

                using (SqlCommand cmd = new SqlCommand(createCategoryQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryName", "Security Awareness");
                    cmd.Parameters.AddWithValue("@Description", "Quizzes to test knowledge about security and scam prevention");
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    int newCategoryId = Convert.ToInt32(cmd.ExecuteScalar());
                    System.Diagnostics.Debug.WriteLine($"Created new category with ID: {newCategoryId}");
                    return newCategoryId;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ensuring category exists: {ex.Message}");
                // Return 1 as fallback - this should work in most cases
                return 1;
            }
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
                System.Diagnostics.Debug.WriteLine($"=== LoadUserPoints START ===");
                System.Diagnostics.Debug.WriteLine($"Session Username: {Session["Username"]}");

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from Username first - using correct column name
                    string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";
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
                            Response.Redirect("UserLogin.aspx");
                            return;
                        }
                    }

                    // Get points from PointsTransactions with fresh read to avoid caching issues
                    int pointsFromTransactions = 0;
                    int maxRetries = 5;

                    for (int retry = 0; retry < maxRetries; retry++)
                    {
                        System.Diagnostics.Debug.WriteLine($"Attempt {retry + 1} to get user points...");

                        // Force fresh read by using WITH (NOLOCK) and ORDER BY to ensure we get latest data
                        string query = @"
                            SELECT ISNULL(SUM(Points), 0) as TotalPoints 
                            FROM PointsTransactions WITH (NOLOCK)
                            WHERE UserId = @UserId";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            object result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                int currentPoints = Convert.ToInt32(result);
                                System.Diagnostics.Debug.WriteLine($"Attempt {retry + 1}: Points found: {currentPoints}");

                                // If this is a reasonable result (not negative), use it
                                if (currentPoints >= 0)
                                {
                                    pointsFromTransactions = currentPoints;

                                    // If we got a result that's different from previous attempts or this is the last try, use it
                                    if (retry == 0 || currentPoints != pointsFromTransactions || retry == maxRetries - 1)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        // If this isn't the last retry, wait a bit before trying again
                        if (retry < maxRetries - 1)
                        {
                            System.Threading.Thread.Sleep(500); // Wait 0.5 seconds
                        }
                    }

                    lblCurrentPoints.Text = pointsFromTransactions.ToString();
                    System.Diagnostics.Debug.WriteLine($"Final total points loaded: {pointsFromTransactions}");

                    // Debug: Show recent transactions for this user to verify data
                    string debugQuery = @"
                        SELECT TOP 15 TransactionType, Points, Description, TransactionDate
                        FROM PointsTransactions WITH (NOLOCK)
                        WHERE UserId = @UserId
                        ORDER BY TransactionDate DESC";

                    using (SqlCommand cmd = new SqlCommand(debugQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            System.Diagnostics.Debug.WriteLine("=== Recent Transactions (Quiz Start) ===");
                            int transactionCount = 0;
                            int totalCalculated = 0;
                            while (reader.Read())
                            {
                                transactionCount++;
                                int points = Convert.ToInt32(reader["Points"]);
                                totalCalculated += points;
                                System.Diagnostics.Debug.WriteLine($"{transactionCount}. {reader["TransactionType"]}: {points} points - {reader["Description"]} ({reader["TransactionDate"]})");
                            }
                            System.Diagnostics.Debug.WriteLine($"Total transactions found: {transactionCount}");
                            System.Diagnostics.Debug.WriteLine($"Manual calculation of recent transactions: {totalCalculated}");
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"=== LoadUserPoints END - lblCurrentPoints.Text: {lblCurrentPoints.Text} ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user points: {ex.Message}");
                lblCurrentPoints.Text = "0";
            }
        }

        private int GetCurrentUserId()
        {
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
                    string getUserIdQuery = "SELECT Id FROM Users WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            int userId = Convert.ToInt32(result);
                            Session["UserID"] = userId;
                            return userId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting user ID: {ex.Message}");
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
                            System.Diagnostics.Debug.WriteLine($"Found Quiz ID: {quizId}");
                        }
                    }

                    if (quizId > 0)
                    {
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking quiz progress: {ex.Message}");
            }
        }

        private void CreateQuizRecord(SqlConnection conn, int categoryId)
        {
            string createQuizQuery = @"
                INSERT INTO Quizzes (CategoryId, QuizTitle, QuizDescription, TotalQuestions, PointsPerCorrect, BonusPointsNoHint, TimeLimit, CreatedDate)
                VALUES (@CategoryId, @QuizTitle, @QuizDescription, @TotalQuestions, @PointsPerCorrect, @BonusPointsNoHint, @TimeLimit, @CreatedDate)";

            using (SqlCommand cmd = new SqlCommand(createQuizQuery, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                cmd.Parameters.AddWithValue("@QuizTitle", "Phone Scams");
                cmd.Parameters.AddWithValue("@QuizDescription", "Learn to identify suspicious calls and protect yourself from phone scams");
                cmd.Parameters.AddWithValue("@TotalQuestions", 10);
                cmd.Parameters.AddWithValue("@PointsPerCorrect", 10);
                cmd.Parameters.AddWithValue("@BonusPointsNoHint", 5);
                cmd.Parameters.AddWithValue("@TimeLimit", 600);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                cmd.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine("Quiz record created successfully");
            }
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string SaveQuizProgress(int questionNumber, string selectedAnswer, bool isCorrect, bool hintUsed, int pointsEarned)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context.Session["UserID"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("SaveQuizProgress: Session UserID is null");
                    return "Error: User not logged in";
                }

                int userId = Convert.ToInt32(context.Session["UserID"]);
                System.Diagnostics.Debug.WriteLine($"=== SaveQuizProgress called ===");
                System.Diagnostics.Debug.WriteLine($"UserID: {userId}, Question: {questionNumber}, Answer: {selectedAnswer}, Correct: {isCorrect}, Points: {pointsEarned}");

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get quiz ID - it should exist now
                    string getQuizIdQuery = "SELECT QuizId FROM Quizzes WHERE QuizTitle = 'Phone Scams'";
                    int quizId = 0;

                    using (SqlCommand cmd = new SqlCommand(getQuizIdQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            quizId = Convert.ToInt32(result);
                            System.Diagnostics.Debug.WriteLine($"Quiz ID found: {quizId}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Quiz not found in SaveQuizProgress");
                            return "Error: Quiz not found";
                        }
                    }

                    // Get or create session
                    int sessionId = GetOrCreateSession(conn, userId, quizId);
                    System.Diagnostics.Debug.WriteLine($"Session ID: {sessionId}");

                    // Check if this question was already answered
                    string checkExistingQuery = @"
                        SELECT COUNT(*) FROM UserQuizAnswers 
                        WHERE SessionId = @SessionId AND QuestionId = @QuestionNumber";

                    using (SqlCommand cmd = new SqlCommand(checkExistingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@QuestionNumber", questionNumber);

                        int existingCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (existingCount > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Question already answered - skipping");
                            return "Already answered";
                        }
                    }

                    // Save user answer
                    string saveAnswerQuery = @"
                        INSERT INTO UserQuizAnswers (SessionId, QuestionId, SelectedOption, IsCorrect, HintUsed, PointsEarned, AnswerTime)
                        VALUES (@SessionId, @QuestionNumber, @SelectedOption, @IsCorrect, @HintUsed, @PointsEarned, @AnswerTime)";

                    using (SqlCommand cmd = new SqlCommand(saveAnswerQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@QuestionNumber", questionNumber);
                        cmd.Parameters.AddWithValue("@SelectedOption", selectedAnswer);
                        cmd.Parameters.AddWithValue("@IsCorrect", isCorrect);
                        cmd.Parameters.AddWithValue("@HintUsed", hintUsed);
                        cmd.Parameters.AddWithValue("@PointsEarned", pointsEarned);
                        cmd.Parameters.AddWithValue("@AnswerTime", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Answer saved: {rowsAffected} rows affected");
                    }

                    // Add points transaction if points were earned
                    if (pointsEarned > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Adding points transaction: {pointsEarned} points");
                        bool success = AddPointsTransaction(conn, userId, pointsEarned, $"Phone Scams Quiz - Question {questionNumber}");
                        System.Diagnostics.Debug.WriteLine($"Points transaction success: {success}");
                    }

                    return "Success";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SaveQuizProgress: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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

        private static bool AddPointsTransaction(SqlConnection conn, int userId, int points, string description)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== AddPointsTransaction START ===");
                System.Diagnostics.Debug.WriteLine($"UserId: {userId}, Points: {points}, Description: {description}");

                // Insert the transaction
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
                    System.Diagnostics.Debug.WriteLine($"Points transaction saved: {rowsAffected} rows affected");

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving points transaction: {ex.Message}");
                return false;
            }
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string CompleteQuiz(int totalPointsEarned, int correctAnswers)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context.Session["UserID"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("CompleteQuiz: Session UserID is null");
                    return "Error: User not logged in";
                }

                int userId = Convert.ToInt32(context.Session["UserID"]);
                System.Diagnostics.Debug.WriteLine($"=== CompleteQuiz called ===");
                System.Diagnostics.Debug.WriteLine($"UserID: {userId}, TotalPoints: {totalPointsEarned}, CorrectAnswers: {correctAnswers}");

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get quiz ID - should exist now
                    string getQuizIdQuery = "SELECT QuizId FROM Quizzes WHERE QuizTitle = 'Phone Scams'";
                    object quizIdResult = new SqlCommand(getQuizIdQuery, conn).ExecuteScalar();

                    int quizId = 0;
                    if (quizIdResult != null)
                    {
                        quizId = Convert.ToInt32(quizIdResult);
                        System.Diagnostics.Debug.WriteLine($"Quiz ID found: {quizId}");
                    }

                    // Check if user has already completed this quiz to avoid duplicate bonuses
                    string checkCompletionQuery = @"
                        SELECT COUNT(*) 
                        FROM UserQuizSessions 
                        WHERE UserId = @UserId AND QuizId = @QuizId AND SessionStatus = 'Completed'";

                    bool alreadyCompleted = false;
                    using (SqlCommand cmd = new SqlCommand(checkCompletionQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@QuizId", quizId);
                        int completedCount = Convert.ToInt32(cmd.ExecuteScalar());
                        alreadyCompleted = (completedCount > 0);
                        System.Diagnostics.Debug.WriteLine($"Already completed count: {completedCount}");
                    }

                    // Get or create session
                    int sessionId = 0;
                    if (quizId > 0)
                    {
                        sessionId = GetOrCreateSession(conn, userId, quizId);
                        System.Diagnostics.Debug.WriteLine($"Session ID: {sessionId}");

                        // Calculate total points from quiz including bonuses
                        int totalPointsIncludingBonuses = totalPointsEarned;

                        // Calculate and add completion bonuses (only if not already completed)
                        if (!alreadyCompleted)
                        {
                            int completionBonus = 10;
                            int firstTimeBonus = 20;
                            int perfectScoreBonus = (correctAnswers == 10) ? 50 : 0;
                            int totalBonus = completionBonus + firstTimeBonus + perfectScoreBonus;

                            totalPointsIncludingBonuses += totalBonus;

                            if (totalBonus > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"Adding completion bonus: {totalBonus} points");

                                string bonusDescription = "Phone Scams Quiz - Completion Bonus";
                                if (perfectScoreBonus > 0)
                                {
                                    bonusDescription += " (Perfect Score!)";
                                }

                                bool bonusSuccess = AddPointsTransaction(conn, userId, totalBonus, bonusDescription);
                                System.Diagnostics.Debug.WriteLine($"Bonus points success: {bonusSuccess}");

                                if (!bonusSuccess)
                                {
                                    System.Diagnostics.Debug.WriteLine("Warning: Failed to add bonus points");
                                }
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Quiz already completed - no bonus points awarded");
                        }

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
                            cmd.Parameters.AddWithValue("@TotalPointsEarned", totalPointsIncludingBonuses);
                            cmd.Parameters.AddWithValue("@SessionStatus", "Completed");
                            cmd.Parameters.AddWithValue("@SessionId", sessionId);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine($"Session updated: {rowsAffected} rows affected");
                        }
                    }

                    return "Success";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CompleteQuiz: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return "Error: " + ex.Message;
            }
        }
    }
}