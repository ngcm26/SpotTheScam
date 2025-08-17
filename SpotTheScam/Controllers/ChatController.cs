using OpenAI;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using OpenAI.Chat;


namespace SpotTheScam.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatClient _chatClient;

        public ChatController()
        {
            var apiKey = System.Configuration.ConfigurationManager.AppSettings["OpenAI:ApiKey"];
            var model = "gpt-4o-mini";
            _chatClient = new ChatClient(model, apiKey);
        }

        [HttpPost]
        public async Task<ActionResult> Ask(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { reply = "Please type a message." });

            // Call the async method
            var completionResult = await _chatClient.CompleteChatAsync(message);

            // Access the ChatCompletion object via .Value
            var completion = completionResult.Value;

            // Get the first reply text
            var reply = completion.Content[0].Text;

            return Json(new { reply });
        }


    }
}
