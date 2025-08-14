using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.User
{
    public partial class UserAchievements : System.Web.UI.Page
    {
        private string connectionString;

        public UserAchievements()
        {
            var connStr = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"];
            if (connStr != null)
            {
                connectionString = connStr.ConnectionString;
            }
            else
            {
                throw new ConfigurationErrorsException("Connection string 'SpotTheScamConnectionString' not found in web.config");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserCurrentPoints();
                LoadUserAchievements();
            }
        }

        private void LoadUserCurrentPoints()
        {
            try
            {
                // Check if user is logged in
                if (Session["Username"] == null || string.IsNullOrEmpty(Session["Username"].ToString()))
                {
                    Response.Redirect("UserLogin.aspx");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"✅ Achievements page - Loading points for user: {Session["Username"]}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get UserID from Username
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
                        }
                        else
                        {
                            lblCurrentPoints.Text = "0";
                            return;
                        }
                    }

                    // Get current total points from PointsTransactions table
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
                        lblCurrentPoints.Text = currentUserPoints.ToString();

                        System.Diagnostics.Debug.WriteLine($"✅ Achievements page - Current points loaded: {currentUserPoints}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading current points in Achievements page: {ex.Message}");
                lblCurrentPoints.Text = "0";
            }
        }

        private void LoadUserAchievements()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🏆 Loading user achievements from database...");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First, check if UserAchievements table exists and has data
                    string checkQuery = "SELECT COUNT(*) FROM UserAchievements WHERE UserId = @UserId";
                    int userId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@UserId", userId);
                        int achievementCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        System.Diagnostics.Debug.WriteLine($"📊 Found {achievementCount} achievements for user {userId}");

                        if (achievementCount == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("⚠️ No achievements found, this could be normal for new users");
                        }
                    }

                    // Load achievements data (you can expand this to dynamically populate the UI)
                    string query = @"
                        SELECT 
                            ua.UserAchievementId,
                            ua.AchievementId,
                            ua.Progress,
                            ua.IsCompleted,
                            ua.UnlockedDate,
                            a.AchievementName,
                            a.Description,
                            a.RequirementType,
                            a.RequirementValue,
                            a.BonusPoints,
                            a.BadgeColor
                        FROM UserAchievements ua
                        INNER JOIN Achievements a ON ua.AchievementId = a.AchievementId
                        WHERE ua.UserId = @UserId
                        ORDER BY ua.IsCompleted DESC, a.RequirementValue";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"✅ Loaded {dt.Rows.Count} achievements for user");
                                // Store in ViewState for later use
                                ViewState["AchievementsData"] = dt;

                                // You can add code here to dynamically update the UI based on actual user data
                                // For now, we'll keep the static achievements but you could make them dynamic
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("⚠️ No achievements data found for user");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading achievements: {ex.Message}");
                // Continue without achievements data - the static achievements will still show
            }
        }

        // Helper method to get user's current points as string
        protected string GetCurrentPointsText()
        {
            return Session["CurrentPoints"]?.ToString() ?? "0";
        }

        // Method to check if user has completed specific achievements (for future use)
        protected bool HasCompletedAchievement(string achievementName)
        {
            try
            {
                DataTable achievementsData = (DataTable)ViewState["AchievementsData"];
                if (achievementsData != null)
                {
                    foreach (DataRow row in achievementsData.Rows)
                    {
                        if (row["AchievementName"].ToString() == achievementName &&
                            Convert.ToBoolean(row["IsCompleted"]))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error checking achievement: {ex.Message}");
                return false;
            }
        }

        // Method to get achievement progress (for future use)
        protected int GetAchievementProgress(string achievementName)
        {
            try
            {
                DataTable achievementsData = (DataTable)ViewState["AchievementsData"];
                if (achievementsData != null)
                {
                    foreach (DataRow row in achievementsData.Rows)
                    {
                        if (row["AchievementName"].ToString() == achievementName)
                        {
                            return Convert.ToInt32(row["Progress"]);
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting achievement progress: {ex.Message}");
                return 0;
            }
        }
    }
}