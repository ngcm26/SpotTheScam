using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace SpotTheScam.User
{
    public partial class UserPointsStore : System.Web.UI.Page
    {
        private string connectionString;

        public UserPointsStore()
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
                LoadStoreItems();
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

                System.Diagnostics.Debug.WriteLine($"✅ Points Store page - Loading points for user: {Session["Username"]}");

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

                        System.Diagnostics.Debug.WriteLine($"✅ Points Store page - Current points loaded: {currentUserPoints}");

                        // Update store item availability based on current points
                        UpdateStoreItemAvailability(currentUserPoints);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading current points in Points Store page: {ex.Message}");
                lblCurrentPoints.Text = "0";
                UpdateStoreItemAvailability(0);
            }
        }

        private void LoadStoreItems()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("📋 Loading store items from database...");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First, check if PointsStoreItems table exists and has data
                    string checkQuery = "SELECT COUNT(*) FROM PointsStoreItems";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        int itemCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                        System.Diagnostics.Debug.WriteLine($"📊 Found {itemCount} items in PointsStoreItems table");

                        if (itemCount == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("⚠️ No items in database, creating sample store items...");
                            CreateSampleStoreItems(conn);
                        }
                    }

                    // Now load the store items
                    string query = @"
                        SELECT 
                            ItemId,
                            ItemName,
                            Description,
                            PointsCost,
                            ItemType,
                            CreatedDate
                        FROM PointsStoreItems
                        ORDER BY PointsCost, ItemName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"✅ Loaded {dt.Rows.Count} store items from PointsStoreItems table");
                                ViewState["StoreItemsData"] = dt;
                                System.Diagnostics.Debug.WriteLine("🔄 Store items loaded successfully from database");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("⚠️ No store items found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading store items: {ex.Message}");
            }
        }

        private void CreateSampleStoreItems(SqlConnection conn)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🏪 Creating sample store items...");

                string insertQuery = @"
                    INSERT INTO PointsStoreItems (ItemName, Description, PointsCost, ItemType, CreatedDate)
                    VALUES (@ItemName, @Description, @PointsCost, @ItemType, @CreatedDate)";

                // Store Item 1 - Safety Guide PDF
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", "Safety Guide PDF");
                    cmd.Parameters.AddWithValue("@Description", "Download a comprehensive 20-page safety guide you can share with family");
                    cmd.Parameters.AddWithValue("@PointsCost", 40);
                    cmd.Parameters.AddWithValue("@ItemType", "PDF");
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                // Store Item 2 - Advanced Quiz
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", "Advanced Quiz");
                    cmd.Parameters.AddWithValue("@Description", "Unlock expert-level quizzes with real-world scam scenarios and case-studies");
                    cmd.Parameters.AddWithValue("@PointsCost", 50);
                    cmd.Parameters.AddWithValue("@ItemType", "Quiz");
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                // Store Item 3 - Detailed Scam Report
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", "Detailed Scam Report");
                    cmd.Parameters.AddWithValue("@Description", "Get a personalized report on the latest scam trends in your area with prevention tips");
                    cmd.Parameters.AddWithValue("@PointsCost", 50);
                    cmd.Parameters.AddWithValue("@ItemType", "Report");
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                // Store Item 4 - Expert Webinar Session
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", "Reserve Live Expert Webinar");
                    cmd.Parameters.AddWithValue("@Description", "Book your spot in upcoming live expert sessions on scam prevention and digital safety");
                    cmd.Parameters.AddWithValue("@PointsCost", 150);
                    cmd.Parameters.AddWithValue("@ItemType", "Webinar");
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                System.Diagnostics.Debug.WriteLine("✅ Successfully created 4 sample store items");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error creating sample store items: {ex.Message}");
            }
        }

        private void UpdateStoreItemAvailability(int currentPoints)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔄 Updating store item availability for {currentPoints} points");

                // Define item costs
                int reportCost = 50;
                int guideCost = 40;
                int quizCost = 50;
                int webinarCost = 150;

                // Update buttons based on points
                btnPurchaseReport.Enabled = currentPoints >= reportCost;
                btnPurchaseReport.Text = currentPoints >= reportCost ? "Purchase Now" : "Insufficient Points";

                btnPurchaseGuide.Enabled = currentPoints >= guideCost;
                btnPurchaseGuide.Text = currentPoints >= guideCost ? "Purchase Now" : "Insufficient Points";

                btnPurchaseQuiz.Enabled = currentPoints >= quizCost;
                btnPurchaseQuiz.Text = currentPoints >= quizCost ? "Purchase Now" : "Insufficient Points";

                btnPurchaseWebinar.Enabled = false; // Always disabled
                btnPurchaseWebinar.Text = currentPoints >= webinarCost ? "Coming Soon" : "Insufficient Points";

                System.Diagnostics.Debug.WriteLine("✅ Store item availability updated successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error updating store item availability: {ex.Message}");
            }
        }

        protected void PurchaseItem_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                string itemInfo = btn.CommandArgument;

                string[] parts = itemInfo.Split('|');
                if (parts.Length == 2)
                {
                    string itemName = parts[0];
                    int itemCost = Convert.ToInt32(parts[1]);
                    int currentPoints = Session["CurrentPoints"] != null ? Convert.ToInt32(Session["CurrentPoints"]) : 0;

                    if (currentPoints >= itemCost)
                    {
                        ProcessPurchase(itemName, itemCost);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Insufficient points for {itemName}. Required: {itemCost}, Available: {currentPoints}");
                        ShowPurchaseErrorMessage("You don't have enough points for this item.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error processing purchase: {ex.Message}");
                ShowPurchaseErrorMessage("There was an error processing your purchase.");
            }
        }

        private void ProcessPurchase(string itemName, int itemCost)
        {
            try
            {
                int userId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;
                if (userId == 0) return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Deduct points
                            string deductPointsQuery = @"
                                INSERT INTO PointsTransactions (UserId, Points, TransactionType, Description, TransactionDate)
                                VALUES (@UserId, @Points, @TransactionType, @Description, @TransactionDate)";

                            using (SqlCommand cmd = new SqlCommand(deductPointsQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                cmd.Parameters.AddWithValue("@Points", -itemCost);
                                cmd.Parameters.AddWithValue("@TransactionType", "Store Purchase");
                                cmd.Parameters.AddWithValue("@Description", $"Purchased: {GetFriendlyItemName(itemName)}");
                                cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }

                            // Record purchase
                            string recordPurchaseQuery = @"
                                INSERT INTO UserStorePurchases (UserId, ItemId, PointsSpent, PurchaseDate, DeliveryStatus, DownloadLink)
                                VALUES (@UserId, @ItemId, @PointsSpent, @PurchaseDate, @DeliveryStatus, @DownloadLink)";

                            using (SqlCommand cmd = new SqlCommand(recordPurchaseQuery, conn, transaction))
                            {
                                string friendlyName = GetFriendlyItemName(itemName);
                                string getItemIdQuery = "SELECT ItemId FROM PointsStoreItems WHERE ItemName = @ItemName";

                                using (SqlCommand getItemCmd = new SqlCommand(getItemIdQuery, conn, transaction))
                                {
                                    getItemCmd.Parameters.AddWithValue("@ItemName", friendlyName);
                                    object itemIdResult = getItemCmd.ExecuteScalar();
                                    int itemId = itemIdResult != null ? Convert.ToInt32(itemIdResult) : 1;

                                    string downloadLink = itemName == "SafetyGuidePDF" ? "/Uploads/pdf_scamguide/ScamSafetyGuide.pdf" : "";

                                    cmd.Parameters.AddWithValue("@UserId", userId);
                                    cmd.Parameters.AddWithValue("@ItemId", itemId);
                                    cmd.Parameters.AddWithValue("@PointsSpent", itemCost);
                                    cmd.Parameters.AddWithValue("@PurchaseDate", DateTime.Now);
                                    cmd.Parameters.AddWithValue("@DeliveryStatus", "Completed");
                                    cmd.Parameters.AddWithValue("@DownloadLink", downloadLink);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine($"✅ Purchase completed: {GetFriendlyItemName(itemName)} for {itemCost} points");
                            HandleItemDelivery(itemName);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                LoadUserCurrentPoints();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error processing purchase for {itemName}: {ex.Message}");
                ShowPurchaseErrorMessage("There was an error processing your purchase. Please try again.");
            }
        }

        private string GetFriendlyItemName(string itemName)
        {
            switch (itemName)
            {
                case "DetailedScamReport": return "Detailed Scam Report";
                case "SafetyGuidePDF": return "Safety Guide PDF";
                case "AdvancedQuiz": return "Advanced Quiz";
                case "ExpertWebinar": return "Reserve Live Expert Webinar";
                default: return itemName;
            }
        }

        private void HandleItemDelivery(string itemName)
        {
            try
            {
                switch (itemName)
                {
                    case "SafetyGuidePDF":
                        TriggerPDFDownload();
                        ShowPurchaseSuccessMessage("Safety Guide PDF purchased successfully! Your download is starting now.");
                        break;

                    case "DetailedScamReport":
                        ShowPurchaseSuccessMessage("Detailed Scam Report purchased successfully! Your report will be emailed within 24 hours.");
                        break;

                    case "AdvancedQuiz":
                        ShowPurchaseSuccessMessage("Advanced Quiz unlocked successfully! Access it in the Quiz section.");
                        break;

                    case "ExpertWebinar":
                        ShowPurchaseSuccessMessage("Expert Webinar reserved! You'll receive booking details via email.");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error in item delivery: {ex.Message}");
                ShowPurchaseErrorMessage("Purchase completed but there was an issue with delivery. Please contact support.");
            }
        }

        private void TriggerPDFDownload()
        {
            try
            {
                string fileName = "ScamSafetyGuide.pdf";
                string filePath = Server.MapPath($"~/Uploads/pdf_scamguide/{fileName}");

                // Check if the PDF file exists
                if (File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"✅ PDF found at: {filePath}");

                    // Trigger download via JavaScript
                    string downloadScript = $@"
                        <script type='text/javascript'>
                            setTimeout(function() {{
                                var link = document.createElement('a');
                                link.href = '/Uploads/pdf_scamguide/{fileName}';
                                link.download = '{fileName}';
                                link.style.display = 'none';
                                document.body.appendChild(link);
                                link.click();
                                document.body.removeChild(link);
                                console.log('PDF download triggered successfully');
                            }}, 1000);
                        </script>";

                    ClientScript.RegisterStartupScript(this.GetType(), "DownloadPDF", downloadScript, false);
                    System.Diagnostics.Debug.WriteLine("✅ PDF download script registered");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ PDF file not found at: {filePath}");
                    ShowPurchaseErrorMessage("PDF file not found. Please contact support.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error triggering download: {ex.Message}");
                ShowPurchaseErrorMessage("There was an issue starting the download. Please contact support.");
            }
        }

        private void ShowPurchaseSuccessMessage(string message)
        {
            string script = $@"<script type='text/javascript'>alert('{message}');</script>";
            ClientScript.RegisterStartupScript(this.GetType(), "PurchaseSuccess", script, false);
        }

        private void ShowPurchaseErrorMessage(string message)
        {
            string script = $@"<script type='text/javascript'>alert('{message}');</script>";
            ClientScript.RegisterStartupScript(this.GetType(), "PurchaseError", script, false);
        }

        protected bool CanAffordItem(int itemCost)
        {
            int currentPoints = Session["CurrentPoints"] != null ? Convert.ToInt32(Session["CurrentPoints"]) : 0;
            return currentPoints >= itemCost;
        }

        protected string GetCurrentPointsText()
        {
            return Session["CurrentPoints"]?.ToString() ?? "0";
        }
    }
}