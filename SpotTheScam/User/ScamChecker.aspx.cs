using System;
using System.Threading.Tasks;
using OpenAI.Chat;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SpotTheScam.User
{
    public partial class ScamChecker : System.Web.UI.Page
    {
        // Store your OpenAI API key here (consider securing it in production)
        private static readonly string apiKey = "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";

        #region Page Load
        /// <summary>
        /// Handles initial page load.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize the page if needed
            }
        }
        #endregion

        #region Image Upload Handler
        /// <summary>
        /// Handles scam checking for uploaded images using OpenAI Vision API.
        /// </summary>
        protected async void btnCheckImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fileScreenshot.HasFile)
                {
                    lblResult.Text = "⚠️ Please upload an image file.";
                    return;
                }

                string fileExt = System.IO.Path.GetExtension(fileScreenshot.FileName).ToLower();
                if (fileExt != ".jpg" && fileExt != ".jpeg" && fileExt != ".png")
                {
                    lblResult.Text = "⚠️ Only image files (.jpg, .jpeg, .png) are allowed.";
                    return;
                }

                // Limit file size to 5MB for safety
                if (fileScreenshot.PostedFile.ContentLength > 5 * 1024 * 1024)
                {
                    lblResult.Text = "⚠️ Image file is too large. Please upload an image smaller than 5MB.";
                    return;
                }

                byte[] imageBytes = fileScreenshot.FileBytes;
                string base64Image = Convert.ToBase64String(imageBytes);

                string prompt = @"You are a scam detection assistant. Please analyze this image and answer in this exact format:

1. Yes/No: Is it a scam? (Only say 'Yes' if there are clear scam indicators)
2. Signs to look out for
3. Why it might be dangerous
4. What to do next

IMPORTANT GUIDELINES:
- Standard bank notifications (transfer confirmations, transaction alerts) from recognized banks (DBS, OCBC, UOB, etc.) are usually LEGITIMATE unless they contain suspicious elements
- Only flag as scam if you see: suspicious links, requests for passwords/PINs, urgent threats, poor grammar/spelling, mismatched sender details, or requests to click links for 'security'
- PayLah, PayNow, and other legitimate payment confirmations are typically safe
- School fee reminders and official notices are usually legitimate
- Look for context clues: does the message match the claimed sender's typical format?
- When unsure, err on the side of caution and say 'No' unless there are obvious red flags

