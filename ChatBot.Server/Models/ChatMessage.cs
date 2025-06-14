namespace ChatBot.Server.Models
{
    public class ChatMessage
    {
        public string UserMessage { get; set; } = string.Empty;
        public string? SessionId { get; set; }
    }
}