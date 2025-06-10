using ChatBot.Server.Models;

namespace ChatBot.Server.Services
{
    public interface INlpService
    {
        Task<IntentResult> AnalyzeIntentAsync(string userMessage);
        Task<List<string>> ExtractEntitiesAsync(string userMessage);
        Task<double> CalculateConfidenceAsync(string userMessage, string intent);
    }

    public class IntentResult
    {
        public string Intent { get; set; }
        public double Confidence { get; set; }
        public List<string> Entities { get; set; }
    }
}