using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpotTheScam
{
    public partial class StaffExpertWebinar : System.Web.UI.Page
    {
        // Get connection string from web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set minimum date to today
                SessionDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");
                LoadSessions();
            }
        }

        protected void AddTimeSlotButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate that end time is after start time
                TimeSpan startTime = TimeSpan.Parse(StartTime.Text);
                TimeSpan endTime = TimeSpan.Parse(EndTime.Text);

                if (endTime <= startTime)
                {
                    ShowAlert("End time must be after start time", "error");
                    return;
                }

                // Insert new session into database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO ExpertWebinarSessions 
                                   (SessionDate, StartTime, EndTime, Status, ExpertName, CreatedAt) 
                                   VALUES (@SessionDate, @StartTime, @EndTime, @Status, @ExpertName, @CreatedAt)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SessionDate", DateTime.Parse(SessionDate.Text));
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);
                    cmd.Parameters.AddWithValue("@Status", "Available");
                    cmd.Parameters.AddWithValue("@ExpertName", "Scam Prevention Expert");
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Clear form and refresh grid
                ClearForm();
                LoadSessions();
                ShowAlert("Time slot added successfully!", "success");
            }
            catch (Exception ex)
            {
                ShowAlert("Error adding time slot: " + ex.Message, "error");
            }
        }

        protected void SessionsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteSession")
            {
                try
                {
                    int sessionId = Convert.ToInt32(e.CommandArgument);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM ExpertWebinarSessions WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", sessionId);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            LoadSessions();
                            ShowAlert("Time slot deleted successfully", "success");
                        }
                        else
                        {
                            ShowAlert("Session not found", "error");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error deleting time slot: " + ex.Message, "error");
                }
            }
        }

        private void LoadSessions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT Id, SessionDate, StartTime, EndTime, Status, 
                                           CustomerName, CustomerPhone, ScamConcerns, ExpertName
                                    FROM ExpertWebinarSessions 
                                    ORDER BY SessionDate, StartTime";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    SessionsGridView.DataSource = dt;
                    SessionsGridView.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error loading sessions: " + ex.Message, "error");
            }
        }

        private void ClearForm()
        {
            SessionDate.Text = string.Empty;
            StartTime.Text = string.Empty;
            EndTime.Text = string.Empty;
        }

        private void ShowAlert(string message, string type)
        {
            string cssClass = type == "success" ? "alert alert-success" : "alert alert-error";
            AlertPanel.CssClass = cssClass;
            AlertMessage.Text = message;
            AlertPanel.Visible = true;

            // Hide alert after 5 seconds using JavaScript
            string script = @"
                setTimeout(function() {
                    document.getElementById('" + AlertPanel.ClientID + @"').style.display = 'none';
                }, 5000);
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideAlert", script, true);
        }
    }
}