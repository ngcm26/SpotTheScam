using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Script.Serialization;

namespace SpotTheScam.Staff
{
    public partial class StaffVideoCall : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if staff is logged in
                if (Session["StaffName"] == null && Session["StaffUsername"] == null && Session["Username"] == null)
                {
                    Response.Redirect("~/User/UserLogin.aspx");
                    return;
                }

                LoadSessionData();
            }
        }

        private void LoadSessionData()
        {
            // Get session ID from query string or previous page
            string sessionIdStr = Request.QueryString["sessionId"];
            string mode = Request.QueryString["mode"];
            string participants = Request.QueryString["participants"];

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
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Updated query with correct column names and proper name handling
                    string query = @"
                SELECT 
                    vcb.UserId,
                    CASE 
                        WHEN vcb.CustomerName IS NOT NULL AND LEN(TRIM(vcb.CustomerName)) > 0 
                        THEN vcb.CustomerName
                        WHEN vcb.FirstName IS NOT NULL AND vcb.LastName IS NOT NULL 
                        THEN vcb.FirstName + ' ' + vcb.LastName
                        WHEN u.Username IS NOT NULL 
                        THEN u.Username
                        ELSE 'Participant ' + RIGHT('000' + CAST(vcb.UserId as VARCHAR), 3)
                    END as CustomerName,
                    
                    CASE 
                        WHEN vcb.CustomerEmail IS NOT NULL AND LEN(TRIM(vcb.CustomerEmail)) > 0 
                        THEN vcb.CustomerEmail
                        WHEN u.Email IS NOT NULL 
                        THEN u.Email
                        ELSE 'participant' + CAST(vcb.UserId as VARCHAR) + '@example.com'
                    END as CustomerEmail,
                    
                    CASE 
                        WHEN vcb.CustomerPhone IS NOT NULL AND LEN(TRIM(vcb.CustomerPhone)) > 0 
                        THEN vcb.CustomerPhone
                        WHEN u.PhoneNumber IS NOT NULL 
                        THEN u.PhoneNumber
                        ELSE '12345678'
                    END as CustomerPhone,
                    
                    vcb.BookingDate,
                    ISNULL(vcb.PointsUsed, 0) as PointsUsed,
                    vcb.BookingStatus,
                    CASE 
                        WHEN vcb.ScamConcerns IS NOT NULL AND LEN(TRIM(vcb.ScamConcerns)) > 0 
                        THEN vcb.ScamConcerns
                        WHEN vcb.MainSecurityConcerns IS NOT NULL 
                        THEN vcb.MainSecurityConcerns
                        ELSE 'General consultation'
                    END as ScamConcerns,
                    vcb.SessionId
                FROM VideoCallBookings vcb
                LEFT JOIN Users u ON vcb.UserId = u.Id
                WHERE vcb.SessionId = @SessionId 
                AND vcb.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected', 'In Call')
                ORDER BY vcb.BookingDate DESC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@SessionId", sessionId);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        System.Diagnostics.Debug.WriteLine($"=== LoadParticipants Debug ===");
                        System.Diagnostics.Debug.WriteLine($"Session ID: {sessionId}");
                        System.Diagnostics.Debug.WriteLine($"Found {dt.Rows.Count} participants");

                        // Debug: Print participant data
                        foreach (DataRow row in dt.Rows)
                        {
                            System.Diagnostics.Debug.WriteLine($"Participant: {row["CustomerName"]}, Phone: {row["CustomerPhone"]}, Email: {row["CustomerEmail"]}, Status: {row["BookingStatus"]}");
                        }

                        if (dt.Rows.Count > 0)
                        {
                            rptRegisteredParticipants.DataSource = dt;
                            rptRegisteredParticipants.DataBind();
                            pnlNoRegistered.Visible = false;
                            lblTotalParticipants.Text = dt.Rows.Count.ToString();
                            lblRegisteredCount.Text = dt.Rows.Count.ToString();

                            System.Diagnostics.Debug.WriteLine($"✅ Participants loaded and bound to repeater. Count: {dt.Rows.Count}");
                        }
                        else
                        {
                            rptRegisteredParticipants.DataSource = null;
                            rptRegisteredParticipants.DataBind();
                            pnlNoRegistered.Visible = true;
                            lblTotalParticipants.Text = "0";
                            lblRegisteredCount.Text = "0";

                            System.Diagnostics.Debug.WriteLine("⚠️ No participants found - checking if any registrations exist at all");

                            // Debug: Check if there are ANY registrations for this session
                            CheckForAnyRegistrations(conn, sessionId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading participants: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"❌ Error loading participants: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            }
        }

        // Debug method to check for any registrations
        private void CheckForAnyRegistrations(SqlConnection conn, int sessionId)
        {
            try
            {
                // Check for ANY bookings for this session regardless of status
                string debugQuery = @"
            SELECT 
                vcb.UserId, 
                vcb.CustomerName, 
                vcb.CustomerPhone, 
                vcb.CustomerEmail, 
                vcb.BookingStatus,
                vcb.BookingDate
            FROM VideoCallBookings vcb
            WHERE vcb.SessionId = @SessionId
            ORDER BY vcb.BookingDate DESC";

                using (SqlCommand debugCmd = new SqlCommand(debugQuery, conn))
                {
                    debugCmd.Parameters.AddWithValue("@SessionId", sessionId);
                    using (SqlDataReader reader = debugCmd.ExecuteReader())
                    {
                        int allRegistrations = 0;
                        System.Diagnostics.Debug.WriteLine($"=== ALL Registrations for Session {sessionId} ===");

                        while (reader.Read())
                        {
                            allRegistrations++;
                            System.Diagnostics.Debug.WriteLine($"Registration {allRegistrations}:");
                            System.Diagnostics.Debug.WriteLine($"  - UserId: {reader["UserId"]}");
                            System.Diagnostics.Debug.WriteLine($"  - Name: {reader["CustomerName"]}");
                            System.Diagnostics.Debug.WriteLine($"  - Phone: {reader["CustomerPhone"]}");
                            System.Diagnostics.Debug.WriteLine($"  - Email: {reader["CustomerEmail"]}");
                            System.Diagnostics.Debug.WriteLine($"  - Status: {reader["BookingStatus"]}");
                            System.Diagnostics.Debug.WriteLine($"  - Date: {reader["BookingDate"]}");
                        }

                        if (allRegistrations == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("❌ No registrations found at all for this session");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ Found {allRegistrations} total registrations");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in debug check: {ex.Message}");
            }
        }

        // Web method for getting participant updates
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
                            CASE 
                                WHEN vcb.CustomerName IS NOT NULL AND LEN(TRIM(vcb.CustomerName)) > 0 
                                THEN vcb.CustomerName
                                WHEN vcb.FirstName IS NOT NULL AND vcb.LastName IS NOT NULL 
                                THEN vcb.FirstName + ' ' + vcb.LastName
                                WHEN u.Username IS NOT NULL 
                                THEN u.Username
                                ELSE 'Participant ' + RIGHT('000' + CAST(vcb.UserId as VARCHAR), 3)
                            END as Name,
                            
                            CASE 
                                WHEN vcb.CustomerPhone IS NOT NULL AND LEN(TRIM(vcb.CustomerPhone)) > 0 
                                THEN vcb.CustomerPhone
                                WHEN u.PhoneNumber IS NOT NULL 
                                THEN u.PhoneNumber
                                ELSE '12345678'
                            END as Phone,
                            
                            COALESCE(vcb.BookingStatus, 'Confirmed') as BookingStatus,
                            COALESCE(vcb.BookingDate, GETDATE()) as LastActivity,
                            CASE 
                                WHEN vcb.BookingStatus = 'Connected' OR vcb.BookingStatus = 'In Call' THEN 'connected'
                                WHEN vcb.BookingStatus = 'Expert Ready' THEN 'waiting'
                                WHEN vcb.BookingStatus = 'Confirmed' THEN 'online'
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
                                string phone = reader["Phone"]?.ToString() ?? "12345678";
                                string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

                                participants.Add(new
                                {
                                    name = reader["Name"].ToString(),
                                    phone = cleanPhone,
                                    status = reader["Status"].ToString(),
                                    bookingStatus = reader["BookingStatus"].ToString(),
                                    lastActive = reader["LastActivity"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["LastActivity"]).ToString("HH:mm")
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

        // Web method for getting participant real names
        [WebMethod]
        public static string GetParticipantRealName(int sessionId, string phoneNumber)
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
                System.Diagnostics.Debug.WriteLine($"❌ Error getting participant real name: {ex.Message}");

                var result = new
                {
                    success = false,
                    name = "Participant"
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(result);
            }
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = "status-message error";
        }

        protected override void OnUnload(EventArgs e)
        {
            // Clean up any session data if needed
            base.OnUnload(e);
        }
    }
}