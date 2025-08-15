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
                        string script = string.Format(@"
                            window.onload = function() {{
                                document.getElementById('phoneInput').value = '{0}';
                                document.getElementById('phoneInput').style.display = 'none';
                                document.getElementById('joinBtn').innerText = 'Connecting to Your Session...';
                                setTimeout(function() {{ joinSession(); }}, 1000);
                            }};", userPhone);
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
                System.Diagnostics.Debug.WriteLine(string.Format("Error getting user phone: {0}", ex.Message));
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
                    System.Diagnostics.Debug.WriteLine(string.Format("🔍 Checking session for phone: {0}", phoneNumber));

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

                                System.Diagnostics.Debug.WriteLine(string.Format("✅ Existing booking found! SessionId: {0}", sessionId));

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

                                System.Diagnostics.Debug.WriteLine(string.Format("✅ Session found and booking created! SessionId: {0}", sessionId));

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
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Error checking session: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Stack trace: {0}", ex.StackTrace));

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
                System.Diagnostics.Debug.WriteLine(string.Format("🔄 Creating/updating booking for session {0}, phone {1}", sessionId, phoneNumber));

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
                        // CRITICAL: Get the actual user's name from Users table FIRST
                        string actualName = GetActualUserName(conn, phoneNumber);

                        // Create new booking with REAL NAME
                        string insertBookingQuery = @"
                            INSERT INTO VideoCallBookings (
                                SessionId, UserId, CustomerName, CustomerPhone, CustomerEmail,
                                BookingDate, BookingStatus, ScamConcerns, PointsUsed
                            ) VALUES (
                                @SessionId, @UserId, @CustomerName, @CustomerPhone, @CustomerEmail,
                                @BookingDate, 'Connected', 'General consultation', 0
                            )";

                        using (SqlCommand insertCmd = new SqlCommand(insertBookingQuery, conn))
                        {
                            // Get actual UserId
                            int actualUserId = GetUserIdByPhone(conn, phoneNumber);

                            insertCmd.Parameters.AddWithValue("@SessionId", sessionId);
                            insertCmd.Parameters.AddWithValue("@UserId", actualUserId > 0 ? actualUserId : 1);
                            insertCmd.Parameters.AddWithValue("@CustomerName", actualName); // USE REAL NAME
                            insertCmd.Parameters.AddWithValue("@CustomerPhone", phoneNumber);
                            insertCmd.Parameters.AddWithValue("@CustomerEmail", GetUserEmail(conn, phoneNumber));
                            insertCmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);

                            int inserted = insertCmd.ExecuteNonQuery();
                            if (inserted > 0)
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("✅ Created new booking for session {0} with name: {1}", sessionId, actualName));
                            }
                        }
                    }
                    else
                    {
                        // Update existing booking status
                        UpdateCustomerConnectionStatus(conn, sessionId, phoneNumber, "Connected");
                        System.Diagnostics.Debug.WriteLine(string.Format("✅ Updated existing booking for session {0}", sessionId));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Error creating/updating booking: {0}", ex.Message));
            }
        }

        // ENHANCED: Get actual user name from Users table with better name resolution
        private static string GetActualUserName(SqlConnection conn, string phoneNumber)
        {
            try
            {
                string getUserNameQuery = @"
                    SELECT TOP 1 
                        CASE 
                            WHEN LEN(TRIM(ISNULL(FirstName, '') + ' ' + ISNULL(LastName, ''))) > 1
                            THEN TRIM(ISNULL(FirstName, '') + ' ' + ISNULL(LastName, ''))
                            WHEN Username IS NOT NULL AND Username NOT LIKE 'user%' AND Username NOT LIKE 'test%'
                                 AND LEN(TRIM(Username)) > 2
                            THEN Username
                            WHEN Email IS NOT NULL AND Email LIKE '%@%.%' AND Email NOT LIKE 'test%'
                                 AND Email NOT LIKE 'participant%@%'
                            THEN CASE 
                                WHEN CHARINDEX('.', LEFT(Email, CHARINDEX('@', Email) - 1)) > 0
                                THEN UPPER(LEFT(LEFT(Email, CHARINDEX('@', Email) - 1), 1)) + 
                                     LOWER(SUBSTRING(LEFT(Email, CHARINDEX('@', Email) - 1), 2, 
                                     CHARINDEX('.', LEFT(Email, CHARINDEX('@', Email) - 1)) - 2)) + ' ' +
                                     UPPER(SUBSTRING(LEFT(Email, CHARINDEX('@', Email) - 1), 
                                     CHARINDEX('.', LEFT(Email, CHARINDEX('@', Email) - 1)) + 1, 1)) +
                                     LOWER(SUBSTRING(LEFT(Email, CHARINDEX('@', Email) - 1), 
                                     CHARINDEX('.', LEFT(Email, CHARINDEX('@', Email) - 1)) + 2, 50))
                                ELSE UPPER(LEFT(LEFT(Email, CHARINDEX('@', Email) - 1), 1)) + 
                                     LOWER(SUBSTRING(LEFT(Email, CHARINDEX('@', Email) - 1), 2, 50))
                            END
                            ELSE 'User'
                        END as RealName
                    FROM Users 
                    WHERE PhoneNumber = @Phone OR PhoneNumber LIKE '%' + @CleanPhone + '%'
                    ORDER BY Id DESC";

                using (SqlCommand cmd = new SqlCommand(getUserNameQuery, conn))
                {
                    string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");
                    cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                    cmd.Parameters.AddWithValue("@CleanPhone", cleanPhone);

                    object result = cmd.ExecuteScalar();
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        string name = result.ToString().Trim();
                        if (name.Length > 1 && name != "User")
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("✅ Found real name: {0} for phone: {1}", name, phoneNumber));
                            return name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Error getting actual user name: {0}", ex.Message));
            }

            return "Participant " + phoneNumber; // Fallback
        }

        // Get actual user ID
        private static int GetUserIdByPhone(SqlConnection conn, string phoneNumber)
        {
            try
            {
                string getUserIdQuery = "SELECT TOP 1 Id FROM Users WHERE PhoneNumber = @Phone OR PhoneNumber LIKE '%' + @CleanPhone + '%'";
                using (SqlCommand cmd = new SqlCommand(getUserIdQuery, conn))
                {
                    string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");
                    cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                    cmd.Parameters.AddWithValue("@CleanPhone", cleanPhone);

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        // Get user email
        private static string GetUserEmail(SqlConnection conn, string phoneNumber)
        {
            try
            {
                string getUserEmailQuery = "SELECT TOP 1 Email FROM Users WHERE PhoneNumber = @Phone OR PhoneNumber LIKE '%' + @CleanPhone + '%'";
                using (SqlCommand cmd = new SqlCommand(getUserEmailQuery, conn))
                {
                    string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");
                    cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                    cmd.Parameters.AddWithValue("@CleanPhone", cleanPhone);

                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : ("participant" + phoneNumber + "@example.com");
                }
            }
            catch
            {
                return "participant" + phoneNumber + "@example.com";
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
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Error updating customer connection status: {0}", ex.Message));
            }
        }

        // ENHANCED: Method to get participant real name with better error handling and aggressive retry
        [WebMethod]
        public static string GetParticipantName(int sessionId, string phoneNumber)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

                    // ENHANCED: Query to get the most accurate participant name with better name resolution
                    string nameQuery = @"
                        SELECT TOP 1
                            CASE 
                                WHEN vcb.CustomerName IS NOT NULL AND LEN(TRIM(vcb.CustomerName)) > 0 
                                     AND vcb.CustomerName NOT LIKE 'Participant %'
                                THEN vcb.CustomerName
                                WHEN vcb.FirstName IS NOT NULL AND vcb.LastName IS NOT NULL 
                                THEN vcb.FirstName + ' ' + vcb.LastName
                                WHEN u.Username IS NOT NULL AND u.Username NOT LIKE 'user%'
                                     AND u.Username NOT LIKE 'test%' AND LEN(TRIM(u.Username)) > 2
                                THEN u.Username
                                WHEN u.Email IS NOT NULL AND u.Email NOT LIKE 'participant%@%'
                                     AND u.Email NOT LIKE 'test%@%' AND u.Email LIKE '%@%.%'
                                THEN CASE 
                                    WHEN CHARINDEX('.', LEFT(u.Email, CHARINDEX('@', u.Email) - 1)) > 0
                                    THEN UPPER(LEFT(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 1)) + 
                                         LOWER(SUBSTRING(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 2, 
                                         CHARINDEX('.', LEFT(u.Email, CHARINDEX('@', u.Email) - 1)) - 2)) + ' ' +
                                         UPPER(SUBSTRING(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 
                                         CHARINDEX('.', LEFT(u.Email, CHARINDEX('@', u.Email) - 1)) + 1, 1)) +
                                         LOWER(SUBSTRING(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 
                                         CHARINDEX('.', LEFT(u.Email, CHARINDEX('@', u.Email) - 1)) + 2, 50))
                                    ELSE UPPER(LEFT(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 1)) + 
                                         LOWER(SUBSTRING(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 2, 50))
                                END
                                ELSE NULL
                            END as ParticipantName,
                            vcb.CustomerEmail,
                            u.Email,
                            u.Username,
                            u.FirstName,
                            u.LastName
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
                                string username = reader["Username"]?.ToString();
                                string firstName = reader["FirstName"]?.ToString();
                                string lastName = reader["LastName"]?.ToString();

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

                                // Try FirstName + LastName if available
                                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                                {
                                    var result = new
                                    {
                                        success = true,
                                        name = firstName + " " + lastName
                                    };

                                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                                    return serializer.Serialize(result);
                                }

                                // Enhanced email name extraction
                                string emailToUse = !string.IsNullOrEmpty(customerEmail) ? customerEmail : userEmail;
                                if (!string.IsNullOrEmpty(emailToUse) && emailToUse.Contains("@") &&
                                    !emailToUse.StartsWith("participant") && !emailToUse.StartsWith("test"))
                                {
                                    string nameFromEmail = emailToUse.Split('@')[0];

                                    // Enhanced name formatting from email
                                    if (nameFromEmail.Length > 1 && !nameFromEmail.All(char.IsDigit))
                                    {
                                        // Handle dots and underscores in email
                                        nameFromEmail = nameFromEmail.Replace(".", " ").Replace("_", " ");

                                        // Capitalize each word
                                        string[] words = nameFromEmail.Split(' ');
                                        for (int i = 0; i < words.Length; i++)
                                        {
                                            if (words[i].Length > 0)
                                            {
                                                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                                            }
                                        }
                                        nameFromEmail = string.Join(" ", words).Trim();

                                        var result = new
                                        {
                                            success = true,
                                            name = nameFromEmail
                                        };

                                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                                        return serializer.Serialize(result);
                                    }
                                }

                                // Try username as last resort before phone fallback
                                if (!string.IsNullOrEmpty(username) && !username.StartsWith("user") &&
                                    !username.StartsWith("test") && username.Length > 2)
                                {
                                    var result = new
                                    {
                                        success = true,
                                        name = username
                                    };

                                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                                    return serializer.Serialize(result);
                                }
                            }
                        }
                    }
                }

                // Fallback to formatted phone number
                string cleanPhoneFallback = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");
                string fallbackName = string.Format("Participant ({0}...{1})",
                    cleanPhoneFallback.Substring(0, Math.Min(4, cleanPhoneFallback.Length)),
                    cleanPhoneFallback.Substring(Math.Max(0, cleanPhoneFallback.Length - 4)));

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
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Error getting participant name: {0}", ex.Message));

                var result = new
                {
                    success = false,
                    name = "Participant"
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // ENHANCED: Optimized participant discovery with better performance and real-time updates
        [WebMethod]
        public static string GetSessionParticipants(int sessionId)
        {
            try
            {
                var participants = new List<object>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // ENHANCED: Query with better name resolution for participants
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
                                     AND u.Username NOT LIKE 'test%' AND LEN(TRIM(u.Username)) > 2
                                THEN u.Username
                                WHEN u.Email IS NOT NULL AND u.Email NOT LIKE 'participant%@%'
                                     AND u.Email NOT LIKE 'test%@%' AND u.Email LIKE '%@%.%'
                                THEN CASE 
                                    WHEN CHARINDEX('.', LEFT(u.Email, CHARINDEX('@', u.Email) - 1)) > 0
                                    THEN UPPER(LEFT(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 1)) + 
                                         LOWER(SUBSTRING(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 2, 
                                         CHARINDEX('.', LEFT(u.Email, CHARINDEX('@', u.Email) - 1)) - 2)) + ' ' +
                                         UPPER(SUBSTRING(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 
                                         CHARINDEX('.', LEFT(u.Email, CHARINDEX('@', u.Email) - 1)) + 1, 1)) +
                                         LOWER(SUBSTRING(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 
                                         CHARINDEX('.', LEFT(u.Email, CHARINDEX('@', u.Email) - 1)) + 2, 50))
                                    ELSE UPPER(LEFT(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 1)) + 
                                         LOWER(SUBSTRING(LEFT(u.Email, CHARINDEX('@', u.Email) - 1), 2, 50))
                                END
                                ELSE 'Participant ' + RIGHT('000' + CAST(ISNULL(vcb.UserId, 1) as VARCHAR), 3)
                            END as ParticipantName,
                            
                            vcb.BookingStatus as Status,
                            vcb.BookingDate as LastActivity,
                            
                            -- Additional fields for better participant tracking
                            CASE 
                                WHEN vcb.BookingDate >= DATEADD(MINUTE, -5, GETDATE()) THEN 'Recently Active'
                                WHEN vcb.BookingDate >= DATEADD(MINUTE, -15, GETDATE()) THEN 'Active'
                                ELSE 'Idle'
                            END as ActivityStatus,
                            
                            DATEDIFF(MINUTE, vcb.BookingDate, GETDATE()) as MinutesAgo,
                            
                            -- Add ORDER BY fields to SELECT for DISTINCT compatibility
                            CASE WHEN vcb.BookingStatus = 'Connected' THEN 1
                                 WHEN vcb.BookingStatus = 'In Call' THEN 2
                                 ELSE 3 END as StatusPriority
                            
                        FROM VideoCallBookings vcb
                        LEFT JOIN Users u ON vcb.UserId = u.Id
                        WHERE vcb.SessionId = @SessionId
                        AND vcb.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected', 'In Call')
                        AND vcb.CustomerPhone IS NOT NULL
                        AND LEN(TRIM(vcb.CustomerPhone)) > 4
                        
                        -- ENHANCEMENT: Include recent participants even if status changed
                        AND (
                            vcb.BookingStatus IN ('Connected', 'In Call') 
                            OR vcb.BookingDate >= DATEADD(MINUTE, -30, GETDATE())
                        )
                        
                        ORDER BY 
                            StatusPriority,
                            vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);

                        System.Diagnostics.Debug.WriteLine(string.Format("🔍 Executing ENHANCED participant query for session {0}", sessionId));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string phone = reader["Phone"]?.ToString();
                                if (string.IsNullOrEmpty(phone)) continue;

                                string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

                                // Enhanced phone validation
                                if (string.IsNullOrEmpty(cleanPhone) ||
                                    cleanPhone.Length < 4 ||
                                    cleanPhone == "12345678" ||
                                    cleanPhone == "1234567890")
                                {
                                    System.Diagnostics.Debug.WriteLine(string.Format("⏭️ Skipping invalid phone: {0}", cleanPhone));
                                    continue;
                                }

                                string participantName = reader["ParticipantName"]?.ToString() ?? "Unknown Participant";
                                string status = reader["Status"]?.ToString() ?? "Connected";
                                string activityStatus = reader["ActivityStatus"]?.ToString() ?? "Active";
                                int minutesAgo = reader["MinutesAgo"] != DBNull.Value ? Convert.ToInt32(reader["MinutesAgo"]) : 0;
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
                                    activityStatus = activityStatus,
                                    minutesAgo = minutesAgo,
                                    peerId = string.Format("customer_{0}", cleanPhone),
                                    lastActive = reader["LastActivity"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["LastActivity"]).ToString("HH:mm")
                                        : "Unknown",
                                    isRecentlyActive = minutesAgo <= 5,
                                    connectionPriority = status == "Connected" ? 1 : status == "In Call" ? 2 : 3
                                });

                                System.Diagnostics.Debug.WriteLine(string.Format("✅ Found participant: {0} ({1}) - Status: {2} - Activity: {3} ({4}m ago)",
                                    participantName, cleanPhone, status, activityStatus, minutesAgo));
                            }
                        }
                    }

                    // ENHANCEMENT: Also check for any recent connection attempts or manual additions
                    if (participants.Count == 0)
                    {
                        // Look for any recent bookings that might have been missed
                        string fallbackQuery = @"
                            SELECT CustomerPhone, CustomerName, BookingStatus, BookingDate
                            FROM VideoCallBookings 
                            WHERE SessionId = @SessionId 
                            AND CustomerPhone IS NOT NULL
                            AND LEN(TRIM(CustomerPhone)) > 4
                            AND BookingDate >= DATEADD(HOUR, -2, GETDATE())
                            ORDER BY BookingDate DESC";

                        using (SqlCommand fallbackCmd = new SqlCommand(fallbackQuery, conn))
                        {
                            fallbackCmd.Parameters.AddWithValue("@SessionId", sessionId);

                            using (SqlDataReader fallbackReader = fallbackCmd.ExecuteReader())
                            {
                                while (fallbackReader.Read())
                                {
                                    string phone = fallbackReader["CustomerPhone"]?.ToString();
                                    string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

                                    if (!string.IsNullOrEmpty(cleanPhone) && cleanPhone.Length >= 4)
                                    {
                                        string name = fallbackReader["CustomerName"]?.ToString() ?? ("Participant " + cleanPhone);
                                        string status = fallbackReader["BookingStatus"]?.ToString() ?? "Connected";

                                        participants.Add(new
                                        {
                                            sessionId = sessionId,
                                            bookingId = 0,
                                            userId = 0,
                                            phone = cleanPhone,
                                            name = name,
                                            status = status,
                                            activityStatus = "Fallback",
                                            minutesAgo = 0,
                                            peerId = string.Format("customer_{0}", cleanPhone),
                                            lastActive = DateTime.Now.ToString("HH:mm"),
                                            isRecentlyActive = true,
                                            connectionPriority = 1
                                        });

                                        System.Diagnostics.Debug.WriteLine(string.Format("🔄 Fallback participant found: {0} ({1})", name, cleanPhone));
                                    }
                                }
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine(string.Format("🔍 GetSessionParticipants - Found {0} participants for session {1}", participants.Count, sessionId));

                // Enhanced logging for debugging
                foreach (var participant in participants)
                {
                    var phoneValue = participant.GetType().GetProperty("phone").GetValue(participant);
                    var nameValue = participant.GetType().GetProperty("name").GetValue(participant);
                    var statusValue = participant.GetType().GetProperty("status").GetValue(participant);
                    var activityValue = participant.GetType().GetProperty("activityStatus").GetValue(participant);
                    System.Diagnostics.Debug.WriteLine(string.Format("  - Participant: {0} ({1}) - Status: {2} - Activity: {3}",
                        nameValue, phoneValue, statusValue, activityValue));
                }

                var result = new
                {
                    success = true,
                    participants = participants,
                    totalCount = participants.Count,
                    sessionId = sessionId,
                    timestamp = DateTime.Now.ToString("HH:mm:ss"),
                    serverTime = DateTime.Now,
                    debugInfo = string.Format("Enhanced query executed at {0:HH:mm:ss}, found {1} participants", DateTime.Now, participants.Count),
                    queryOptimized = true,
                    includesRecentActivity = true
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Error getting session participants: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Stack trace: {0}", ex.StackTrace));

                var result = new
                {
                    success = false,
                    message = ex.Message,
                    participants = new List<object>(),
                    totalCount = 0,
                    sessionId = sessionId,
                    timestamp = DateTime.Now.ToString("HH:mm:ss"),
                    debugInfo = string.Format("Error occurred at {0:HH:mm:ss}: {1}", DateTime.Now, ex.Message),
                    queryOptimized = false
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // ENHANCEMENT: Real-time participant status update method
        [WebMethod]
        public static string UpdateParticipantStatus(int sessionId, string phoneNumber, string status)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

                    UpdateCustomerConnectionStatus(conn, sessionId, cleanPhone, status);

                    var result = new
                    {
                        success = true,
                        message = string.Format("Status updated to {0} for participant {1}", status, cleanPhone)
                    };

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    return serializer.Serialize(result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Error updating participant status: {0}", ex.Message));

                var result = new
                {
                    success = false,
                    message = ex.Message
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // ENHANCED: Method to manually add test participants for testing participant connections
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

                            var result = new { success = true, message = string.Format("Test participant {0} added successfully", participantName) };
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
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Error adding test participant: {0}", ex.Message));

                var result = new { success = false, message = ex.Message };
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // ENHANCED: Method to simulate multiple participants for comprehensive testing
        [WebMethod]
        public static string CreateTestParticipants(int sessionId)
        {
            try
            {
                var testParticipants = new List<(string phone, string name)>
                {
                    ("91234567", "Alice Johnson"),
                    ("92345678", "Bob Smith"),
                    ("93456789", "Carol Davis"),
                    ("94567890", "David Wilson"),
                    ("95678901", "Emma Brown")
                };

                int created = 0;
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    foreach (var participant in testParticipants)
                    {
                        // Check if already exists
                        string checkQuery = "SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = @SessionId AND CustomerPhone = @Phone";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@SessionId", sessionId);
                            checkCmd.Parameters.AddWithValue("@Phone", participant.phone);

                            int existing = Convert.ToInt32(checkCmd.ExecuteScalar());
                            if (existing == 0)
                            {
                                // Create booking
                                string insertQuery = @"
                                    INSERT INTO VideoCallBookings (
                                        SessionId, UserId, CustomerName, CustomerPhone, CustomerEmail,
                                        BookingDate, BookingStatus, ScamConcerns, PointsUsed
                                    ) VALUES (
                                        @SessionId, 1, @CustomerName, @CustomerPhone, @CustomerEmail,
                                        @BookingDate, 'Connected', 'General consultation', 0
                                    )";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                    insertCmd.Parameters.AddWithValue("@CustomerName", participant.name);
                                    insertCmd.Parameters.AddWithValue("@CustomerPhone", participant.phone);
                                    insertCmd.Parameters.AddWithValue("@CustomerEmail", participant.name.Replace(" ", "").ToLower() + "@example.com");
                                    insertCmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);

                                    insertCmd.ExecuteNonQuery();
                                    created++;
                                }
                            }
                        }
                    }
                }

                var result = new { success = true, message = string.Format("Created {0} test participants for session {1}", created, sessionId) };
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Error creating test participants: {0}", ex.Message));

                var result = new { success = false, message = ex.Message };
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // ADDITIONAL: Helper method for creating test participants with specific session
        public static string CreateTestParticipantsForSession(int sessionId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // Test participants with different phone numbers
                    var testParticipants = new[]
                    {
                        new { Phone = "91234567", Name = "Alice Johnson", Email = "alice.johnson@email.com" },
                        new { Phone = "92345678", Name = "Bob Smith", Email = "bob.smith@email.com" },
                        new { Phone = "93456789", Name = "Carol Wilson", Email = "carol.wilson@email.com" }
                    };

                    int created = 0;

                    foreach (var participant in testParticipants)
                    {
                        // Check if already exists
                        string checkQuery = @"
                            SELECT COUNT(*) FROM VideoCallBookings 
                            WHERE SessionId = @SessionId AND CustomerPhone = @Phone";

                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@SessionId", sessionId);
                            checkCmd.Parameters.AddWithValue("@Phone", participant.Phone);

                            int existing = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (existing == 0)
                            {
                                string insertQuery = @"
                                    INSERT INTO VideoCallBookings (
                                        SessionId, UserId, CustomerName, CustomerPhone, CustomerEmail,
                                        BookingDate, BookingStatus, ScamConcerns, PointsUsed
                                    ) VALUES (
                                        @SessionId, 1, @CustomerName, @CustomerPhone, @CustomerEmail,
                                        @BookingDate, 'Connected', 'General consultation', 0
                                    )";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                    insertCmd.Parameters.AddWithValue("@CustomerName", participant.Name);
                                    insertCmd.Parameters.AddWithValue("@CustomerPhone", participant.Phone);
                                    insertCmd.Parameters.AddWithValue("@CustomerEmail", participant.Email);
                                    insertCmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);

                                    insertCmd.ExecuteNonQuery();
                                    created++;
                                }
                            }
                        }
                    }

                    var result = new
                    {
                        success = true,
                        message = string.Format("Created {0} test participants for session {1}", created, sessionId),
                        participants = testParticipants.Select(p => new { p.Name, p.Phone }).ToArray()
                    };

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    return serializer.Serialize(result);
                }
            }
            catch (Exception ex)
            {
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
                        System.Diagnostics.Debug.WriteLine(string.Format("Customer session ended for phone: {0}", hdnCustomerPhone.Value));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Error updating session on unload: {0}", ex.Message));
                }
            }
            base.OnUnload(e);
        }
    }
}