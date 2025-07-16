using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.UI;

namespace SpotTheScam.User
{
    public partial class PointsDebug : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDebugInfo();
            }
        }

        private void LoadDebugInfo()
        {
            try
            {
                // Show session info
                lblUsername.Text = Session["Username"]?.ToString() ?? "Not logged in";
                lblUserId.Text = Session["UserID"]?.ToString() ?? "N/A";

                if (Session["Username"] == null)
                {
                    lblTableCheck.Text = "<span class='error'>Please log in first</span>";
                    return;
                }

                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID if not in session
                    int userId = 0;
                    if (Session["UserID"] != null)
                    {
                        userId = Convert.ToInt32(Session["UserID"]);
                    }
                    else
                    {
                        string getUserIdQuery = "SELECT UserId FROM Users WHERE Username = @Username";
                        using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", Session["Username"]);
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                userId = Convert.ToInt32(result);
                                Session["UserID"] = userId;
                                lblUserId.Text = userId.ToString();
                            }
                        }
                    }

                    // Check if tables exist
                    CheckTables(conn);

                    if (userId > 0)
                    {
                        // Check points in Users table
                        CheckUsersPoints(conn, userId);

                        // Check points in PointsTransactions table
                        CheckTransactionsPoints(conn, userId);

                        // Load recent transactions
                        LoadTransactions(conn, userId);

                        // Load quiz sessions
                        LoadSessions(conn, userId);
                    }
                }
            }
            catch (Exception ex)
            {
                lblTableCheck.Text = $"<span class='error'>Error: {ex.Message}</span>";
            }
        }

        private void CheckTables(SqlConnection conn)
        {
            try
            {
                string checkTablesQuery = @"
                    SELECT TABLE_NAME 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME IN ('Users', 'PointsTransactions', 'UserQuizSessions', 'UserQuizAnswers', 'Quizzes')
                    ORDER BY TABLE_NAME";

                using (SqlCommand cmd = new SqlCommand(checkTablesQuery, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        string tables = "<span class='success'>Existing tables: </span>";
                        while (reader.Read())
                        {
                            tables += reader["TABLE_NAME"].ToString() + ", ";
                        }
                        lblTableCheck.Text = tables.TrimEnd(',', ' ');
                    }
                }
            }
            catch (Exception ex)
            {
                lblTableCheck.Text = $"<span class='error'>Error checking tables: {ex.Message}</span>";
            }
        }

        private void CheckUsersPoints(SqlConnection conn, int userId)
        {
            try
            {
                string query = "SELECT Points FROM Users WHERE UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        lblUsersPoints.Text = $"<span class='success'>Points in Users table: {result}</span>";
                    }
                    else
                    {
                        lblUsersPoints.Text = "<span class='error'>No points found in Users table (NULL or 0)</span>";
                    }
                }
            }
            catch (Exception ex)
            {
                lblUsersPoints.Text = $"<span class='error'>Error: {ex.Message}</span>";
            }
        }

        private void CheckTransactionsPoints(SqlConnection conn, int userId)
        {
            try
            {
                // First check if table exists
                string checkTableQuery = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = 'PointsTransactions'";

                using (SqlCommand cmd = new SqlCommand(checkTableQuery, conn))
                {
                    int tableExists = Convert.ToInt32(cmd.ExecuteScalar());

                    if (tableExists == 0)
                    {
                        lblTransactionsPoints.Text = "<span class='error'>PointsTransactions table does not exist!</span>";
                        return;
                    }
                }

                // Check total points
                string query = "SELECT SUM(Points) FROM PointsTransactions WHERE UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        lblTransactionsPoints.Text = $"<span class='success'>Total points in PointsTransactions: {result}</span>";
                    }
                    else
                    {
                        lblTransactionsPoints.Text = "<span class='error'>No points found in PointsTransactions table</span>";
                    }
                }

                // Also show count of transactions
                string countQuery = "SELECT COUNT(*) FROM PointsTransactions WHERE UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(countQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    lblTransactionsPoints.Text += $"<br/>Number of transactions: {count}";
                }
            }
            catch (Exception ex)
            {
                lblTransactionsPoints.Text = $"<span class='error'>Error: {ex.Message}</span>";
            }
        }

        private void LoadTransactions(SqlConnection conn, int userId)
        {
            try
            {
                string query = @"
                    SELECT TOP 10 TransactionType, Points, Description, TransactionDate
                    FROM PointsTransactions 
                    WHERE UserId = @UserId 
                    ORDER BY TransactionDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvTransactions.DataSource = dt;
                    gvTransactions.DataBind();
                }
            }
            catch (Exception ex)
            {
                gvTransactions.DataSource = null;
                gvTransactions.DataBind();
            }
        }

        private void LoadSessions(SqlConnection conn, int userId)
        {
            try
            {
                string query = @"
                    SELECT s.SessionId, q.QuizTitle, s.StartTime, s.EndTime, s.SessionStatus, 
                           s.TotalQuestions, s.CorrectAnswers, s.TotalPointsEarned
                    FROM UserQuizSessions s
                    LEFT JOIN Quizzes q ON s.QuizId = q.QuizId
                    WHERE s.UserId = @UserId 
                    ORDER BY s.StartTime DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvSessions.DataSource = dt;
                    gvSessions.DataBind();
                }
            }
            catch (Exception ex)
            {
                gvSessions.DataSource = null;
                gvSessions.DataBind();
            }
        }

        protected void btnAddTestPoints_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    lblTestResult.Text = "<span class='error'>Please log in first</span>";
                    return;
                }

                int userId = Convert.ToInt32(Session["UserID"]);
                string connectionString = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Create PointsTransactions table if it doesn't exist
                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PointsTransactions')
                        CREATE TABLE PointsTransactions (
                            TransactionId int IDENTITY(1,1) PRIMARY KEY,
                            UserId int NOT NULL,
                            TransactionType nvarchar(50) NOT NULL,
                            Points int NOT NULL,
                            Description nvarchar(255),
                            TransactionDate datetime NOT NULL
                        )";

                    using (SqlCommand cmd = new SqlCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Add test points
                    string addPointsQuery = @"
                        INSERT INTO PointsTransactions (UserId, TransactionType, Points, Description, TransactionDate)
                        VALUES (@UserId, @TransactionType, @Points, @Description, @TransactionDate)";

                    using (SqlCommand cmd = new SqlCommand(addPointsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@TransactionType", "Test");
                        cmd.Parameters.AddWithValue("@Points", 10);
                        cmd.Parameters.AddWithValue("@Description", "Manual test points added");
                        cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        lblTestResult.Text = $"<span class='success'>Added 10 test points successfully ({rowsAffected} rows affected)</span>";
                    }
                }

                // Refresh the page data
                LoadDebugInfo();
            }
            catch (Exception ex)
            {
                lblTestResult.Text = $"<span class='error'>Error adding test points: {ex.Message}</span>";
            }
        }
    }
}