using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Collections.Generic;

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

                    // FIXED: Look for existing booking with this phone number first
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
                        WHERE vcb.CustomerPhone = @Phone 
                        AND vcb.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected')
                        AND es.Status = 'Available'
                        ORDER BY vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(findBookingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);
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
                                UpdateCustomerConnectionStatus(conn, sessionId, phoneNumber, "Connected");

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

                    // FIXED: If no existing booking, look for any available session and create booking
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

                                // FIXED: Create new booking for this phone number
                                CreateOrUpdateBooking(conn, sessionId, phoneNumber);

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

        // FIXED: Create or update booking for the participant
        private static void CreateOrUpdateBooking(SqlConnection conn, int sessionId, string phoneNumber)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔄 Creating/updating booking for session {sessionId}, phone {phoneNumber}");

                // Clean phone number
                string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

                // Check if booking already exists with any phone variation
                var phoneVariations = new List<string> { phoneNumber, cleanPhone, "+" + cleanPhone, "65" + cleanPhone };

                string checkBookingQuery = @"
                    SELECT COUNT(*) FROM VideoCallBookings 
                    WHERE SessionId = @SessionId 
                    AND CustomerPhone IN ('" + string.Join("','", phoneVariations) + "')";

                using (SqlCommand checkCmd = new SqlCommand(checkBookingQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@SessionId", sessionId);

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
                            insertCmd.Parameters.AddWithValue("@CustomerName", $"Participant {cleanPhone}");
                            insertCmd.Parameters.AddWithValue("@CustomerPhone", cleanPhone);
                            insertCmd.Parameters.AddWithValue("@CustomerEmail", $"participant{cleanPhone}@example.com");
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

        // FIXED: Helper method to update customer connection status
        private static void UpdateCustomerConnectionStatus(SqlConnection conn, int sessionId, string phoneNumber, string status)
        {
            try
            {
                string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");
                var phoneVariations = new List<string> { phoneNumber, cleanPhone, "+" + cleanPhone, "65" + cleanPhone };

                string updateBookingQuery = @"
                    UPDATE VideoCallBookings 
                    SET BookingStatus = @Status, BookingDate = @BookingDate
                    WHERE SessionId = @SessionId 
                    AND CustomerPhone IN ('" + string.Join("','", phoneVariations) + "')";

                using (SqlCommand updateCmd = new SqlCommand(updateBookingQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@SessionId", sessionId);
                    updateCmd.Parameters.AddWithValue("@Status", status);
                    updateCmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);

                    updateCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error updating customer connection status: {ex.Message}");
            }
        }

        // NEW: Method to notify staff when participant joins
        [WebMethod]
        public static string NotifyStaffParticipantJoined(string sessionId, string phoneNumber, string _)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // Update participant status to 'Connected'
                    UpdateCustomerConnectionStatus(conn, Convert.ToInt32(sessionId), phoneNumber, "Connected");

                    var result = new
                    {
                        success = true,
                        message = "Staff notified of participant connection"
                    };

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    return serializer.Serialize(result);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error notifying staff: {ex.Message}");

                var result = new
                {
                    success = false,
                    message = ex.Message
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // Add this method to your VideoCall.aspx.cs file

        [WebMethod]
        public static string GetSessionParticipants(int sessionId)
        {
            try
            {
                var participants = new List<object>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT DISTINCT
                    COALESCE(vcb.CustomerPhone, wr.Phone, '12345678') as Phone,
                    COALESCE(vcb.CustomerName, wr.FirstName + ' ' + wr.LastName, 'Participant') as Name,
                    COALESCE(vcb.BookingStatus, 'Confirmed') as Status
                FROM VideoCallBookings vcb
                FULL OUTER JOIN WebinarRegistrations wr ON vcb.UserId = wr.UserId
                WHERE (vcb.SessionId = @SessionId OR wr.SessionId = @SessionId)
                AND (vcb.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected') OR wr.IsActive = 1)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string phone = reader["Phone"].ToString();
                                string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

                                participants.Add(new
                                {
                                    phone = cleanPhone,
                                    name = reader["Name"].ToString(),
                                    status = reader["Status"].ToString()
                                });
                            }
                        }
                    }
                }

                var result = new { success = true, participants = participants };
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting session participants: {ex.Message}");

                var result = new { success = false, message = ex.Message, participants = new List<object>() };
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
                        UpdateCustomerConnectionStatus(conn, Convert.ToInt32(hdnSessionId.Value), hdnCustomerPhone.Value, "Customer Disconnected");
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