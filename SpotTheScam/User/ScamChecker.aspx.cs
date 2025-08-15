using System;
using System.Threading.Tasks;
using OpenAI.Chat;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace SpotTheScam.User
{
    public partial class ScamChecker : System.Web.UI.Page
    {
        // Read OpenAI API key from config or environment (do not hardcode)
        private static readonly string apiKey =
            ConfigurationManager.AppSettings["OpenAIApiKey"]
            ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        #region Default Response Templates
        private static readonly string DEFAULT_NO_SCAM_RESPONSE = @"1. Yes/No: Is it a scam?

No

2. Signs to look out for
- Content appears to be from a legitimate source
- No suspicious links or requests for sensitive information
- No urgent threats or poor grammar detected

3. Why it might be dangerous
- No immediate dangers detected in this content
- Always verify message sources independently
- Be cautious of any follow-up communications

4. What to do next
- No action required for this content
- Continue to be vigilant with future messages
- Report suspicious content to authorities if needed";

        private static readonly string DEFAULT_YES_SCAM_RESPONSE = @"1. Yes/No: Is it a scam?

Yes

2. Signs to look out for
- Suspicious sender or contact information
- Unfamiliar or shortened links
- Requests for sensitive information

3. Why it might be dangerous
- May steal your personal or financial information
- Could install malware on your device
- Might trick you into sending money

4. What to do next
- Do not click any links or reply
- Report the message to your bank or authorities
- Block the sender and delete the message";
        #endregion

        // JSON result shape for consistent rendering + lightweight analytics
        private static readonly string RESULT_JSON_INSTRUCTIONS = "Return ONLY a compact JSON object in this exact shape (no backticks, no extra text):\n{" +
            "\n  \"is_scam\": \"Yes|No\"," +
            "\n  \"confidence\": \"Low|Medium|High\"," +
            "\n  \"signs\": [\"bullet 1\", \"bullet 2\", \"bullet 3\"]," +
            "\n  \"dangers\": [\"bullet 1\", \"bullet 2\", \"bullet 3\"]," +
            "\n  \"next_steps\": [\"bullet 1\", \"bullet 2\", \"bullet 3\"]," +
            "\n  \"reader_note\": \"one short reassuring sentence for elderly users\"," +
            "\n  \"scam_type\": \"delivery|job|bank|impersonation|other\"," +
            "\n  \"channel\": \"sms|email|whatsapp|other\"\n}" +
            "\nRules: use simple language, keep bullets short (max ~12 words), include at most 3 bullets per list, NEVER include any text outside the JSON, and choose the closest scam_type and channel from the allowed values.";

        #region Page Load
        /// <summary>
        /// Handles initial page load.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize the page if needed
                lblResult.Text = string.Empty;
            }
            // Ensure file uploads are processed correctly
            if (this.Form != null)
            {
                this.Form.Enctype = "multipart/form-data";
            }
        }
        #endregion

        #region Main Scam Check Handler
        /// <summary>
        /// Handles scam checking for both manual text input and uploaded images using OpenAI API.
        /// </summary>
        protected async void btnCheckScam_Click(object sender, EventArgs e)
        {
            string userInput = this.txtUserInput.Text.Trim();
            bool hasText = !string.IsNullOrWhiteSpace(userInput);
            bool hasImage = fileScreenshot != null && fileScreenshot.HasFile;

            // Enforce mutual exclusivity: exactly one input must be provided
            if ((hasText && hasImage) || (!hasText && !hasImage))
            {
                string msg = (hasText && hasImage)
                    ? "Only one type of media can be analyzed at a time. Please clear either the text or the photo."
                    : "Please either paste a message OR upload a photo to analyze.";

                string jsMsg = System.Web.HttpUtility.JavaScriptStringEncode(msg);
                System.Web.UI.ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "ShowCheckerError",
                    "(function(){var err=document.getElementById('checkerErrorMsg'); if(err){ err.textContent='" + jsMsg + "'; err.style.display='block'; } var box=document.getElementById('resultSection'); if(box){ box.classList.add('hidden'); } var bg=document.querySelector('.checker-main-bg'); if(bg){ bg.classList.remove('expanded'); } })();",
                    true
                );
                return;
            }

            // Clear any existing inline error when starting a valid analysis
            System.Web.UI.ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "HideCheckerError",
                "(function(){var err=document.getElementById('checkerErrorMsg'); if(err){ err.style.display='none'; err.textContent=''; } })();",
                true
            );

            if (hasImage)
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    lblResult.Text = "⚠️ AI analysis is unavailable right now. Please try again later.";
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultNoKeyImg", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');", true);
                    return;
                }
                await AnalyzeImageWithOpenAI();
                return;
            }
            if (hasText)
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    lblResult.Text = "⚠️ AI analysis is unavailable right now. Please try again later.";
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultNoKeyTxt", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');", true);
                    return;
                }
                await AnalyzeTextWithOpenAI(userInput);
                return;
            }
        }
        #endregion

        #region Image Analysis Handler
        /// <summary>
        /// Handles scam checking for uploaded images using OpenAI Vision API.
        /// </summary>
        private async Task AnalyzeImageWithOpenAI()
        {
            try
            {
                string fileExt = System.IO.Path.GetExtension(fileScreenshot.FileName).ToLower();
                if (fileExt != ".jpg" && fileExt != ".jpeg" && fileExt != ".png")
                {
                    lblResult.Text = "⚠️ Only image files (.jpg, .jpeg, .png) are allowed.";
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultOnErrorExt", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');setResultBoxStyle();", true);
                    return;
                }

                // Limit file size to 5MB for safety
                if (fileScreenshot.PostedFile.ContentLength > 5 * 1024 * 1024)
                {
                    lblResult.Text = "⚠️ Image file is too large. Please upload an image smaller than 5MB.";
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultOnErrorSize", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');setResultBoxStyle();", true);
                    return;
                }

                byte[] imageBytes = fileScreenshot.FileBytes;
                string base64Image = Convert.ToBase64String(imageBytes);

                string prompt = @"You are helping an elderly user understand if an image (screenshot/photo) shows a scam.
