using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Models;
using System.Text.Json;

namespace OnlineFastFoodDelivery.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;

        private const string SessionKey = "ChatHistory";

        public ChatbotController(Kernel kernel)
        {
            _kernel = kernel;
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
        }

        private ChatHistory GetChatHistory()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json))
            {
                var history = new ChatHistory();

                var systemPrompt = @"
You are an AI assistant integrated into the Zaika Junction food delivery application.

Your role is to provide friendly and helpful support to users related to food, delivery, ordering, or general queries.

**Crucially, introduce yourself politely and ask how you can assist only at the beginning of a new user session or when a new chat history is initiated. Do not repeat this introduction in subsequent turns within the same conversation.**

✦ Do not include:
- Any tool_code blocks
- `print` statements
- Function usage examples or technical instructions

Only mention that you were integrated by Akshat Dwivedi if the user explicitly asks about your developer or integration.

Keep your tone helpful, conversational, and aligned with a digital assistant for a food service platform.
";
                history.AddUserMessage(systemPrompt);
                SaveChatHistory(history);
                return history;
            }

            return JsonSerializer.Deserialize<ChatHistory>(json) ?? new ChatHistory();
        }

        private void SaveChatHistory(ChatHistory history)
        {
            var json = JsonSerializer.Serialize(history);
            HttpContext.Session.SetString(SessionKey, json);
        }

        public async Task<IActionResult> Index()
        {
            var chatHistory = GetChatHistory();

            var response = await _chatService.GetChatMessageContentsAsync(chatHistory);
            var reply = response.FirstOrDefault()?.Content ?? "";

            chatHistory.AddAssistantMessage(reply);
            SaveChatHistory(chatHistory);

            var viewModel = new ChatViewModel();
            viewModel.Messages.Add(("AI", reply));
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] string question)
        {
            try
            {
                var chatHistory = GetChatHistory();

                chatHistory.AddUserMessage(question);
                var response = await _chatService.GetChatMessageContentsAsync(chatHistory);
                var reply = response.FirstOrDefault()?.Content ?? "";

                chatHistory.AddAssistantMessage(reply);
                SaveChatHistory(chatHistory);

                return Json(new { sender = "AI", message = reply });
            }
            catch (Exception ex)
            {
                // Log the error
                return Json(new { sender = "AI", message = "Oops! Something went wrong. Please try again later." });
            }
        }
    }
}
