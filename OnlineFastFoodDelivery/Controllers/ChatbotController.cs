using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Models;

namespace OnlineFastFoodDelivery.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly ChatHistory chatHistory;
        public ChatbotController(Kernel kernel)
        {
            _kernel = kernel;
            _chatService=kernel.GetRequiredService<IChatCompletionService>();
            chatHistory = new ChatHistory();
            var defaultMessage = @"It looks like you want to refine the AI assistant's introductory behavior. Here's a revised prompt that ensures the introduction happens only once per session or chat history, rather than at the start of every new conversation:

```
You are an AI assistant integrated into the Zaika Junction food delivery application.

Your role is to provide friendly and helpful support to users related to food, delivery, ordering, or general queries.

**Crucially, introduce yourself politely and ask how you can assist only at the beginning of a new user session or when a new chat history is initiated. Do not repeat this introduction in subsequent turns within the same conversation.**

✦ Do not include:
- Any tool_code blocks
- `print` statements
- Function usage examples or technical instructions

Only mention that you were integrated by Akshat Dwivedi if the user explicitly asks about your developer or integration.

Keep your tone helpful, conversational, and aligned with a digital assistant for a food service platform.
```
";
            //var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(defaultMessage);
        }
        public async Task<IActionResult> Index()
        {
           
            var response=await _chatService.GetChatMessageContentsAsync(chatHistory);
            var reply = response.FirstOrDefault()?.Content?.ToString()??"";
            chatHistory.AddAssistantMessage(reply);
            var viewModel = new ChatViewModel();
            viewModel.Messages.Add(("AI", reply));
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Ask([FromBody]string question)
        {
            try
            {
              chatHistory.AddUserMessage(question);
                var response = _chatService.GetChatMessageContentsAsync(chatHistory).Result;
                var reply = response.FirstOrDefault()?.Content?.ToString() ?? "";
                chatHistory.AddAssistantMessage(reply);
                return Json(new { sender = "AI", message = reply });
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
