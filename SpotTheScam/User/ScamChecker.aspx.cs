using System;
using System.Threading.Tasks;
using OpenAI.Chat;

namespace SpotTheScam.User
{
    public partial class ScamChecker : System.Web.UI.Page
    {
        // You can store the API key securely elsewhere
        private static readonly string apiKey = "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";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize the page
            }
        }

        protected async void btnCheckScam_Click(object sender, EventArgs e)
        {
            try
            {
                string userInput = this.txtUserInput.Text.Trim();
                if (string.IsNullOrEmpty(userInput))
                {
                    this.lblResult.Text = "⚠️ Please enter a message to check.";
                    return;
                }

                this.btnCheckScam.Enabled = false;
                this.btnCheckScam.Text = "🔍 Analyzing...";

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
                        this.lblResult.Text = Server.HtmlEncode(analysis).Replace("\n", "<br/>");
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
            }
            catch (Exception ex)
            {
                this.lblResult.Text = $"❌ Error analyzing message: {ex.Message}. Please try again.";
            }
            finally
            {
                this.btnCheckScam.Enabled = true;
                this.btnCheckScam.Text = "🔍 Check for Scam";
            }
        }
    }
}