Analyze the image content only (sender, wording, URLs, amounts, attachments, QR codes). Do NOT perform face recognition.

Focus on classic red flags: unknown sender, suspicious links/QRs, requests for OTP/password, urgent threats, payment requests, prize claims, account lock warnings, poor grammar, mismatched numbers/addresses, lookalike brands.

" + RESULT_JSON_INSTRUCTIONS;

                lblResult.Text = "🔄 Sending image to AI service...";
                btnCheckScam.Enabled = false;
                btnCheckScam.Text = "🔍 Analyzing...";

                // Hide result section initially
                System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "HideResultSection", "document.getElementById('resultSection').classList.add('hidden');document.querySelector('.checker-main-bg').classList.remove('expanded');", true);

                string apiUrl = "https://api.openai.com/v1/chat/completions";
                string imageType = fileExt.Replace(".", "");

                var payload = new
                {
                    model = "gpt-4o",
                    messages = new object[]
                    {
                        new { role = "system", content = "You are a cybersecurity expert specializing in scam detection. Provide clear, educational analysis of potential scams." },
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new { type = "text", text = prompt },
                                new { type = "image_url", image_url = new { url = $"data:image/{imageType};base64,{base64Image}" } }
                            }
                        }
                    },
                    max_tokens = 1000
                };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(apiUrl, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var json = JObject.Parse(responseString);
                        var aiContent = json["choices"]?[0]?["message"]?["content"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(aiContent))
                        {
                            var html = TryRenderJsonOrFallback(aiContent);
                            lblResult.Text = html;
                            // Try to log scan event (image path)
                            try { InferAndLogFromJson(aiContent, defaultChannel: "sms"); } catch { /* non-blocking */ }
                            
                            // Show result section and apply styling
                            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultSection", 
                                "document.getElementById('resultSection').classList.remove('hidden');" +
                                "document.querySelector('.checker-main-bg').classList.add('expanded');" +
                                "setResultBoxStyle();", true);
                        }
                        else
                        {
                            lblResult.Text = "❌ No response from AI. Please try another image.";
                            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultNoResponse", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');setResultBoxStyle();", true);
                        }
                    }
                    else
                    {
                        string errorMsg = "❌ API Error: ";
                        try
                        {
                            var errorJson = JObject.Parse(responseString);
                            errorMsg += errorJson["error"]?["message"]?.ToString() ?? responseString;
                        }
                        catch { errorMsg += responseString; }
                        lblResult.Text = errorMsg;
                        System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultApiError", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');setResultBoxStyle();", true);
                    }
                }
            }
            catch (System.Web.HttpException hex) when (hex.Message.Contains("Maximum request length exceeded"))
            {
                lblResult.Text = "<div class='result-danger-box'>❌ The uploaded image is too large for scanning. Please upload an image smaller than 5MB.</div>";
                System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultHttpEx", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');", true);
            }
            catch (Exception ex)
            {
                lblResult.Text = $"❌ Error: {ex.Message}";
                System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultGenEx", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');", true);
            }
            finally
            {
                btnCheckScam.Enabled = true;
                btnCheckScam.Text = "Analyze";
            }
        }
        #endregion

        #region OpenAI Text Analysis
        /// <summary>
        /// Calls OpenAI Chat API to analyze a text message for scam detection.
        /// </summary>
        private async Task AnalyzeTextWithOpenAI(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                this.lblResult.Text = "⚠️ Please enter a message to check.";
                System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultEmptyText", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');", true);
                return;
            }

            this.btnCheckScam.Enabled = false;
            this.btnCheckScam.Text = "🔍 Analyzing...";

            // At the start of analysis, hide result and reset background
            System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "HideResultSectionText", "document.getElementById('resultSection').classList.add('hidden');document.querySelector('.checker-main-bg').classList.remove('expanded');", true);

            string prompt = @"You are helping an elderly user decide if a text message (SMS/WhatsApp/email) is a scam.
