using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace SpotTheScam.Staff
{
    public partial class StaffVideoCall : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if expert came with a direct session link
                string sessionIdString = Request.QueryString["sessionId"];
                string customerPhone = Request.QueryString["phone"];

                if (!string.IsNullOrEmpty(sessionIdString) && !string.IsNullOrEmpty(customerPhone))
                {
                    // Direct session access
                    hdnSessionId.Value = sessionIdString;
                    hdnCustomerPhone.Value = customerPhone;

                    // Pre-fill phone input
                    string script = $@"
                        window.onload = function() {{
                            document.getElementById('phoneInput').value = '{customerPhone}';
                            connectToCustomer();
                        }};";
                    ClientScript.RegisterStartupScript(this.GetType(), "AutoConnect", script, true);
                }

                System.Diagnostics.Debug.WriteLine("Expert Video Call page loaded");
            }
        }

        [WebMethod]
        public static string CheckCustomerSession(string phoneNumber)
        {
            var result = new
            {
                success = false,
                message = "",
                sessionId = "",
                customerName = "",
                customerPhone = "",
                sessionDate = "",
                sessionTime = "",
                concerns = ""
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine($"🔍 STAFF: Checking customer session for phone: {phoneNumber}");

                    // Generate all possible phone format variations (same as user side)
                    var phoneVariations = GetPhoneVariations(phoneNumber);

                    System.Diagnostics.Debug.WriteLine($"🔍 STAFF: Phone variations to check:");
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
                            COALESCE(vcb.CustomerName, 'Webinar Participant') as CustomerName,
                            vcb.CustomerPhone,
                            vcb.BookingStatus,
                            vcb.BookingDate,
                            vcb.UserId,
                            COALESCE(vcb.ScamConcerns, 'General consultation') as Concerns,
                            es.SessionTitle,
                            es.SessionDate,
                            es.StartTime,
                            es.EndTime,
                            es.ExpertName,
                            es.Status as SessionStatus
                        FROM VideoCallBookings vcb
                        LEFT JOIN ExpertSessions es ON vcb.SessionId = es.Id
                        WHERE ({phoneConditions})
                        AND vcb.BookingStatus = 'Confirmed'
                        ORDER BY vcb.BookingDate DESC";

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
                                // Found the customer session
                                string sessionId = reader["SessionId"]?.ToString() ?? "";
                                string customerName = reader["CustomerName"]?.ToString() ?? "Webinar Participant";
                                string customerPhone = reader["CustomerPhone"]?.ToString() ?? phoneNumber;
                                string concerns = reader["Concerns"]?.ToString() ?? "General scam prevention consultation";

                                // Get session details
                                string sessionDate = "Today";
                                string sessionTime = "Current";

                                if (reader["SessionDate"] != DBNull.Value)
                                {
                                    DateTime sessDate = Convert.ToDateTime(reader["SessionDate"]);
                                    sessionDate = sessDate.ToString("dd/MM/yyyy");
                                }

                                if (reader["StartTime"] != DBNull.Value)
                                {
                                    TimeSpan startTime = (TimeSpan)reader["StartTime"];
                                    sessionTime = startTime.ToString(@"hh\:mm");
                                }

                                result = new
                                {
                                    success = true,
                                    message = "Participant session found! Connecting...",
                                    sessionId = sessionId,
                                    customerName = customerName,
                                    customerPhone = customerPhone,
                                    sessionDate = sessionDate,
                                    sessionTime = sessionTime,
                                    concerns = concerns
                                };

                                System.Diagnostics.Debug.WriteLine($"✅ STAFF: Found customer session! SessionId: {sessionId}, Customer: {customerName}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("❌ STAFF: No customer session found with any phone variation");

                                result = new
                                {
                                    success = false,
                                    message = "No active participant session found. Please ask the participant to join first.",
                                    sessionId = "",
                                    customerName = "",
                                    customerPhone = "",
                                    sessionDate = "",
                                    sessionTime = "",
                                    concerns = ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ STAFF: Error checking customer session: {ex.Message}");
                result = new
                {
                    success = false,
                    message = "Error connecting to participant session. Please try again.",
                    sessionId = "",
                    customerName = "",
                    customerPhone = "",
                    sessionDate = "",
                    sessionTime = "",
                    concerns = ""
                };
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(result);
        }

        [WebMethod]
        public static string GetWaitingParticipants()
        {
            List<object> participants = new List<object>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // Get participants who are currently registered and confirmed
                    string query = @"
                        SELECT DISTINCT
                            vcb.CustomerPhone as Phone,
                            COALESCE(vcb.CustomerName, 'Participant') as Name,
                            COALESCE(vcb.ScamConcerns, 'General inquiry') as Concerns,
                            vcb.BookingDate as WaitingTime
                        FROM VideoCallBookings vcb
                        WHERE vcb.BookingStatus = 'Confirmed'
                        AND vcb.BookingDate >= DATEADD(hour, -24, GETDATE()) -- Last 24 hours
                        ORDER BY vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                participants.Add(new
                                {
                                    phone = reader["Phone"]?.ToString() ?? "",
                                    name = reader["Name"]?.ToString() ?? "Participant",
                                    concerns = reader["Concerns"]?.ToString() ?? "General inquiry",
                                    waitingTime = reader["WaitingTime"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["WaitingTime"]).ToString("HH:mm") :
                                        DateTime.Now.ToString("HH:mm")
                                });
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"✅ STAFF: Found {participants.Count} waiting participants");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ STAFF: Error getting waiting participants: {ex.Message}");
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(participants);
        }

        // Generate all possible phone number variations (same logic as user side)
        private static List<string> GetPhoneVariations(string phone)
        {
            var variations = new List<string>();

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
            var uniqueVariations = new List<string>();
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

                        // Update session status when expert leaves
                        string updateQuery = @"
                            UPDATE VideoCallBookings 
                            SET BookingStatus = 'Expert Disconnected'
                            WHERE CustomerPhone = @CustomerPhone 
                            AND BookingStatus = 'Confirmed'";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@CustomerPhone", hdnCustomerPhone.Value);
                            cmd.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine($"Expert session ended for phone: {hdnCustomerPhone.Value}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating session on expert unload: {ex.Message}");
                }
            }
            base.OnUnload(e);
        }
    }
}