namespace Models
{
    public class Chatbot
    {

    }
    public class ChatViewModel
    {
        public List<(string Role, string Message)> Messages { get; set; } = new();
    }
}