Use simple language. Don't add extra sections.";

                lblResult.Text = "🔄 Sending image to AI service...";
                btnCheckScam.Enabled = false;
                btnCheckScam.Text = "🔍 Analyzing...";

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
                        var result = json["choices"]?[0]?["message"]?["content"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            string lower = result.ToLower();
                            // If the AI says it can't help with this photo, show a red box
                            if (lower.Contains("can't help with identifying people") || lower.Contains("cannot help with identifying people") || lower.Contains("cannot help with authenticity") || lower.Contains("i'm sorry, i can't help"))
                            {
                                lblResult.Text = "<div class='result-danger-box'>" + Server.HtmlEncode(result).Replace("\n", "<br/>") + "</div>";
                            }
                            // If the AI says the photo is not a message/email/SMS, show a green box and do not determine scam
                            else if (lower.Contains("does not appear to contain a message") || lower.Contains("does not appear to be an email") || lower.Contains("does not appear to be a message") || lower.Contains("does not appear to be an sms") || lower.Contains("does not appear to contain text"))
                            {
                                lblResult.Text = "<div class='result-success'>This photo does not appear to contain a message or scam content.</div>";
                            }
                            else
                            {
                                // If the AI's answer is ambiguous or does not contain 'yes' or 'no', show a neutral message
                                if (!(lower.Contains("yes") || lower.Contains("no")))
                                {
                                    lblResult.Text = "<div class='result-success'>No scam detected in this image.</div>";
                                }
                                else
                                {
                                    string formatted = FormatScamResultToHtml(result);
                                    lblResult.Text = formatted;
                                }
                            }
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowResultSection", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');setResultBoxStyle();", true);
                        }
                        else
                        {
                            lblResult.Text = "❌ No response from AI. Please try another image.";
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
                    }
                }
            }
            catch (System.Web.HttpException hex) when (hex.Message.Contains("Maximum request length exceeded"))
            {
                lblResult.Text = "<div class='result-danger-box'>❌ The uploaded image is too large for scanning. Please upload an image smaller than 5MB.</div>";
            }
            catch (Exception ex)
            {
                lblResult.Text = $"❌ Error: {ex.Message}";
            }
            finally
            {
                btnCheckScam.Enabled = true;
                btnCheckScam.Text = "🔍 Check for Scam";
            }
        }
        #endregion

        #region Manual Text Input Handler
        /// <summary>
        /// Handles scam checking for both manual text input and uploaded images using OpenAI API.
        /// </summary>
        protected async void btnCheckScam_Click(object sender, EventArgs e)
        {
            string userInput = this.txtUserInput.Text.Trim();
            bool hasText = !string.IsNullOrEmpty(userInput);
            bool hasImage = fileScreenshot.HasFile;

            if ((hasText && hasImage) || (!hasText && !hasImage))
            {
                lblResult.Text = "⚠️ Please either paste a message OR upload a photo, not both.";
                return;
            }

            if (hasText)
            {
                await AnalyzeTextWithOpenAI(userInput);
                return;
            }

            // --- Image analysis logic (from previous btnCheckImage_Click) ---
            string fileExt = System.IO.Path.GetExtension(fileScreenshot.FileName).ToLower();
            if (fileExt != ".jpg" && fileExt != ".jpeg" && fileExt != ".png")
            {
                lblResult.Text = "⚠️ Only image files (.jpg, .jpeg, .png) are allowed.";
                return;
            }

            // Limit file size to 5MB for safety
            if (fileScreenshot.PostedFile.ContentLength > 5 * 1024 * 1024)
            {
                lblResult.Text = "⚠️ Image file is too large. Please upload an image smaller than 5MB.";
                return;
            }

            byte[] imageBytes = fileScreenshot.FileBytes;
            string base64Image = Convert.ToBase64String(imageBytes);

            string prompt = @"Please check if this image contains a scam message. Respond in this exact format:

1. Yes/No: Is it a scam?

2. Signs to look out for
- (bullet point 1)
- (bullet point 2)
- (bullet point 3)

3. Why it might be dangerous
- (bullet point 1)
- (bullet point 2)
- (bullet point 3)

4. What to do next
- (bullet point 1)
- (bullet point 2)
- (bullet point 3)

Use simple language for elderly users. Do not add extra sections or explanations.";

            lblResult.Text = "🔄 Sending image to AI service...";
            btnCheckScam.Enabled = false;
            btnCheckScam.Text = "🔍 Analyzing...";

            try
            {
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
                        var result = json["choices"]?[0]?["message"]?["content"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            string formatted = FormatScamResultToHtml(result);
                            lblResult.Text = formatted;
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowResultSection", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');setResultBoxStyle();", true);
                        }
                        else
                        {
                            lblResult.Text = "❌ No response from AI. Please try another image.";
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
                    }
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = $"❌ Error: {ex.Message}";
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
                return;
            }

            this.btnCheckScam.Enabled = false;
            this.btnCheckScam.Text = "🔍 Analyzing...";

            // At the start of analysis, hide result and reset background
            Page.ClientScript.RegisterStartupScript(this.GetType(), "HideResultSection", "document.getElementById('resultSection').classList.add('hidden');document.querySelector('.checker-main-bg').classList.remove('expanded');", true);

            string prompt = $@"Please check if this message is a scam. Respond in this exact format:

1. Yes/No: Is it a scam?

2. Signs to look out for
- (bullet point 1)
- (bullet point 2)
- (bullet point 3)

3. Why it might be dangerous
- (bullet point 1)
- (bullet point 2)
- (bullet point 3)

4. What to do next
- (bullet point 1)
- (bullet point 2)
- (bullet point 3)

Here is the message to check:
{userInput}

Use simple language for elderly users. Do not add extra sections or explanations.";

            try
            {
                var chat = new ChatClient(model: "gpt-3.5-turbo", apiKey: apiKey);
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
                    string formatted = FormatScamResultToHtml(analysis);
                    this.lblResult.Text = formatted;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowResultSection", "document.getElementById('resultSection').classList.remove('hidden');document.querySelector('.checker-main-bg').classList.add('expanded');setResultBoxStyle();", true);
                }
                else
                {
                    this.lblResult.Text = "❌ Unable to analyze the message. Please try again.";
                }
            }
            catch (Exception apiEx)
            {
                this.lblResult.Text = $"❌ API Error: {apiEx.Message}. Please check your internet connection and try again.";
            }
            finally
            {
                this.btnCheckScam.Enabled = true;
                this.btnCheckScam.Text = "Analyze";
            }
        }
        #endregion

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
    }
}

