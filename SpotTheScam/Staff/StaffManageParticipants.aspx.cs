using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

namespace SpotTheScam.Staff
{
    public partial class StaffManageParticipants : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
        private int sessionId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if staff is logged in - FIXED: Redirect to UserLogin.aspx
            if (Session["StaffName"] == null && Session["StaffUsername"] == null && Session["Username"] == null)
            {
                Response.Redirect("~/User/UserLogin.aspx");
                return;
            }

            // Get session ID from query string
            if (!int.TryParse(Request.QueryString["sessionId"], out sessionId))
            {
                Response.Redirect("StaffManageSessions.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSessionInfo();
                LoadParticipants();
            }
        }

        private void LoadSessionInfo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            SessionTitle, SessionDescription, SessionDate, 
                            CONVERT(varchar, StartTime, 108) as StartTime,
                            CONVERT(varchar, EndTime, 108) as EndTime,
                            ExpertName, SessionTopic
                        FROM ExpertSessions 
                        WHERE Id = @SessionId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string sessionInfo = $@"
                                    <strong>{reader["SessionTitle"]}</strong><br/>
                                    <em>{reader["SessionDescription"]}</em><br/>
                                    📅 <strong>Date:</strong> {Convert.ToDateTime(reader["SessionDate"]):dd/MM/yyyy}<br/>
                                    🕐 <strong>Time:</strong> {reader["StartTime"]} - {reader["EndTime"]}<br/>
                                    👨‍💼 <strong>Expert:</strong> {reader["ExpertName"]}<br/>
                                    📚 <strong>Topic:</strong> {reader["SessionTopic"]}
                                ";

                                lblSessionInfo.Text = sessionInfo;

