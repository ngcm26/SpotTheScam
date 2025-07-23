using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam.Staff
{
    public partial class StaffManageParticipants : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
        private int sessionId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if staff is logged in
            if (Session["StaffName"] == null)
            {
                Response.Redirect("StaffLogin.aspx");
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
                            u.Name as UserName,
                            u.Email,
                            u.PhoneNumber,
                            vcb.BookingDate,
                            vcb.PointsUsed,
                            vcb.BookingStatus
                        FROM VideoCallBookings vcb
                        INNER JOIN Users u ON vcb.UserId = u.Id
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
                int userId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName.ToLower())
                {
                    case "removeparticipant":
                        RemoveParticipant(userId);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error processing command: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"Error in ItemCommand: {ex.Message}");
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
                                SELECT u.Name, vcb.PointsUsed 
                                FROM VideoCallBookings vcb
                                INNER JOIN Users u ON vcb.UserId = u.Id
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

        protected void btnStartSession_Click(object sender, EventArgs e)
        {
            try
            {
                // Redirect to the video call page
                Response.Redirect($"StaffVideoCall.aspx?sessionId={sessionId}");
            }
            catch (Exception ex)
            {
                ShowAlert("Error starting session: " + ex.Message, "error");
            }
        }

        private void ShowAlert(string message, string type)
        {
            AlertPanel.CssClass = type == "success" ? "alert alert-success" : "alert alert-error";
            AlertMessage.Text = message;
            AlertPanel.Visible = true;
        }
    }
}