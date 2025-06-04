namespace ChatBot.Server.Settings
{
    public class OpenRouterSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = "https://openrouter.ai/chat/completions";
    }
}
