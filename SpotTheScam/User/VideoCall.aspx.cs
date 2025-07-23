using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Services;
using System.Web.Script.Serialization;

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
                        AND BookingStatus = 'Confirmed'
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
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting user phone: {ex.Message}");
            }

            return null;
        }

        [WebMethod]
        public static string CheckSession(string phoneNumber)
        {
            var result = new
            {
                success = false,
                message = "",
                sessionId = "",
                staffName = "",
                sessionDate = "",
                sessionTime = "",
                duration = ""
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine($"🔍 Checking session for phone: {phoneNumber}");

                    // Generate all possible phone format variations
                    var phoneVariations = GetPhoneVariations(phoneNumber);

                    System.Diagnostics.Debug.WriteLine($"🔍 Phone variations to check:");
                    foreach (var variation in phoneVariations)
                    {
                        System.Diagnostics.Debug.WriteLine($"   - {variation}");
                    }

                    // Build dynamic query with all phone variations
                    string phoneConditions = "";
                    for (int i = 0; i < phoneVariations.Count; i++)
                    {
                        if (i > 0) phoneConditions += " OR ";
                        phoneConditions += $"vcb.CustomerPhone = @Phone{i}";
                    }

                    string query = $@"
                        SELECT TOP 1 
                            vcb.SessionId,
                            COALESCE(vcb.CustomerName, 'Participant') as CustomerName,
                            vcb.CustomerPhone,
                            vcb.BookingStatus,
                            vcb.BookingDate,
                            vcb.UserId,
                            es.SessionTitle,
                            es.SessionDate,
                            es.StartTime,
                            es.EndTime,
                            es.ExpertName,
                            es.ExpertTitle,
                            es.Status as SessionStatus
                        FROM VideoCallBookings vcb
                        INNER JOIN ExpertSessions es ON vcb.SessionId = es.Id
                        WHERE ({phoneConditions})
                        AND vcb.BookingStatus = 'Confirmed'
                        AND es.Status = 'Available'
                        ORDER BY 
                            CASE 
                                WHEN es.SessionDate = CAST(GETDATE() AS DATE) THEN 1
                                WHEN es.SessionDate > CAST(GETDATE() AS DATE) THEN 2
                                ELSE 3
                            END,
                            vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add all phone variation parameters
                        for (int i = 0; i < phoneVariations.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@Phone{i}", phoneVariations[i]);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Found an active session registration
                                DateTime sessionDate = reader["SessionDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["SessionDate"])
                                    : DateTime.Today;

                                TimeSpan startTime = reader["StartTime"] != DBNull.Value
                                    ? (TimeSpan)reader["StartTime"]
                                    : DateTime.Now.TimeOfDay;

                                TimeSpan endTime = reader["EndTime"] != DBNull.Value
                                    ? (TimeSpan)reader["EndTime"]
                                    : DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1));

                                // More lenient timing - allow joining anytime on session day or 24 hours around it
                                DateTime sessionDateTime = sessionDate.Add(startTime);
                                DateTime now = DateTime.Now;
                                TimeSpan timeDifference = now - sessionDateTime;

                                bool canJoin = Math.Abs(timeDifference.TotalHours) <= 24; // 24 hour window

                                System.Diagnostics.Debug.WriteLine($"📅 Session Date: {sessionDate:yyyy-MM-dd}");
                                System.Diagnostics.Debug.WriteLine($"🕐 Session Time: {startTime} - {endTime}");
                                System.Diagnostics.Debug.WriteLine($"⏰ Current Time: {now}");
                                System.Diagnostics.Debug.WriteLine($"⏱️ Time Difference: {timeDifference.TotalHours:F1} hours");
                                System.Diagnostics.Debug.WriteLine($"✅ Can Join: {canJoin}");

                                if (canJoin)
                                {
                                    result = new
                                    {
                                        success = true,
                                        message = "Expert session found! Connecting...",
                                        sessionId = reader["SessionId"].ToString(),
                                        staffName = reader["ExpertName"]?.ToString() ?? "Scam Prevention Expert",
                                        sessionDate = sessionDate.ToString("dd/MM/yyyy"),
                                        sessionTime = startTime.ToString(@"hh\:mm"),
                                        duration = "60"
                                    };

                                    System.Diagnostics.Debug.WriteLine($"✅ Expert session found! SessionId: {result.sessionId}");
                                }
                                else
                                {
                                    result = new
                                    {
                                        success = false,
                                        message = $"Your session was scheduled for {sessionDateTime:dd/MM/yyyy HH:mm}. Please join closer to your scheduled time.",
                                        sessionId = "",
                                        staffName = "",
                                        sessionDate = "",
                                        sessionTime = "",
                                        duration = ""
                                    };
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("❌ No active session found with any phone variation");

                                result = new
                                {
                                    success = false,
                                    message = $"No session registration found for phone number {phoneNumber}. Please register for a session first through our webinar system.",
                                    sessionId = "",
                                    staffName = "",
                                    sessionDate = "",
                                    sessionTime = "",
                                    duration = ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error checking session: {ex.Message}");
                result = new
                {
                    success = false,
                    message = "Error connecting to expert session. Please try again or contact support.",
                    sessionId = "",
                    staffName = "",
                    sessionDate = "",
                    sessionTime = "",
                    duration = ""
                };
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(result);
        }

        // Generate all possible phone number variations
        private static System.Collections.Generic.List<string> GetPhoneVariations(string phone)
        {
            var variations = new System.Collections.Generic.List<string>();

            if (string.IsNullOrEmpty(phone))
                return variations;

            // Add original
            variations.Add(phone);

            // Clean phone (remove all non-digits)
            string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

            if (!variations.Contains(cleanPhone))
                variations.Add(cleanPhone);

            // If it's 8 digits, it's likely local Singapore number
            if (cleanPhone.Length == 8)
            {
                string withSG = $"+65{cleanPhone}";
                string withSGNoPlus = $"65{cleanPhone}";

                if (!variations.Contains(withSG))
                    variations.Add(withSG);
                if (!variations.Contains(withSGNoPlus))
                    variations.Add(withSGNoPlus);
            }

            // If it's 10 digits starting with 65, create variations
            if (cleanPhone.Length == 10 && cleanPhone.StartsWith("65"))
            {
                string localOnly = cleanPhone.Substring(2); // Remove 65 prefix
                string withPlus = $"+{cleanPhone}";

                if (!variations.Contains(localOnly))
                    variations.Add(localOnly);
                if (!variations.Contains(withPlus))
                    variations.Add(withPlus);
            }

            // If it starts with +65, create variations
            if (phone.StartsWith("+65"))
            {
                string without65 = phone.Substring(3); // Remove +65
                string withoutPlus = phone.Substring(1); // Remove +

                if (!variations.Contains(without65))
                    variations.Add(without65);
                if (!variations.Contains(withoutPlus))
                    variations.Add(withoutPlus);
            }

            // Remove any duplicates and empty strings
            var uniqueVariations = new System.Collections.Generic.List<string>();
            foreach (var variation in variations)
            {
                if (!string.IsNullOrEmpty(variation) && !uniqueVariations.Contains(variation))
                {
                    uniqueVariations.Add(variation);
                }
            }

            return uniqueVariations;
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

                        // Update session status when customer leaves
                        string updateQuery = @"
                            UPDATE VideoCallBookings 
                            SET BookingStatus = 'Customer Disconnected'
                            WHERE CustomerPhone = @CustomerPhone 
                            AND BookingStatus = 'Confirmed'";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@CustomerPhone", hdnCustomerPhone.Value);
                            int rowsUpdated = cmd.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine($"Customer session ended for phone: {hdnCustomerPhone.Value}, rows updated: {rowsUpdated}");
                        }
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