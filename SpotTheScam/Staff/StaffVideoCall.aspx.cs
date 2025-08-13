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
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // FIXED: Get the actual registration count first to avoid confusion
                int actualRegistrationCount = GetActualRegistrationCount(conn, sessionId);

                System.Diagnostics.Debug.WriteLine($"=== LoadRegisteredParticipants for session {sessionId} ===");
                System.Diagnostics.Debug.WriteLine($"Actual registration count: {actualRegistrationCount}");

                // FIXED: Only create samples if there are truly no registrations AND it's a valid session
                if (actualRegistrationCount == 0)
                {
                    // Check if the session exists and is valid
                    if (IsValidSession(conn, sessionId))
                    {
                        System.Diagnostics.Debug.WriteLine("No registrations found for valid session. Creating sample data for testing...");
                        CreateSampleParticipants(conn, sessionId);
                        actualRegistrationCount = GetActualRegistrationCount(conn, sessionId);
                        System.Diagnostics.Debug.WriteLine($"After creating samples: {actualRegistrationCount} participants");
                    }
                }

                // FIXED: Query with better logic to avoid duplicates
                string query = @"
                    WITH ParticipantData AS (
                        -- Get from VideoCallBookings first
                        SELECT 
                            vcb.UserId,
                            vcb.CustomerName,
                            vcb.CustomerPhone,
                            vcb.CustomerEmail,
                            vcb.BookingDate,
                            vcb.ScamConcerns,
                            vcb.PointsUsed,
                            vcb.BookingStatus,
                            'VideoCall' as SourceTable,
                            ROW_NUMBER() OVER (PARTITION BY vcb.UserId ORDER BY vcb.BookingDate DESC) as rn
                        FROM VideoCallBookings vcb
                        WHERE vcb.SessionId = @SessionId 
                        AND vcb.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected', 'In Call')
                        
                        UNION ALL
                        
                        -- Get from WebinarRegistrations only if no VideoCallBookings exist for that user
                        SELECT 
                            wr.UserId,
                            wr.FirstName + ' ' + wr.LastName as CustomerName,
                            wr.Phone as CustomerPhone,
                            wr.Email as CustomerEmail,
                            wr.RegistrationDate as BookingDate,
                            wr.SecurityConcerns as ScamConcerns,
                            wr.PointsUsed,
                            'Confirmed' as BookingStatus,
                            'Webinar' as SourceTable,
                            ROW_NUMBER() OVER (PARTITION BY wr.UserId ORDER BY wr.RegistrationDate DESC) as rn
                        FROM WebinarRegistrations wr
                        WHERE wr.SessionId = @SessionId 
                        AND wr.IsActive = 1
                        AND NOT EXISTS (
                            SELECT 1 FROM VideoCallBookings vcb2 
                            WHERE vcb2.SessionId = @SessionId 
                            AND vcb2.UserId = wr.UserId
                            AND vcb2.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected', 'In Call')
                        )
                    )
                    SELECT 
                        UserId,
                        COALESCE(NULLIF(CustomerName, ''), 'Participant ' + CAST(UserId AS VARCHAR)) as CustomerName,
                        COALESCE(NULLIF(CustomerPhone, ''), '12345678') as CustomerPhone,
                        COALESCE(NULLIF(CustomerEmail, ''), 'participant' + CAST(UserId AS VARCHAR) + '@example.com') as CustomerEmail,
                        BookingDate,
                        COALESCE(NULLIF(ScamConcerns, ''), 'General consultation') as ScamConcerns,
                        PointsUsed,
                        BookingStatus,
                        SourceTable
                    FROM ParticipantData 
                    WHERE rn = 1  -- Only get the latest record for each user
                    ORDER BY BookingDate ASC";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@SessionId", sessionId);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    System.Diagnostics.Debug.WriteLine($"Query returned {dt.Rows.Count} participant records");

                    // FIXED: Set the count immediately and reliably
                    int finalParticipantCount = dt.Rows.Count;

                    lblTotalParticipants.Text = finalParticipantCount.ToString();
                    lblRegisteredCount.Text = finalParticipantCount.ToString();

                    System.Diagnostics.Debug.WriteLine($"Set label counts to: {finalParticipantCount}");

                    if (finalParticipantCount > 0)
                    {
                        // Debug: Print participant data
                        foreach (DataRow row in dt.Rows)
                        {
                            System.Diagnostics.Debug.WriteLine($"Participant: {row["CustomerName"]}, Phone: {row["CustomerPhone"]}, Email: {row["CustomerEmail"]}, Source: {row["SourceTable"]}");
                        }

                        rptRegisteredParticipants.DataSource = dt;
                        rptRegisteredParticipants.DataBind();
                        pnlNoRegistered.Visible = false;

                        System.Diagnostics.Debug.WriteLine($"✅ Participants loaded and bound to repeater. Count: {finalParticipantCount}");

                        // FIXED: Improved JavaScript initialization
                        string initScript = GenerateInitializationScript(dt, sessionId, finalParticipantCount);
                        ClientScript.RegisterStartupScript(this.GetType(), "InitializeParticipants", initScript, true);
                    }
                    else
                    {
                        rptRegisteredParticipants.DataSource = null;
                        rptRegisteredParticipants.DataBind();
                        pnlNoRegistered.Visible = true;

                        System.Diagnostics.Debug.WriteLine($"⚠️ No registered participants found for session {sessionId}");
                    }
                }
            }
        }

        // FIXED: Helper method to get actual registration count
        private int GetActualRegistrationCount(SqlConnection conn, int sessionId)
        {
            string countQuery = @"
                SELECT COUNT(DISTINCT UserId) as TotalCount
                FROM (
                    SELECT UserId FROM VideoCallBookings 
                    WHERE SessionId = @SessionId 
                    AND BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected', 'In Call')
                    
                    UNION
                    
                    SELECT UserId FROM WebinarRegistrations 
                    WHERE SessionId = @SessionId 
                    AND IsActive = 1
                    AND NOT EXISTS (
                        SELECT 1 FROM VideoCallBookings vcb 
                        WHERE vcb.SessionId = @SessionId 
                        AND vcb.UserId = WebinarRegistrations.UserId
                        AND vcb.BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected', 'In Call')
                    )
                ) AS CombinedRegistrations";

            using (SqlCommand cmd = new SqlCommand(countQuery, conn))
            {
                cmd.Parameters.AddWithValue("@SessionId", sessionId);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        // FIXED: Helper method to check if session exists
        private bool IsValidSession(SqlConnection conn, int sessionId)
        {
            string sessionQuery = "SELECT COUNT(*) FROM ExpertSessions WHERE Id = @SessionId AND Status = 'Available'";
            using (SqlCommand cmd = new SqlCommand(sessionQuery, conn))
            {
                cmd.Parameters.AddWithValue("@SessionId", sessionId);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // FIXED: Improved sample participant creation
        private void CreateSampleParticipants(SqlConnection conn, int sessionId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Creating sample participants for testing...");

                // Check if samples already exist to avoid duplicates
                string checkQuery = "SELECT COUNT(*) FROM VideoCallBookings WHERE SessionId = @SessionId";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@SessionId", sessionId);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Sample participants already exist, skipping creation");
                        return;
                    }
                }

                string insertQuery = @"
                    INSERT INTO VideoCallBookings (
                        SessionId, UserId, CustomerName, CustomerPhone, CustomerEmail, 
                        BookingDate, BookingStatus, ScamConcerns, PointsUsed
                    ) VALUES (
                        @SessionId, @UserId, @CustomerName, @CustomerPhone, @CustomerEmail,
                        @BookingDate, @BookingStatus, @ScamConcerns, @PointsUsed
                    )";

                // Sample participant 1
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@SessionId", sessionId);
                    cmd.Parameters.AddWithValue("@UserId", 1);
                    cmd.Parameters.AddWithValue("@CustomerName", "John Smith");
                    cmd.Parameters.AddWithValue("@CustomerPhone", "+6512345678");
                    cmd.Parameters.AddWithValue("@CustomerEmail", "john.smith@example.com");
                    cmd.Parameters.AddWithValue("@BookingDate", DateTime.Now.AddMinutes(-30));
                    cmd.Parameters.AddWithValue("@BookingStatus", "Confirmed");
                    cmd.Parameters.AddWithValue("@ScamConcerns", "Banking security");
                    cmd.Parameters.AddWithValue("@PointsUsed", 0);
                    cmd.ExecuteNonQuery();
                }

                System.Diagnostics.Debug.WriteLine("✅ Created 1 sample participant");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error creating sample participants: {ex.Message}");
            }
        }

        // FIXED: Generate proper initialization script
        private string GenerateInitializationScript(DataTable dt, int sessionId, int participantCount)
        {
            var script = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    console.log('🚀 Initializing StaffVideoCall with {participantCount} participants');
                    
                    // FIXED: Force update all counters immediately and consistently
                    var actualCount = {participantCount};
                    updateAllCountersImmediately(actualCount);
                    
                    // Initialize participant data for JavaScript
                    if (typeof initializeParticipantStatuses === 'function') {{
                        var participantData = [];";

            foreach (DataRow row in dt.Rows)
            {
                string phone = row["CustomerPhone"].ToString();
                string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");
                string name = row["CustomerName"].ToString().Replace("'", "\\'");

                script += $@"
                        participantData.push({{
                            phone: '{cleanPhone}',
                            name: '{name}',
                            status: 'waiting',
                            bookingStatus: 'Confirmed'
                        }});";
            }

            script += $@"
                        console.log('Initializing participant statuses with data:', participantData);
                        initializeParticipantStatuses(participantData);
                    }}
                    
                    // Start real-time updates
                    startRealTimeUpdates({sessionId});
                    
                    // FIXED: Multiple updates to ensure consistency
                    setTimeout(function() {{
                        updateAllCountersImmediately(actualCount);
                        if (typeof updateParticipantCounts === 'function') {{
                            updateParticipantCounts();
                        }}
                        if (typeof checkOnlineParticipants === 'function') {{
                            checkOnlineParticipants();
                        }}
                    }}, 500);
                    
                    // Final update after 2 seconds
                    setTimeout(function() {{
                        updateAllCountersImmediately(actualCount);
                    }}, 2000);
                }});
                
                // FIXED: Comprehensive function to update all counters consistently
                function updateAllCountersImmediately(count) {{
                    console.log('📊 FORCING UPDATE of all counters to:', count);
                    
                    // Update all possible counter elements
                    var counterSelectors = [
                        'totalParticipants',
                        '{lblTotalParticipants.ClientID}',
                        '{lblRegisteredCount.ClientID}'
                    ];
                    
                    counterSelectors.forEach(function(id) {{
                        var element = document.getElementById(id);
                        if (element) {{
                            element.textContent = count;
                            element.innerText = count;
                            console.log('✅ Updated', id, 'to:', count);
                        }}
                    }});
                    
                    // Update stat cards by traversing DOM
                    var statNumbers = document.querySelectorAll('.stat-number');
                    statNumbers.forEach(function(el) {{
                        var label = el.parentElement.querySelector('.stat-label');
                        if (label && label.textContent.includes('Total')) {{
                            el.textContent = count;
                            el.innerText = count;
                            console.log('✅ Updated Total Registered stat to:', count);
                        }}
                    }});
                    
                    // Update participant count badges
                    var countBadges = document.querySelectorAll('.participant-count');
                    countBadges.forEach(function(badge) {{
                        var section = badge.closest('.participants-section');
                        if (section) {{
                            var titleElement = section.querySelector('.section-title');
                            if (titleElement && titleElement.textContent.includes('Registered')) {{
                                badge.textContent = count;
                                badge.innerText = count;
                                console.log('✅ Updated Registered participant badge to:', count);
                            }}
                        }}
                    }});
                    
                    console.log('✅ All counters force-updated to:', count);
                }}
                
                // Enhanced real-time updates function
                function startRealTimeUpdates(sessionId) {{
                    console.log('🔄 Starting real-time updates for session:', sessionId);
                    
                    function updateStatus() {{
                        fetch('StaffVideoCall.aspx/GetParticipantUpdates', {{
                            method: 'POST',
                            headers: {{ 'Content-Type': 'application/json' }},
                            body: JSON.stringify({{ sessionId: sessionId }})
                        }})
                        .then(response => response.json())
                        .then(data => {{
                            try {{
                                var result = JSON.parse(data.d);
                                if (result.success && result.participants) {{
                                    updateParticipantStatusUI(result.participants);
                                    updateOnlineCount(result.participants);
                                    
                                    // FIXED: Maintain the total registered count
                                    var actualCount = {participantCount};
                                    updateAllCountersImmediately(actualCount);
                                }}
                            }} catch (e) {{
                                console.log('Status update error:', e);
                            }}
                        }})
                        .catch(error => {{
                            console.log('Failed to update participant statuses:', error);
                        }});
                    }}
                    
                    // Update every 15 seconds (longer interval to reduce server load)
                    setInterval(updateStatus, 15000);
                    
                    // Initial update after 3 seconds
                    setTimeout(updateStatus, 3000);
                }}
                
                // Enhanced participant status UI update
                function updateParticipantStatusUI(participants) {{
                    participants.forEach(function(participant) {{
                        var cleanPhone = participant.phone.replace(/[^0-9]/g, '');
                        var statusElement = document.getElementById('status_' + cleanPhone);
                        var buttonElement = document.getElementById('btn_' + cleanPhone);
                        
                        if (statusElement) {{
                            statusElement.className = 'status-indicator status-' + participant.status;
                        }}
                        
                        if (buttonElement) {{
                            switch(participant.status) {{
                                case 'online':
                                    buttonElement.textContent = '🟢 Connect Now';
                                    buttonElement.disabled = false;
                                    break;
                                case 'connected':
                                    buttonElement.textContent = '📞 In Call';
                                    buttonElement.disabled = true;
                                    break;
                                default:
                                    buttonElement.textContent = '📞 Connect';
                                    buttonElement.disabled = false;
                            }}
                        }}
                    }});
                }}
                
                // Enhanced online count update
                function updateOnlineCount(participants) {{
                    var onlineCount = participants.filter(p => p.status === 'online' || p.status === 'waiting').length;
                    var connectedCount = participants.filter(p => p.status === 'connected').length;
                    
                    // Update online participants counter
                    var onlineElements = document.querySelectorAll('#onlineParticipants, #onlineCount');
                    onlineElements.forEach(function(el) {{
                        el.textContent = onlineCount;
                    }});
                    
                    // Update connected participants counter
                    var connectedElements = document.querySelectorAll('#connectedParticipants');
                    connectedElements.forEach(function(el) {{
                        el.textContent = connectedCount;
                    }});
                    
                    // Update stat cards for online and connected (but NOT total registered)
                    var statNumbers = document.querySelectorAll('.stat-number');
                    statNumbers.forEach(function(el) {{
                        var label = el.parentElement.querySelector('.stat-label');
                        if (label) {{
                            if (label.textContent.includes('Online')) {{
                                el.textContent = onlineCount;
                            }} else if (label.textContent.includes('Video Call')) {{
                                el.textContent = connectedCount;
                            }}
                        }}
                    }});
                    
                    console.log('📊 Updated status counts - Online:', onlineCount, 'Connected:', connectedCount);
                }}";

            return script;
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
                        WITH ParticipantStatus AS (
                            SELECT 
                                UserId,
                                COALESCE(CustomerName, FirstName + ' ' + LastName, 'Participant') as Name,
                                COALESCE(CustomerPhone, Phone, '12345678') as Phone,
                                COALESCE(BookingStatus, 'Confirmed') as BookingStatus,
                                COALESCE(BookingDate, RegistrationDate, GETDATE()) as LastActivity,
                                CASE 
                                    WHEN BookingStatus = 'Connected' OR BookingStatus = 'In Call' THEN 'connected'
                                    WHEN BookingStatus = 'Expert Ready' THEN 'waiting'
                                    WHEN BookingStatus = 'Confirmed' THEN 'online'
                                    ELSE 'offline'
                                END as Status,
                                ROW_NUMBER() OVER (PARTITION BY UserId ORDER BY 
                                    CASE WHEN BookingStatus IS NOT NULL THEN 1 ELSE 2 END,
                                    COALESCE(BookingDate, RegistrationDate) DESC
                                ) as rn
                            FROM (
                                SELECT 
                                    UserId, CustomerName, CustomerPhone, BookingStatus, BookingDate,
                                    NULL as FirstName, NULL as LastName, NULL as Phone, NULL as RegistrationDate
                                FROM VideoCallBookings 
                                WHERE SessionId = @SessionId 
                                AND BookingStatus IN ('Confirmed', 'Expert Ready', 'Connected', 'In Call')
                                
                                UNION ALL
                                
                                SELECT 
                                    UserId, NULL as CustomerName, NULL as CustomerPhone, NULL as BookingStatus, NULL as BookingDate,
                                    FirstName, LastName, Phone, RegistrationDate
                                FROM WebinarRegistrations 
                                WHERE SessionId = @SessionId 
                                AND IsActive = 1
                            ) AS CombinedData
                        )
                        SELECT Name, Phone, Status, BookingStatus, LastActivity
                        FROM ParticipantStatus 
                        WHERE rn = 1
                        ORDER BY LastActivity DESC";

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
                        phoneConditions += $"CustomerPhone = @Phone{i}";
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
        public static string NotifyParticipants(int sessionId, string participantIds, string message)
        {
            try
            {
                var idArray = participantIds.Split(',').Where(id => !string.IsNullOrEmpty(id)).ToArray();
                int notifiedCount = 0;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString))
                {
                    conn.Open();

                    foreach (string phoneId in idArray)
                    {
                        // Update booking status to indicate notification sent
                        string updateQuery = @"
                            UPDATE VideoCallBookings 
                            SET BookingStatus = 'Expert Ready' 
                            WHERE SessionId = @SessionId 
                            AND (CustomerPhone LIKE '%' + @PhoneId + '%' OR UserId = @UserId)";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@SessionId", sessionId);
                            cmd.Parameters.AddWithValue("@PhoneId", phoneId.Replace("+", "").Replace(" ", ""));

                            if (int.TryParse(phoneId, out int userId))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@UserId", DBNull.Value);
                            }

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