                                // Generate session link for staff
                                string sessionLink = $"{Request.Url.Scheme}://{Request.Url.Authority}/Staff/StaffVideoCall.aspx?sessionId={sessionId}";
                                lblSessionLink.Text = sessionLink;
                            }
                            else
                            {
                                ShowAlert("Session not found.", "error");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading session information: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error loading session info: {ex.Message}");
            }
        }

        private void LoadParticipants()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            vcb.UserId,
                            COALESCE(u.Name, vcb.CustomerName, 'Participant') as UserName,
                            COALESCE(u.Email, vcb.CustomerEmail, 'No email') as Email,
                            COALESCE(u.PhoneNumber, vcb.CustomerPhone, 'No phone') as PhoneNumber,
                            vcb.BookingDate,
                            vcb.PointsUsed,
                            vcb.BookingStatus,
                            COALESCE(vcb.ScamConcerns, 'General consultation') as ScamConcerns
                        FROM VideoCallBookings vcb
                        LEFT JOIN Users u ON vcb.UserId = u.Id
                        WHERE vcb.SessionId = @SessionId AND vcb.BookingStatus = 'Confirmed'
                        ORDER BY vcb.BookingDate DESC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@SessionId", sessionId);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptParticipants.DataSource = dt;
                            rptParticipants.DataBind();
                            pnlNoParticipants.Visible = false;
                            lblParticipantCount.Text = dt.Rows.Count.ToString();

                            System.Diagnostics.Debug.WriteLine($"Loaded {dt.Rows.Count} participants");
                        }
                        else
                        {
                            rptParticipants.DataSource = null;
                            rptParticipants.DataBind();
                            pnlNoParticipants.Visible = true;
                            lblParticipantCount.Text = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading participants: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error loading participants: {ex.Message}");
            }
        }

        protected void rptParticipants_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                switch (e.CommandName.ToLower())
                {
                    case "removeparticipant":
                        int userId = Convert.ToInt32(e.CommandArgument);
                        RemoveParticipant(userId);
                        break;
                    case "connectparticipant":
                        string[] args = e.CommandArgument.ToString().Split(',');
                        if (args.Length >= 3)
                        {
                            int participantUserId = Convert.ToInt32(args[0]);
                            string phone = args[1];
                            string name = args[2];
                            ConnectToParticipant(participantUserId, phone, name);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error processing command: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error in ItemCommand: {ex.Message}");
            }
        }

        private void ConnectToParticipant(int userId, string phone, string name)
        {
            try
            {
                // Store connection info and redirect to video call page
                Session["ConnectToUserId"] = userId;
                Session["ConnectToPhone"] = phone;
                Session["ConnectToName"] = name;
                Session["CurrentSessionId"] = sessionId;

                // Redirect to video call page with connection parameters
                string videoCallUrl = $"StaffVideoCall.aspx?sessionId={sessionId}&connectTo={userId}&phone={phone}&name={Server.UrlEncode(name)}";
                Response.Redirect(videoCallUrl);
            }
            catch (Exception ex)
            {
                ShowAlert("Error initiating connection: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error connecting to participant: {ex.Message}");
            }
        }

        private void RemoveParticipant(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Get user details for confirmation message
                            string userName = "";
                            int pointsToRefund = 0;

                            string getUserQuery = @"
                                SELECT 
                                    COALESCE(u.Name, vcb.CustomerName, 'Participant') as Name, 
                                    vcb.PointsUsed 
                                FROM VideoCallBookings vcb
                                LEFT JOIN Users u ON vcb.UserId = u.Id
                                WHERE vcb.SessionId = @SessionId AND vcb.UserId = @UserId";

                            using (SqlCommand getUserCmd = new SqlCommand(getUserQuery, conn, transaction))
                            {
                                getUserCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                getUserCmd.Parameters.AddWithValue("@UserId", userId);
                                using (SqlDataReader reader = getUserCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        userName = reader["Name"].ToString();
                                        pointsToRefund = Convert.ToInt32(reader["PointsUsed"] ?? 0);
                                    }
                                }
                            }

                            // Remove registration
                            string deleteQuery = "DELETE FROM VideoCallBookings WHERE SessionId = @SessionId AND UserId = @UserId";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                            {
                                deleteCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                deleteCmd.Parameters.AddWithValue("@UserId", userId);
                                int deleted = deleteCmd.ExecuteNonQuery();

                                if (deleted > 0)
                                {
                                    // Refund points if any were used
                                    if (pointsToRefund > 0)
                                    {
                                        string refundQuery = "UPDATE Users SET Points = Points + @PointsToRefund WHERE Id = @UserId";
                                        using (SqlCommand refundCmd = new SqlCommand(refundQuery, conn, transaction))
                                        {
                                            refundCmd.Parameters.AddWithValue("@PointsToRefund", pointsToRefund);
                                            refundCmd.Parameters.AddWithValue("@UserId", userId);
                                            refundCmd.ExecuteNonQuery();
                                        }
                                    }

                                    // Update current participants count
                                    string updateCountQuery = @"
                                        UPDATE ExpertSessions 
                                        SET CurrentParticipants = (
                                            SELECT COUNT(*) FROM VideoCallBookings 
                                            WHERE SessionId = @SessionId AND BookingStatus = 'Confirmed'
                                        ) 
                                        WHERE Id = @SessionId";

                                    using (SqlCommand updateCmd = new SqlCommand(updateCountQuery, conn, transaction))
                                    {
                                        updateCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                        updateCmd.ExecuteNonQuery();
                                    }

                                    transaction.Commit();

                                    string message = pointsToRefund > 0
                                        ? $"Removed {userName} and refunded {pointsToRefund} points."
                                        : $"Removed {userName} from the session.";

                                    ShowAlert(message, "success");
                                    LoadParticipants();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ShowAlert("Failed to remove participant.", "error");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error removing participant: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error removing participant: {ex.Message}");
            }
        }

        // NEW: Bulk action methods
        protected void btnConnectSelected_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedIds = Request.Form["hdnSelectedParticipants"];
                if (string.IsNullOrEmpty(selectedIds))
                {
                    ShowAlert("Please select participants to connect to.", "error");
                    return;
                }

                var participantIds = selectedIds.Split(',').Where(id => !string.IsNullOrEmpty(id)).ToList();
                if (participantIds.Count == 0)
                {
                    ShowAlert("No participants selected.", "error");
                    return;
                }

                if (participantIds.Count == 1)
                {
                    // Single participant - redirect to one-on-one call
                    int userId = Convert.ToInt32(participantIds[0]);
                    var participantInfo = GetParticipantInfo(userId);
                    if (participantInfo != null)
                    {
                        ConnectToParticipant(userId, participantInfo.Phone, participantInfo.Name);
                    }
                }
                else
                {
                    // Multiple participants - redirect to group call mode
                    Session["SelectedParticipants"] = selectedIds;
                    Session["CurrentSessionId"] = sessionId;
                    Response.Redirect($"StaffVideoCall.aspx?sessionId={sessionId}&mode=group&participants={selectedIds}");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error connecting to selected participants: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error in bulk connect: {ex.Message}");
            }
        }

        protected void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedIds = Request.Form["hdnSelectedParticipants"];
                if (string.IsNullOrEmpty(selectedIds))
                {
                    ShowAlert("Please select participants to remove.", "error");
                    return;
                }

                var participantIds = selectedIds.Split(',').Where(id => !string.IsNullOrEmpty(id)).Select(int.Parse).ToList();
                int removedCount = 0;
                int totalPointsRefunded = 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (int userId in participantIds)
                            {
                                // Get points to refund
                                string getPointsQuery = "SELECT ISNULL(PointsUsed, 0) FROM VideoCallBookings WHERE SessionId = @SessionId AND UserId = @UserId";
                                using (SqlCommand getPointsCmd = new SqlCommand(getPointsQuery, conn, transaction))
                                {
                                    getPointsCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                    getPointsCmd.Parameters.AddWithValue("@UserId", userId);
                                    int pointsToRefund = Convert.ToInt32(getPointsCmd.ExecuteScalar() ?? 0);
                                    totalPointsRefunded += pointsToRefund;
                                }

                                // Remove participant
                                string deleteQuery = "DELETE FROM VideoCallBookings WHERE SessionId = @SessionId AND UserId = @UserId";
                                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                                {
                                    deleteCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                    deleteCmd.Parameters.AddWithValue("@UserId", userId);
                                    if (deleteCmd.ExecuteNonQuery() > 0)
                                    {
                                        removedCount++;
                                    }
                                }

                                // Refund points if any
                                if (totalPointsRefunded > 0)
                                {
                                    string refundQuery = "UPDATE Users SET Points = Points + @PointsToRefund WHERE Id = @UserId";
                                    using (SqlCommand refundCmd = new SqlCommand(refundQuery, conn, transaction))
                                    {
                                        refundCmd.Parameters.AddWithValue("@PointsToRefund", totalPointsRefunded);
                                        refundCmd.Parameters.AddWithValue("@UserId", userId);
                                        refundCmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // Update session participant count
                            string updateCountQuery = @"
                                UPDATE ExpertSessions 
                                SET CurrentParticipants = (
                                    SELECT COUNT(*) FROM VideoCallBookings 
                                    WHERE SessionId = @SessionId AND BookingStatus = 'Confirmed'
                                ) 
                                WHERE Id = @SessionId";

                            using (SqlCommand updateCmd = new SqlCommand(updateCountQuery, conn, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@SessionId", sessionId);
                                updateCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            string message = totalPointsRefunded > 0
                                ? $"Removed {removedCount} participants and refunded {totalPointsRefunded} total points."
                                : $"Removed {removedCount} participants from the session.";

                            ShowAlert(message, "success");
                            LoadParticipants();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error removing selected participants: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error in bulk remove: {ex.Message}");
            }
        }

        protected void btnStartBroadcast_Click(object sender, EventArgs e)
        {
            try
            {
                Session["CurrentSessionId"] = sessionId;
                Response.Redirect($"StaffVideoCall.aspx?sessionId={sessionId}&mode=broadcast");
            }
            catch (Exception ex)
            {
                ShowAlert("Error starting broadcast session: " + ex.Message, "error");
            }
        }

        protected void btnStartGroupCall_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedIds = Request.Form["hdnSelectedParticipants"];
                if (string.IsNullOrEmpty(selectedIds))
                {
                    ShowAlert("Please select participants for the group call.", "error");
                    return;
                }

                Session["SelectedParticipants"] = selectedIds;
                Session["CurrentSessionId"] = sessionId;
                Response.Redirect($"StaffVideoCall.aspx?sessionId={sessionId}&mode=group&participants={selectedIds}");
            }
            catch (Exception ex)
            {
                ShowAlert("Error starting group call: " + ex.Message, "error");
            }
        }

        protected void btnStartOneOnOne_Click(object sender, EventArgs e)
        {
            try
            {
                Session["CurrentSessionId"] = sessionId;
                Response.Redirect($"StaffVideoCall.aspx?sessionId={sessionId}&mode=oneOnOne");
            }
            catch (Exception ex)
            {
                ShowAlert("Error starting one-on-one mode: " + ex.Message, "error");
            }
        }

        // Helper class for participant information
        public class ParticipantInfo
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
        }

        private ParticipantInfo GetParticipantInfo(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            vcb.UserId,
                            COALESCE(u.Name, vcb.CustomerName, 'Participant') as Name,
                            COALESCE(u.PhoneNumber, vcb.CustomerPhone, '') as Phone,
                            COALESCE(u.Email, vcb.CustomerEmail, '') as Email
                        FROM VideoCallBookings vcb
                        LEFT JOIN Users u ON vcb.UserId = u.Id
                        WHERE vcb.SessionId = @SessionId AND vcb.UserId = @UserId AND vcb.BookingStatus = 'Confirmed'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ParticipantInfo
                                {
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    Name = reader["Name"].ToString(),
                                    Phone = reader["Phone"].ToString(),
                                    Email = reader["Email"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting participant info: {ex.Message}");
            }
            return null;
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = type == "success" ? "alert alert-success" : "alert alert-error";
            AlertMessage.Text = message;
            AlertPanel.Visible = true;
        }
    }
}