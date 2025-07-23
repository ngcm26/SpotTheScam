using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SpotTheScam.User
{
    public partial class LinkChecker : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblResult.Text = string.Empty;
            }
        }

        protected async void btnCheckLink_Click(object sender, EventArgs e)
        {
            string url = txtUrlInput.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                lblResult.Text = "⚠️ Please enter a URL to check.";
                return;
            }

            string resultMsg = await CheckUrlWithGoogleSafeBrowsing(url);
            lblResult.Text = resultMsg;

            // Determine if the result is safe or unsafe
            string script = "document.getElementById('resultSection').classList.remove('result-success','result-danger','hidden');document.querySelector('.checker-main-bg').classList.add('expanded');";
            if (resultMsg.Contains("Unsafe link detected!"))
            {
                script += "document.getElementById('resultSection').classList.add('result-danger');";
            }
            else if (resultMsg.Contains("not flagged as malicious"))
            {
                script += "document.getElementById('resultSection').classList.add('result-success');";
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultSection", script, true);
        }

        private async Task<string> CheckUrlWithGoogleSafeBrowsing(string urlToCheck)
        {
            string API_KEY = "AIzaSyDUlP9Zy3yXVaHIPfunCqevIxnmj7URkUo"; // TODO: Replace with your actual API key
            string apiUrl = $"https://safebrowsing.googleapis.com/v4/threatMatches:find?key={API_KEY}";

            var payload = new
            {
                client = new { clientId = "SpotTheScamApp", clientVersion = "1.0" },
                threatInfo = new
                {
                    threatTypes = new[] { "MALWARE", "SOCIAL_ENGINEERING" },
                    platformTypes = new[] { "ANY_PLATFORM" },
                    threatEntryTypes = new[] { "URL" },
                    threatEntries = new[] { new { url = urlToCheck } }
                }
            };

            using (var client = new HttpClient())
            {
                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                try
                {
                    var response = await client.PostAsync(apiUrl, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(string.IsNullOrWhiteSpace(responseString) ? "{}" : responseString);
                    if (json["matches"] != null)
                    {
                        // Explain why the link is unsafe
                        var reasons = new StringBuilder();
                        foreach (var match in json["matches"])
                        {
                            string threatType = match["threatType"]?.ToString();
                            if (threatType == "MALWARE")
                                reasons.Append("<b>Malware:</b> This site may try to install harmful software on your device.<br>");
                            else if (threatType == "SOCIAL_ENGINEERING")
                                reasons.Append("<b>Phishing:</b> This site may try to trick you into giving away personal information.<br>");
                            else
                                reasons.Append($"<b>{threatType}:</b> This site is flagged as unsafe.<br>");
                        }
                        // Set result text only (no box)
                        return $"⚠️ Unsafe link detected!<br>{reasons}Please avoid clicking this link and do not enter any personal information.";
                    }
                    else
                    {
                        return "✅ This link is not flagged as malicious.<br>Remember: Always be careful when clicking on links, especially from unknown sources. Stay safe online!";
                    }
                }
                catch (Exception ex)
                {
                    return $"❌ Error checking link: {ex.Message}";
                }
            }
        }
    }
}