Analyze ONLY the provided text. Be concrete, brief, and friendly.
Focus on classic red flags: unknown sender, suspicious links, requests for OTP/password, urgent threats, payment requests, prize claims, account lock warnings, poor grammar, mismatched numbers/addresses, lookalike brands.

" + RESULT_JSON_INSTRUCTIONS + @"

Here is the message to check:
" + userInput;

            try
            {
                var chat = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);
                var messages = new ChatMessage[]
                {
                    new SystemChatMessage("You are a cybersecurity expert specializing in scam detection. Provide clear, educational analysis of potential scams."),
                    new UserChatMessage(prompt)
                };

                this.lblResult.Text = "🔄 Connecting to AI service...";

                var completion = await chat.CompleteChatAsync(messages);

                if (completion != null && completion.Value != null && completion.Value.Content.Count > 0)
                {
                    string analysis = completion.Value.Content[0].Text.Trim();
                    string html = TryRenderJsonOrFallback(analysis);
                    this.lblResult.Text = html;
                    // Try to log scan event (text path)
                    try { InferAndLogFromJson(analysis, defaultChannel: GuessChannelFromText(userInput)); } catch { /* non-blocking */ }
                    
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultSectionText", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');setResultBoxStyle();", true);
                }
                else
                {
                    this.lblResult.Text = "❌ Unable to analyze the message. Please try again.";
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultSectionTextFail", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');", true);
                }
            }
            catch (Exception apiEx)
            {
                this.lblResult.Text = $"❌ API Error: {apiEx.Message}. Please check your internet connection and try again.";
                System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowResultSectionTextApiErr", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');", true);
            }
            finally
            {
                this.btnCheckScam.Enabled = true;
                this.btnCheckScam.Text = "Analyze";
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Processes AI response and handles edge cases with consistent default responses
        /// </summary>
        private string ProcessAIResponse(string aiResponse)
        {
            if (string.IsNullOrWhiteSpace(aiResponse))
                return DEFAULT_NO_SCAM_RESPONSE;

            string lower = aiResponse.ToLower();
            
            // Handle special cases where AI can't analyze the content
            if (lower.Contains("can't help with identifying people") || 
                lower.Contains("cannot help with identifying people") || 
                lower.Contains("cannot help with authenticity") || 
                lower.Contains("i'm sorry, i can't help"))
            {
                return aiResponse; // Return as-is for these special cases
            }
            
            // If the AI says the content doesn't contain a message/email/SMS
            if (lower.Contains("does not appear to contain a message") || 
                lower.Contains("does not appear to be an email") || 
                lower.Contains("does not appear to be a message") || 
                lower.Contains("does not appear to be an sms") || 
                lower.Contains("does not appear to contain text"))
            {
                return DEFAULT_NO_SCAM_RESPONSE;
            }
            
            // If the AI's answer is ambiguous or does not contain 'yes' or 'no'
            if (!(lower.Contains("yes") || lower.Contains("no")))
            {
                return DEFAULT_NO_SCAM_RESPONSE;
            }
            
            // If the AI's answer is ambiguous but contains 'yes' (and not 'no')
            if (lower.Contains("yes") && !lower.Contains("no"))
            {
                return DEFAULT_YES_SCAM_RESPONSE;
            }
            
            // Return the original AI response if it's properly formatted
            return aiResponse;
        }

        private string ConvertMarkdownBoldToHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            // Remove all asterisks
            string noAsterisks = input.Replace("*", "");
            // Convert markdown bold (**) to <b>
            // This regex replaces **text** with <b>text</b>
            return System.Text.RegularExpressions.Regex.Replace(noAsterisks, @"\*\*(.*?)\*\*", "<b>$1</b>");
        }

        // Add paragraph breaks between numbered points for standard format
        private string AddParagraphBreaksToNumberedList(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            // Add <br><br> before each number (except the first)
            return System.Text.RegularExpressions.Regex.Replace(input, @"(\d+\.)", m => (m.Index > 0 ? "<br><br>" : "") + m.Value);
        }

        // Format result to HTML with bold section headers and bullet lists
        private string FormatScamResultToHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            // Ensure section 1 is always formatted as:
            // 1. Yes/No: Is it a scam?\n- Yes/No
            string formatted = input;
            
            // If section 1 is just '1. Yes' or '1. No', convert to the new format
            formatted = System.Text.RegularExpressions.Regex.Replace(formatted, @"1\.\s*(yes|no)\b", m => "1. Yes/No: Is it a scam?\n- " + m.Groups[1].Value, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Bold numbered section headers (e.g., 1. Yes/No: Is it a scam?)
            formatted = System.Text.RegularExpressions.Regex.Replace(formatted, @"(^|<br\s*/?>|\n)(\d+\.\s.*)", "$1<b>$2</b>");
            
            // Convert dash lists to <ul><li>...</li></ul>
            // Step 1: Convert - lines to <li>
            formatted = System.Text.RegularExpressions.Regex.Replace(formatted, @"(?:^|\n|<br\s*/?>)-\s?(.*)", "<li>$1</li>");
            
            // Step 2: Wrap consecutive <li> in <ul>
            formatted = System.Text.RegularExpressions.Regex.Replace(formatted, @"((<li>.*?</li>\s*)+)", m => $"<ul>{m.Value}</ul>");
            
            // Replace newlines with <br>
            formatted = formatted.Replace("\n", "<br/>");
            
            // Remove redundant bullet point '- Yes/No: Is it a scam?'
            formatted = System.Text.RegularExpressions.Regex.Replace(formatted, @"(<li>)Yes/No: Is it a scam\?(</li>)", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            return formatted;
        }

        // Try to parse the model's output as our JSON contract and render a consistent box.
        // If parsing fails, fallback to the legacy formatter.
        private string TryRenderJsonOrFallback(string content)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(content);
                if (obj == null || obj.is_scam == null) return FormatScamResultToHtml(ProcessAIResponse(content));

                string yesNo = (string)obj.is_scam;
                string confidence = (string)(obj.confidence ?? "");
                IEnumerable<string> signs = ((obj.signs ?? new string[0]) as IEnumerable<object>)?.Select(o => o.ToString()) ?? new string[0];
                IEnumerable<string> dangers = ((obj.dangers ?? new string[0]) as IEnumerable<object>)?.Select(o => o.ToString()) ?? new string[0];
                IEnumerable<string> next = ((obj.next_steps ?? new string[0]) as IEnumerable<object>)?.Select(o => o.ToString()) ?? new string[0];
                string note = (string)(obj.reader_note ?? "");

                var sb = new StringBuilder();
                sb.Append("<b>1. Yes/No: Is it a scam?</b><ul><li>").Append(yesNo).Append(" (Confidence: ").Append(confidence).Append(")</li></ul>");
                sb.Append("<b>2. Signs to look out for</b>");
                sb.Append(RenderList(signs));
                sb.Append("<b>3. Why it might be dangerous</b>");
                sb.Append(RenderList(dangers));
                sb.Append("<b>4. What to do next</b>");
                sb.Append(RenderList(next));
                if (!string.IsNullOrWhiteSpace(note))
                {
                    sb.Append("<br><i>").Append(System.Web.HttpUtility.HtmlEncode(note)).Append("</i>");
                }
                return sb.ToString();
            }
            catch
            {
                return FormatScamResultToHtml(ProcessAIResponse(content));
            }
        }

        private string RenderList(IEnumerable<string> items)
        {
            var arr = (items ?? new string[0]).Take(3).ToArray();
            if (arr.Length == 0) return "<ul><li>None</li></ul>";
            var sb = new StringBuilder("<ul>");
            foreach (var it in arr)
            {
                sb.Append("<li>").Append(System.Web.HttpUtility.HtmlEncode(it)).Append("</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        #endregion

        private string GuessChannelFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "sms";
            string t = text.ToLowerInvariant();
            if (t.Contains("dear") || t.Contains("regards") || t.Contains("subject:") || t.Contains("@")) return "email";
            if (t.Contains("whatsapp") || t.Contains("wa.me") || t.Contains("wtsapp")) return "whatsapp";
            return "sms";
        }

        #region Trend Radar Logging
        private void InferAndLogFromJson(string aiContent, string defaultChannel)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(aiContent);
                string scamType = (string)(obj?.scam_type ?? "other");
                string channel = (string)(obj?.channel ?? defaultChannel ?? "other");
                scamType = NormalizeCategory(scamType, new[] { "delivery", "job", "bank", "impersonation", "other" }, "other");
                channel = NormalizeCategory(channel, new[] { "sms", "email", "whatsapp", "other" }, defaultChannel ?? "other");
                LogScanEvent(scamType, channel);
            }
            catch
            {
                // if parsing fails, log a generic bucket
                LogScanEvent("other", defaultChannel ?? "other");
            }
        }

        private string NormalizeCategory(string input, IEnumerable<string> allowed, string fallback)
        {
            if (string.IsNullOrWhiteSpace(input)) return fallback;
            string x = input.Trim().ToLowerInvariant();
            foreach (var a in allowed)
            {
                if (x.Contains(a)) return a;
            }
            return fallback;
        }

        private void LogScanEvent(string scamType, string channel)
        {
            try
            {
                string cs = WebConfigurationManager.ConnectionStrings["SpotTheScamConnectionString"].ConnectionString;
                using (var conn = new SqlConnection(cs))
                using (var cmd = new SqlCommand("INSERT INTO ScanEvents (user_id, session_id, scam_type, channel) VALUES (@uid,@sid,@type,@channel)", conn))
                {
                    object uid = Session["UserId"] ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@sid", Session.SessionID ?? string.Empty);
                    cmd.Parameters.AddWithValue("@type", (object)scamType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@channel", (object)channel ?? (object)DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { /* non-blocking */ }
        }
        #endregion
    }
}