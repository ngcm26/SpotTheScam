using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Linq;

namespace SpotTheScam.Staff
{
    public partial class StaffVideoCall : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if staff is logged in - FIXED: Redirect to UserLogin.aspx
                if (Session["StaffName"] == null && Session["StaffUsername"] == null && Session["Username"] == null)
                {
                    Response.Redirect("~/User/UserLogin.aspx");
                    return;
                }

                LoadSessionData();
            }
            else
            {
                // Handle postback events
                string eventTarget = Request["__EVENTTARGET"];
                string eventArgument = Request["__EVENTARGUMENT"];

                if (eventArgument == "refresh")
                {
                    LoadSessionData();
                }
            }
        }

        private void LoadSessionData()
        {
            // Get session ID from query string or previous page
            string sessionIdStr = Request.QueryString["sessionId"];
            string mode = Request.QueryString["mode"]; // New: mode parameter
            string participants = Request.QueryString["participants"]; // New: selected participants

            System.Diagnostics.Debug.WriteLine("=== LoadSessionData called ===");
            System.Diagnostics.Debug.WriteLine("Query string sessionId: " + sessionIdStr);
            System.Diagnostics.Debug.WriteLine("Mode: " + mode);
            System.Diagnostics.Debug.WriteLine("Participants: " + participants);

            int sessionId;
            if (string.IsNullOrEmpty(sessionIdStr) || !int.TryParse(sessionIdStr, out sessionId))
            {
                // Try to get from manage participants page
                if (Session["CurrentSessionId"] != null)
                {
                    sessionId = Convert.ToInt32(Session["CurrentSessionId"]);
                    System.Diagnostics.Debug.WriteLine("Using session ID from Session: " + sessionId);
                }
                else
                {
                    ShowError("Invalid session ID. Please select a session first.");
                    System.Diagnostics.Debug.WriteLine("ERROR: No valid session ID found");
                    return;
                }
            }

            hdnSessionId.Value = sessionId.ToString();
            Session["CurrentSessionId"] = sessionId;

            // Store mode and participant info for JavaScript
            if (!string.IsNullOrEmpty(mode))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SetMode",
                    $"sessionMode = '{mode}'; selectedParticipantIds = '{participants}';", true);
            }

            System.Diagnostics.Debug.WriteLine("Processing session ID: " + sessionId);

            try
            {
                LoadSessionInfo(sessionId);
                LoadRegisteredParticipants(sessionId);

                System.Diagnostics.Debug.WriteLine("✅ LoadSessionData completed successfully");
            }
            catch (Exception ex)
            {
                ShowError("Error loading session data: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("❌ Error loading session data: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("❌ Stack trace: " + ex.StackTrace);
            }
        }

        private void LoadSessionInfo(int sessionId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        es.SessionTitle, es.SessionDescription, es.SessionDate, 
                        CONVERT(varchar, es.StartTime, 108) as StartTime,
                        CONVERT(varchar, es.EndTime, 108) as EndTime,
                        es.ExpertName, es.SessionTopic, es.MaxParticipants,
                        ISNULL(es.CurrentParticipants, 0) as CurrentParticipants
                    FROM ExpertSessions es
                    WHERE es.Id = @SessionId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SessionId", sessionId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string sessionTitle = reader["SessionTitle"].ToString();
                            string sessionDescription = reader["SessionDescription"].ToString();
                            DateTime sessionDate = Convert.ToDateTime(reader["SessionDate"]);
                            string startTime = reader["StartTime"].ToString();
                            string endTime = reader["EndTime"].ToString();
                            string expertName = reader["ExpertName"].ToString();
                            string sessionTopic = reader["SessionTopic"].ToString();

                            string sessionInfo = string.Format(@"
                                <p><strong>{0}</strong></p>
                                <p>{1}</p>
                                <p><strong>📅 Date:</strong> {2:dd/MM/yyyy} | 
                                   <strong>🕐 Time:</strong> {3} - {4} | 
                                   <strong>👨‍💼 Expert:</strong> {5} | 
                                   <strong>📚 Topic:</strong> {6}</p>",
                                sessionTitle, sessionDescription, sessionDate, startTime, endTime, expertName, sessionTopic);

                            lblSessionInfo.Text = sessionInfo;
                        }
                        else
                        {
                            ShowError("Session not found.");
                        }
                    }
                }
            }
        }

        private void LoadRegisteredParticipants(int sessionId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        vcb.UserId,
                        COALESCE(vcb.CustomerName, u.Name, 'Participant') as CustomerName,
                        COALESCE(vcb.CustomerPhone, u.PhoneNumber, 'No phone') as CustomerPhone,
                        COALESCE(vcb.CustomerEmail, u.Email, 'No email provided') as CustomerEmail,
                        vcb.BookingDate,
                        vcb.BookingStatus,
                        COALESCE(vcb.ScamConcerns, 'General consultation') as ScamConcerns,
                        COALESCE(vcb.PointsUsed, 0) as PointsUsed
                    FROM VideoCallBookings vcb
                    LEFT JOIN Users u ON vcb.UserId = u.Id
                    WHERE vcb.SessionId = @SessionId 
                    AND vcb.BookingStatus = 'Confirmed'
                    ORDER BY vcb.BookingDate ASC";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@SessionId", sessionId);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    System.Diagnostics.Debug.WriteLine("=== LoadRegisteredParticipants ===");
                    System.Diagnostics.Debug.WriteLine("Found " + dt.Rows.Count + " registered participants for session " + sessionId);

                    if (dt.Rows.Count > 0)
                    {
                        // Debug: Print participant data
                        foreach (DataRow row in dt.Rows)
                        {
                            System.Diagnostics.Debug.WriteLine("Participant: " + row["CustomerName"] + ", Phone: " + row["CustomerPhone"] + ", Email: " + row["CustomerEmail"]);
                        }

                        rptRegisteredParticipants.DataSource = dt;
                        rptRegisteredParticipants.DataBind();
                        pnlNoRegistered.Visible = false;

                        // Update counts
                        lblTotalParticipants.Text = dt.Rows.Count.ToString();
                        lblRegisteredCount.Text = dt.Rows.Count.ToString();

                        System.Diagnostics.Debug.WriteLine("✅ Participants loaded and bound to repeater. Count: " + dt.Rows.Count);
                        System.Diagnostics.Debug.WriteLine("✅ lblTotalParticipants.Text set to: " + lblTotalParticipants.Text);
                        System.Diagnostics.Debug.WriteLine("✅ lblRegisteredCount.Text set to: " + lblRegisteredCount.Text);

                        // Add JavaScript to update the UI after page load
                        string updateScript = $@"
                            document.addEventListener('DOMContentLoaded', function() {{
                                console.log('Server-side participant count: {dt.Rows.Count}');
                                document.getElementById('totalParticipants').textContent = '{dt.Rows.Count}';
                                document.getElementById('onlineParticipants').textContent = '0';
                                document.getElementById('connectedParticipants').textContent = '0';
                                
                                // Force update participant counts after a short delay
                                setTimeout(function() {{
                                    updateParticipantCounts();
                                    checkOnlineParticipants();
                                }}, 1000);
                            }});
                        ";
                        ClientScript.RegisterStartupScript(this.GetType(), "UpdateParticipantCounts", updateScript, true);
                    }
                    else
                    {
                        rptRegisteredParticipants.DataSource = null;
                        rptRegisteredParticipants.DataBind();
                        pnlNoRegistered.Visible = true;

                        lblTotalParticipants.Text = "0";
                        lblRegisteredCount.Text = "0";

                        System.Diagnostics.Debug.WriteLine("⚠️ No registered participants found for session " + sessionId);

                        // Add JavaScript to show zero counts
                        string zeroScript = @"
                            document.addEventListener('DOMContentLoaded', function() {
                                document.getElementById('totalParticipants').textContent = '0';
                                document.getElementById('onlineParticipants').textContent = '0';
                                document.getElementById('connectedParticipants').textContent = '0';
                            });
                        ";
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowZeroCounts", zeroScript, true);
                    }
                }
            }
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = "status-message error";
        }

        // ===== ENHANCED WEB METHODS =====

        [WebMethod]
        public static string GetParticipantUpdates(int sessionId)
        {
            try
            {
                var participants = new List<object>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            vcb.UserId,
                            COALESCE(vcb.CustomerName, u.Name, 'Participant') as Name,
                            COALESCE(vcb.CustomerPhone, u.PhoneNumber, '') as Phone,
                            vcb.BookingStatus,
                            vcb.BookingDate,
                            CASE 
                                WHEN vcb.BookingStatus = 'Connected' THEN 'connected'
                                WHEN vcb.BookingStatus = 'Expert Ready' THEN 'waiting'
                                WHEN vcb.BookingStatus = 'Confirmed' AND DATEDIFF(minute, vcb.BookingDate, GETDATE()) <= 30 THEN 'online'
                                ELSE 'offline'
                            END as Status
                        FROM VideoCallBookings vcb
                        LEFT JOIN Users u ON vcb.UserId = u.Id
                        WHERE vcb.SessionId = @SessionId 
                        AND vcb.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected', 'In Call')
                        ORDER BY vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string phone = reader["Phone"]?.ToString() ?? "";
                                string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

                                participants.Add(new
                                {
                                    name = reader["Name"].ToString(),
                                    phone = cleanPhone,
                                    status = reader["Status"].ToString(),
                                    bookingStatus = reader["BookingStatus"].ToString(),
                                    lastActive = reader["BookingDate"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["BookingDate"]).ToString("HH:mm")
                                        : "Unknown"
                                });
                            }
                        }
                    }
                }

                var result = new
                {
                    success = true,
                    participants = participants,
                    timestamp = DateTime.Now.ToString("HH:mm:ss")
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    success = false,
                    participants = new List<object>(),
                    message = "Error: " + ex.Message,
                    timestamp = DateTime.Now.ToString("HH:mm:ss")
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        [WebMethod]
        public static string UpdateParticipantStatus(int sessionId, string phoneNumber, string newStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    var phoneVariations = GetPhoneVariations(phoneNumber);

                    string phoneConditions = "";
                    for (int i = 0; i < phoneVariations.Count; i++)
                    {
                        if (i > 0) phoneConditions += " OR ";
                        phoneConditions += $"vcb.CustomerPhone = @Phone{i}";
                    }

                    string updateQuery = $@"
                        UPDATE VideoCallBookings 
                        SET BookingStatus = @Status 
                        WHERE SessionId = @SessionId 
                        AND ({phoneConditions})";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);

                        for (int i = 0; i < phoneVariations.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@Phone{i}", phoneVariations[i]);
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();

                        var result = new
                        {
                            success = rowsAffected > 0,
                            message = rowsAffected > 0 ? "Status updated successfully" : "No records updated",
                            rowsAffected = rowsAffected
                        };

                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        return serializer.Serialize(result);
                    }
                }
            }
            catch (Exception ex)
            {
                var result = new
                {
                    success = false,
                    message = "Error updating status: " + ex.Message,
                    rowsAffected = 0
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        [WebMethod]
        public static string GetBulkParticipantData(int sessionId, string participantIds)
        {
            try
            {
                var participants = new List<object>();
                var idList = participantIds.Split(',').Where(id => !string.IsNullOrEmpty(id)).Select(int.Parse).ToList();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    foreach (int userId in idList)
                    {
                        string query = @"
                            SELECT 
                                vcb.UserId,
                                COALESCE(vcb.CustomerName, u.Name, 'Participant') as Name,
                                vcb.CustomerPhone as Phone,
                                COALESCE(vcb.CustomerEmail, u.Email, '') as Email,
                                vcb.ScamConcerns
                            FROM VideoCallBookings vcb
                            LEFT JOIN Users u ON vcb.UserId = u.Id
                            WHERE vcb.SessionId = @SessionId AND vcb.UserId = @UserId
                            AND vcb.BookingStatus = 'Confirmed'";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@SessionId", sessionId);
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    participants.Add(new
                                    {
                                        userId = Convert.ToInt32(reader["UserId"]),
                                        name = reader["Name"].ToString(),
                                        phone = reader["Phone"]?.ToString() ?? "",
                                        email = reader["Email"]?.ToString() ?? "",
                                        concerns = reader["ScamConcerns"]?.ToString() ?? "General"
                                    });
                                }
                            }
                        }
                    }
                }

                var result = new
                {
                    success = true,
                    participants = participants,
                    count = participants.Count,
                    message = $"Loaded {participants.Count} participants for bulk operation"
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    success = false,
                    participants = new List<object>(),
                    count = 0,
                    message = "Error: " + ex.Message
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        [WebMethod]
        public static string TestParticipantCount(int sessionId)
        {
            try
            {
                var participants = new List<object>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            vcb.CustomerName,
                            vcb.CustomerPhone,
                            vcb.CustomerEmail,
                            vcb.BookingStatus
                        FROM VideoCallBookings vcb
                        WHERE vcb.SessionId = @SessionId 
                        AND vcb.BookingStatus = 'Confirmed'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                participants.Add(new
                                {
                                    name = reader["CustomerName"] != null ? reader["CustomerName"].ToString() : "No name",
                                    phone = reader["CustomerPhone"] != null ? reader["CustomerPhone"].ToString() : "No phone",
                                    email = reader["CustomerEmail"] != null ? reader["CustomerEmail"].ToString() : "No email",
                                    status = reader["BookingStatus"] != null ? reader["BookingStatus"].ToString() : "Unknown"
                                });
                            }
                        }
                    }
                }

                var result = new
                {
                    success = true,
                    count = participants.Count,
                    message = "Found " + participants.Count + " confirmed participants for session " + sessionId,
                    participants = participants
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    success = false,
                    count = 0,
                    message = "Error: " + ex.Message,
                    participants = new List<object>()
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        [WebMethod]
        public static string GetOnlineParticipants(int sessionId)
        {
            var participants = new List<object>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // Get participants who have been active recently (last 5 minutes)
                    string query = @"
                        SELECT 
                            vcb.CustomerName,
                            vcb.CustomerPhone,
                            vcb.CustomerEmail,
                            vcb.ScamConcerns,
                            vcb.BookingDate
                        FROM VideoCallBookings vcb
                        WHERE vcb.SessionId = @SessionId 
                        AND vcb.BookingStatus = 'Confirmed'
                        AND vcb.BookingDate >= DATEADD(minute, -30, GETDATE())
                        ORDER BY vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string joinTime = DateTime.Now.ToString("HH:mm");
                                if (reader["BookingDate"] != DBNull.Value)
                                {
                                    joinTime = Convert.ToDateTime(reader["BookingDate"]).ToString("HH:mm");
                                }

                                participants.Add(new
                                {
                                    name = reader["CustomerName"] != null ? reader["CustomerName"].ToString() : "Participant",
                                    phone = reader["CustomerPhone"] != null ? reader["CustomerPhone"].ToString() : "",
                                    email = reader["CustomerEmail"] != null ? reader["CustomerEmail"].ToString() : "",
                                    concerns = reader["ScamConcerns"] != null ? reader["ScamConcerns"].ToString() : "General",
                                    joinTime = joinTime,
                                    isOnline = true // You can implement actual online checking logic here
                                });
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("✅ STAFF: Found " + participants.Count + " potentially online participants");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ STAFF: Error getting online participants: " + ex.Message);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(participants);
        }

        [WebMethod]
        public static string CheckParticipantStatus(string phoneNumber)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // Generate phone variations to check
                    var phoneVariations = GetPhoneVariations(phoneNumber);

                    string phoneConditions = "";
                    for (int i = 0; i < phoneVariations.Count; i++)
                    {
                        if (i > 0) phoneConditions += " OR ";
                        phoneConditions += "vcb.CustomerPhone = @Phone" + i;
                    }

                    string query = "SELECT TOP 1 " +
                                 "vcb.CustomerName, " +
                                 "vcb.CustomerPhone, " +
                                 "vcb.BookingDate, " +
                                 "vcb.BookingStatus " +
                                 "FROM VideoCallBookings vcb " +
                                 "WHERE (" + phoneConditions + ") " +
                                 "AND vcb.BookingStatus = 'Confirmed' " +
                                 "ORDER BY vcb.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add phone variation parameters
                        for (int i = 0; i < phoneVariations.Count; i++)
                        {
                            cmd.Parameters.AddWithValue("@Phone" + i, phoneVariations[i]);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime bookingDate = Convert.ToDateTime(reader["BookingDate"]);
                                bool isOnline = DateTime.Now.Subtract(bookingDate).TotalMinutes <= 30; // Consider online if booked within 30 minutes

                                var result = new
                                {
                                    success = true,
                                    isOnline = isOnline,
                                    lastSeen = bookingDate.ToString("dd/MM/yyyy HH:mm"),
                                    message = isOnline ? "Participant is available" : "Participant may not be online"
                                };

                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                return serializer.Serialize(result);
                            }
                            else
                            {
                                var result = new
                                {
                                    success = false,
                                    isOnline = false,
                                    lastSeen = "",
                                    message = "Participant not found in registered list"
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
                System.Diagnostics.Debug.WriteLine("❌ STAFF: Error checking participant status: " + ex.Message);
                var result = new
                {
                    success = false,
                    isOnline = false,
                    lastSeen = "",
                    message = "Error checking participant status"
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        [WebMethod]
        public static string UpdateParticipantConnectionStatus(string phoneNumber, string status)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    var phoneVariations = GetPhoneVariations(phoneNumber);

                    string phoneConditions = "";
                    for (int i = 0; i < phoneVariations.Count; i++)
                    {
                        if (i > 0) phoneConditions += " OR ";
                        phoneConditions += "CustomerPhone = @Phone" + i;
                    }

                    string updateQuery = "UPDATE VideoCallBookings " +
                                       "SET BookingStatus = @Status " +
                                       "WHERE (" + phoneConditions + ") " +
                                       "AND BookingStatus = 'Confirmed'";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);

                        for (int i = 0; i < phoneVariations.Count; i++)
                        {
                            cmd.Parameters.AddWithValue("@Phone" + i, phoneVariations[i]);
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();

                        var result = new
                        {
                            success = rowsAffected > 0,
                            message = rowsAffected > 0 ? "Status updated successfully" : "No records updated"
                        };

                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        return serializer.Serialize(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ STAFF: Error updating participant status: " + ex.Message);
                var result = new
                {
                    success = false,
                    message = "Error updating participant status"
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        [WebMethod]
        public static string NotifyParticipants(int sessionId, string participantIds, string message)
        {
            try
            {
                var idList = participantIds.Split(',').Where(id => !string.IsNullOrEmpty(id)).Select(int.Parse).ToList();
                int notifiedCount = 0;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    foreach (int userId in idList)
                    {
                        // Update booking status to indicate notification sent
                        string updateQuery = @"
                            UPDATE VideoCallBookings 
                            SET BookingStatus = 'Expert Ready' 
                            WHERE SessionId = @SessionId AND UserId = @UserId 
                            AND BookingStatus = 'Confirmed'";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@SessionId", sessionId);
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                notifiedCount++;
                            }
                        }
                    }
                }

                var result = new
                {
                    success = true,
                    notifiedCount = notifiedCount,
                    message = $"Notified {notifiedCount} participants that expert is ready"
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    success = false,
                    notifiedCount = 0,
                    message = "Error notifying participants: " + ex.Message
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        // Generate all possible phone number variations
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
                string withSG = "+65" + cleanPhone;
                string withSGNoPlus = "65" + cleanPhone;

                if (!variations.Contains(withSG))
                    variations.Add(withSG);
                if (!variations.Contains(withSGNoPlus))
                    variations.Add(withSGNoPlus);
            }

            // If it's 10 digits starting with 65, create variations
            if (cleanPhone.Length == 10 && cleanPhone.StartsWith("65"))
            {
                string localOnly = cleanPhone.Substring(2); // Remove 65 prefix
                string withPlus = "+" + cleanPhone;

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
            // Clean up any session data if needed
            base.OnUnload(e);
        }
    }
}