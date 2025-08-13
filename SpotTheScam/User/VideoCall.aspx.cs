using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace SpotTheScam.User
{
    public partial class VideoCall : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if customer came with a direct session link
                string sessionIdString = Request.QueryString["sessionId"];
                string userId = Request.QueryString["userId"];

                if (!string.IsNullOrEmpty(sessionIdString) && !string.IsNullOrEmpty(userId))
                {
                    // Direct session access from JoinSession.aspx
                    hdnSessionId.Value = sessionIdString;

                    // Get user's phone number from database
                    string userPhone = GetUserPhoneNumber(Convert.ToInt32(userId));
                    if (!string.IsNullOrEmpty(userPhone))
                    {
                        hdnCustomerPhone.Value = userPhone;

                        // Pre-fill phone input and auto-join
                        string script = $@"
                            window.onload = function() {{
                                document.getElementById('phoneInput').value = '{userPhone}';
                                document.getElementById('phoneInput').style.display = 'none';
                                document.getElementById('joinBtn').innerText = 'Connecting to Your Session...';
                                setTimeout(function() {{ joinSession(); }}, 1000);
                            }};";
                        ClientScript.RegisterStartupScript(this.GetType(), "AutoJoin", script, true);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Customer Video Call page loaded");
            }
        }

        private string GetUserPhoneNumber(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // First try to get phone from VideoCallBookings for this user
                    string phoneQuery = @"
                        SELECT TOP 1 CustomerPhone 
                        FROM VideoCallBookings 
                        WHERE UserId = @UserId 
                        AND BookingStatus IN ('Confirmed', 'Expert Ready')
                        ORDER BY BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(phoneQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        if (result != null && !string.IsNullOrEmpty(result.ToString()))
                        {
                            return result.ToString();
                        }
                    }

                    // If not found in bookings, try Users table
                    string userPhoneQuery = "SELECT PhoneNumber FROM Users WHERE Id = @UserId";
                    using (SqlCommand cmd = new SqlCommand(userPhoneQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        if (result != null && !string.IsNullOrEmpty(result.ToString()))
                        {
                            return result.ToString();
                        }
                    }

                    // If still no phone found, return a default test phone
                    return "12345678";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting user phone: {ex.Message}");
                return "12345678"; // Default fallback
            }
        }

        [WebMethod]
        public static string CheckSession(string phoneNumber)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine($"🔍 Checking session for phone: {phoneNumber}");

                    // Clean the phone number
                    string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

                    // Look for existing booking with this phone number first
                    string findBookingQuery = @"
                        SELECT TOP 1 
                            vcb.SessionId,
                            es.SessionTitle,
                            es.SessionDate,
                            es.StartTime,
                            es.EndTime,
                            es.ExpertName,
                            es.Status,
                            vcb.BookingStatus
                        FROM VideoCallBookings vcb
                        INNER JOIN ExpertSessions es ON vcb.SessionId = es.Id
                        WHERE (vcb.CustomerPhone = @Phone OR vcb.CustomerPhone = @CleanPhone)
                        AND vcb.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected')
                        AND es.Status = 'Available'
                        ORDER BY vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(findBookingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                        cmd.Parameters.AddWithValue("@CleanPhone", cleanPhone);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int sessionId = Convert.ToInt32(reader["SessionId"]);
                                string sessionTitle = reader["SessionTitle"].ToString();
                                string expertName = reader["ExpertName"].ToString();
                                DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                                TimeSpan startTime = (TimeSpan)reader["StartTime"];

                                reader.Close();

                                // Update booking status to indicate customer is trying to connect
                                UpdateCustomerConnectionStatus(conn, sessionId, cleanPhone, "Connected");

                                var result = new
                                {
                                    success = true,
                                    message = "Expert session found! Connecting...",
                                    sessionId = sessionId.ToString(),
                                    staffName = expertName,
                                    sessionDate = sessionDate.ToString("dd/MM/yyyy"),
                                    sessionTime = startTime.ToString(@"hh\:mm"),
                                    duration = "60"
                                };

                                System.Diagnostics.Debug.WriteLine($"✅ Existing booking found! SessionId: {sessionId}");

                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                return serializer.Serialize(result);
                            }
                        }
                    }

                    // If no existing booking, look for any available session and create booking
                    string findSessionQuery = @"
                        SELECT TOP 1 
                            es.Id as SessionId,
                            es.SessionTitle,
                            es.SessionDate,
                            es.StartTime,
                            es.EndTime,
                            es.ExpertName,
                            es.Status
                        FROM ExpertSessions es
                        WHERE es.Status = 'Available'
                        AND es.SessionDate >= CAST(GETDATE() AS DATE)
                        ORDER BY es.SessionDate ASC, es.StartTime ASC";

                    using (SqlCommand cmd = new SqlCommand(findSessionQuery, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int sessionId = Convert.ToInt32(reader["SessionId"]);
                                string sessionTitle = reader["SessionTitle"].ToString();
                                string expertName = reader["ExpertName"].ToString();
                                DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                                TimeSpan startTime = (TimeSpan)reader["StartTime"];

                                reader.Close();

                                // Create new booking for this phone number
                                CreateOrUpdateBooking(conn, sessionId, cleanPhone);

                                var result = new
                                {
                                    success = true,
                                    message = "Expert session found! Connecting...",
                                    sessionId = sessionId.ToString(),
                                    staffName = expertName,
                                    sessionDate = sessionDate.ToString("dd/MM/yyyy"),
                                    sessionTime = startTime.ToString(@"hh\:mm"),
                                    duration = "60"
                                };

                                System.Diagnostics.Debug.WriteLine($"✅ Session found and booking created! SessionId: {sessionId}");

                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                return serializer.Serialize(result);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("❌ No available sessions found");

                                var result = new
                                {
                                    success = false,
                                    message = "No expert sessions are currently available. Please check back later or contact support.",
                                    sessionId = "",
                                    staffName = "",
                                    sessionDate = "",
                                    sessionTime = "",
                                    duration = ""
                                };

                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                return serializer.Serialize(result);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error checking session: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");

                var result = new
                {
                    success = false,
                    message = "Error connecting to expert session. Please try again or contact support.",
                    sessionId = "",
                    staffName = "",
                    sessionDate = "",
                    sessionTime = "",
                    duration = ""
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // Create or update booking for the participant
        private static void CreateOrUpdateBooking(SqlConnection conn, int sessionId, string phoneNumber)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔄 Creating/updating booking for session {sessionId}, phone {phoneNumber}");

                // Check if booking already exists
                string checkBookingQuery = @"
                    SELECT COUNT(*) FROM VideoCallBookings 
                    WHERE SessionId = @SessionId 
                    AND CustomerPhone = @Phone";

                using (SqlCommand checkCmd = new SqlCommand(checkBookingQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@SessionId", sessionId);
                    checkCmd.Parameters.AddWithValue("@Phone", phoneNumber);

                    int existingBookings = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existingBookings == 0)
                    {
                        // Create new booking
                        string insertBookingQuery = @"
                            INSERT INTO VideoCallBookings (
                                SessionId, UserId, CustomerName, CustomerPhone, CustomerEmail,
                                BookingDate, BookingStatus, ScamConcerns, PointsUsed
                            ) VALUES (
                                @SessionId, 1, @CustomerName, @CustomerPhone, @CustomerEmail,
                                @BookingDate, 'Connected', 'General consultation', 0
                            )";

                        using (SqlCommand insertCmd = new SqlCommand(insertBookingQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@SessionId", sessionId);
                            insertCmd.Parameters.AddWithValue("@CustomerName", $"Participant {phoneNumber}");
                            insertCmd.Parameters.AddWithValue("@CustomerPhone", phoneNumber);
                            insertCmd.Parameters.AddWithValue("@CustomerEmail", $"participant{phoneNumber}@example.com");
                            insertCmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);

                            int inserted = insertCmd.ExecuteNonQuery();
                            if (inserted > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"✅ Created new booking for session {sessionId}");
                            }
                        }
                    }
                    else
                    {
                        // Update existing booking status
                        UpdateCustomerConnectionStatus(conn, sessionId, phoneNumber, "Connected");
                        System.Diagnostics.Debug.WriteLine($"✅ Updated existing booking for session {sessionId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error creating/updating booking: {ex.Message}");
            }
        }

        // Helper method to update customer connection status
        private static void UpdateCustomerConnectionStatus(SqlConnection conn, int sessionId, string phoneNumber, string status)
        {
            try
            {
                string updateBookingQuery = @"
                    UPDATE VideoCallBookings 
                    SET BookingStatus = @Status, BookingDate = @BookingDate
                    WHERE SessionId = @SessionId 
                    AND CustomerPhone = @Phone";

                using (SqlCommand updateCmd = new SqlCommand(updateBookingQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@SessionId", sessionId);
                    updateCmd.Parameters.AddWithValue("@Status", status);
                    updateCmd.Parameters.AddWithValue("@Phone", phoneNumber);
                    updateCmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);

                    updateCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error updating customer connection status: {ex.Message}");
            }
        }

        // CRITICAL FIX: Enhanced method to get participant real name
        [WebMethod]
        public static string GetParticipantName(int sessionId, string phoneNumber)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

                    // Enhanced query to get the most accurate participant name
                    string nameQuery = @"
                        SELECT TOP 1
                            CASE 
                                WHEN vcb.CustomerName IS NOT NULL AND LEN(TRIM(vcb.CustomerName)) > 0 
                                     AND vcb.CustomerName NOT LIKE 'Participant %'
                                THEN vcb.CustomerName
                                WHEN vcb.FirstName IS NOT NULL AND vcb.LastName IS NOT NULL 
                                THEN vcb.FirstName + ' ' + vcb.LastName
                                WHEN u.Username IS NOT NULL AND u.Username NOT LIKE 'user%'
                                THEN u.Username
                                WHEN u.Name IS NOT NULL AND LEN(TRIM(u.Name)) > 0
                                THEN u.Name
                                ELSE NULL
                            END as ParticipantName,
                            vcb.CustomerEmail,
                            u.Email
                        FROM VideoCallBookings vcb
                        LEFT JOIN Users u ON vcb.UserId = u.Id
                        WHERE vcb.SessionId = @SessionId 
                        AND (vcb.CustomerPhone = @Phone OR vcb.CustomerPhone = @CleanPhone)
                        ORDER BY vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(nameQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                        cmd.Parameters.AddWithValue("@CleanPhone", cleanPhone);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string participantName = reader["ParticipantName"]?.ToString();
                                string customerEmail = reader["CustomerEmail"]?.ToString();
                                string userEmail = reader["Email"]?.ToString();

                                // If we have a good name, use it
                                if (!string.IsNullOrEmpty(participantName))
                                {
                                    var result = new
                                    {
                                        success = true,
                                        name = participantName
                                    };

                                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                                    return serializer.Serialize(result);
                                }

                                // Try to extract name from email if no direct name available
                                string emailToUse = !string.IsNullOrEmpty(customerEmail) ? customerEmail : userEmail;
                                if (!string.IsNullOrEmpty(emailToUse) && emailToUse.Contains("@") && !emailToUse.StartsWith("participant"))
                                {
                                    string nameFromEmail = emailToUse.Split('@')[0];
                                    // Capitalize first letter and handle common email patterns
                                    if (nameFromEmail.Length > 1 && !nameFromEmail.All(char.IsDigit))
                                    {
                                        nameFromEmail = char.ToUpper(nameFromEmail[0]) + nameFromEmail.Substring(1).ToLower();
                                        if (nameFromEmail.Contains("."))
                                        {
                                            string[] parts = nameFromEmail.Split('.');
                                            nameFromEmail = string.Join(" ", parts.Select(p => char.ToUpper(p[0]) + p.Substring(1)));
                                        }

                                        var result = new
                                        {
                                            success = true,
                                            name = nameFromEmail
                                        };

                                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                                        return serializer.Serialize(result);
                                    }
                                }
                            }
                        }
                    }
                }

                // Fallback to formatted phone number
                string cleanPhoneFallback = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");
                string fallbackName = $"Participant ({cleanPhoneFallback.Substring(0, Math.Min(4, cleanPhoneFallback.Length))}...{cleanPhoneFallback.Substring(Math.Max(0, cleanPhoneFallback.Length - 4))})";

                var fallbackResult = new
                {
                    success = true,
                    name = fallbackName
                };

                JavaScriptSerializer fallbackSerializer = new JavaScriptSerializer();
                return fallbackSerializer.Serialize(fallbackResult);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting participant name: {ex.Message}");

                var result = new
                {
                    success = false,
                    name = "Participant"
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // CRITICAL FIX: Fixed participant discovery with correct column names
        [WebMethod]
        public static string GetSessionParticipants(int sessionId)
        {
            try
            {
                var participants = new List<object>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // FIXED: Query with correct column names based on your database schema
                    string query = @"
                        SELECT 
                            vcb.SessionId,
                            vcb.BookingId,
                            vcb.UserId,
                            vcb.CustomerPhone as Phone,
                            
                            CASE 
                                WHEN vcb.CustomerName IS NOT NULL AND LEN(TRIM(vcb.CustomerName)) > 0 
                                     AND vcb.CustomerName NOT LIKE 'Participant %'
                                THEN vcb.CustomerName
                                WHEN vcb.FirstName IS NOT NULL AND vcb.LastName IS NOT NULL 
                                THEN vcb.FirstName + ' ' + vcb.LastName
                                WHEN u.Username IS NOT NULL AND u.Username NOT LIKE 'user%'
                                THEN u.Username
                                ELSE 'Participant ' + RIGHT('000' + CAST(ISNULL(vcb.UserId, 1) as VARCHAR), 3)
                            END as ParticipantName,
                            
                            vcb.BookingStatus as Status,
                            vcb.BookingDate as LastActivity
                        FROM VideoCallBookings vcb
                        LEFT JOIN Users u ON vcb.UserId = u.Id
                        WHERE vcb.SessionId = @SessionId
                        AND vcb.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected', 'In Call')
                        AND vcb.CustomerPhone IS NOT NULL
                        AND LEN(TRIM(vcb.CustomerPhone)) > 0
                        ORDER BY vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);

                        System.Diagnostics.Debug.WriteLine($"🔍 Executing participant query for session {sessionId}");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string phone = reader["Phone"]?.ToString();
                                if (string.IsNullOrEmpty(phone)) continue;

                                string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

                                // Skip empty phones or obviously invalid ones
                                if (string.IsNullOrEmpty(cleanPhone) || cleanPhone.Length < 4)
                                {
                                    System.Diagnostics.Debug.WriteLine($"⏭️ Skipping invalid phone: {cleanPhone}");
                                    continue;
                                }

                                string participantName = reader["ParticipantName"]?.ToString() ?? "Unknown Participant";
                                string status = reader["Status"]?.ToString() ?? "Connected";
                                int userId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : 0;
                                int bookingId = reader["BookingId"] != DBNull.Value ? Convert.ToInt32(reader["BookingId"]) : 0;

                                participants.Add(new
                                {
                                    sessionId = sessionId,
                                    bookingId = bookingId,
                                    userId = userId,
                                    phone = cleanPhone,
                                    name = participantName,
                                    status = status,
                                    peerId = $"customer_{cleanPhone}",
                                    lastActive = reader["LastActivity"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["LastActivity"]).ToString("HH:mm")
                                        : "Unknown"
                                });

                                System.Diagnostics.Debug.WriteLine($"✅ Found participant: {participantName} ({cleanPhone}) - Status: {status}");
                            }
                        }
                    }

                    // SIMPLIFIED: Remove the complex second query that was causing issues
                    // The main query above should catch all participants
                }

                System.Diagnostics.Debug.WriteLine($"🔍 GetSessionParticipants - Found {participants.Count} participants for session {sessionId}");

                // Log each participant for debugging
                foreach (var participant in participants)
                {
                    var phoneValue = participant.GetType().GetProperty("phone").GetValue(participant);
                    var nameValue = participant.GetType().GetProperty("name").GetValue(participant);
                    var statusValue = participant.GetType().GetProperty("status").GetValue(participant);
                    System.Diagnostics.Debug.WriteLine($"  - Participant: {nameValue} ({phoneValue}) - Status: {statusValue}");
                }

                var result = new
                {
                    success = true,
                    participants = participants,
                    totalCount = participants.Count,
                    sessionId = sessionId,
                    timestamp = DateTime.Now.ToString("HH:mm:ss"),
                    debugInfo = $"Query executed at {DateTime.Now:HH:mm:ss}, found {participants.Count} participants"
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error getting session participants: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");

                var result = new
                {
                    success = false,
                    message = ex.Message,
                    participants = new List<object>(),
                    totalCount = 0,
                    sessionId = sessionId,
                    timestamp = DateTime.Now.ToString("HH:mm:ss"),
                    debugInfo = $"Error occurred at {DateTime.Now:HH:mm:ss}: {ex.Message}"
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // ADDITIONAL FIX: Method to manually add a participant for testing
        [WebMethod]
        public static string AddTestParticipant(int sessionId, string phoneNumber, string participantName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

                    // Check if participant already exists
                    string checkQuery = "SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = @SessionId AND CustomerPhone = @Phone";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@SessionId", sessionId);
                        checkCmd.Parameters.AddWithValue("@Phone", cleanPhone);

                        int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (existingCount == 0)
                        {
                            // Add new test participant
                            CreateOrUpdateBooking(conn, sessionId, cleanPhone);

                            // Update with custom name if provided
                            if (!string.IsNullOrEmpty(participantName))
                            {
                                string updateNameQuery = @"
                                    UPDATE VideoCallBookings 
                                    SET CustomerName = @Name 
                                    WHERE SessionId = @SessionId AND CustomerPhone = @Phone";

                                using (SqlCommand updateCmd = new SqlCommand(updateNameQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@Name", participantName);
                                    updateCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                    updateCmd.Parameters.AddWithValue("@Phone", cleanPhone);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }

                            var result = new { success = true, message = $"Test participant {participantName} added successfully" };
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            return serializer.Serialize(result);
                        }
                        else
                        {
                            var result = new { success = false, message = "Participant already exists" };
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            return serializer.Serialize(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error adding test participant: {ex.Message}");

                var result = new { success = false, message = ex.Message };
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnSessionId.Value) && !string.IsNullOrEmpty(hdnCustomerPhone.Value))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                    {
                        conn.Open();
                        string cleanPhone = System.Text.RegularExpressions.Regex.Replace(hdnCustomerPhone.Value, @"[^\d]", "");
                        UpdateCustomerConnectionStatus(conn, Convert.ToInt32(hdnSessionId.Value), cleanPhone, "Customer Disconnected");
                        System.Diagnostics.Debug.WriteLine($"Customer session ended for phone: {hdnCustomerPhone.Value}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating session on unload: {ex.Message}");
                }
            }
            base.OnUnload(e);
        }
    }
}