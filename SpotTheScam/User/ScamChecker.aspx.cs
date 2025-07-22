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

            string prompt = @"Please help check if this image contains a scam message. 
If there is text, extract it and analyze if it is a scam. 
Explain your answer in a simple and friendly way that is easy for elderly people to understand.
Your response should include:
1. A clear answer: Is this a scam? (Yes or No)
2. Simple signs to look out for in the message
3. A short explanation of why it might be dangerous (if it is)
4. What the person should do next (like ignore, delete, or report it)
Avoid technical words. Use everyday language that even someone with little internet experience can understand.";

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
                            string cleaned = Server.HtmlEncode(result).Replace("\n", "<br/>");
                            // Add paragraph breaks between numbered points
                            string formatted = AddParagraphBreaksToNumberedList(cleaned);
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

            string prompt = @"Please help check if this image contains a scam message. 
If there is text, extract it and analyze if it is a scam. 
Explain your answer in a simple and friendly way that is easy for elderly people to understand.
Your response should include:
1. A clear answer: Is this a scam? (Yes or No)
2. Simple signs to look out for in the message
3. A short explanation of why it might be dangerous (if it is)
4. What the person should do next (like ignore, delete, or report it)
Avoid technical words. Use everyday language that even someone with little internet experience can understand.";

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
                            string cleaned = Server.HtmlEncode(result).Replace("\n", "<br/>");
                            // Add paragraph breaks between numbered points
                            string formatted = AddParagraphBreaksToNumberedList(cleaned);
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

            string prompt = $@"Please help check if this message is a scam. Explain your answer in a simple and friendly way that is easy for elderly people to understand.

Your response should include:
1. A clear answer: Is this a scam? (Yes or No)
2. Simple signs to look out for in the message
3. A short explanation of why it might be dangerous (if it is)
4. What the person should do next (like ignore, delete, or report it)

Here is the message to check:
{userInput}

Avoid technical words. Use everyday language that even someone with little internet experience can understand.";

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
                    string cleaned = ConvertMarkdownBoldToHtml(analysis);
                    string formatted = AddParagraphBreaksToNumberedList(cleaned);
                    this.lblResult.Text = formatted.Replace("\n", "<br/>");
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
    }
}

