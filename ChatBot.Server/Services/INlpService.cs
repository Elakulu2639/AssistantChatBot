using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBot.Server.Models;

namespace ChatBot.Server.Services
{
    public interface INlpService
    {
        Task<IntentResult> AnalyzeIntentAsync(string userMessage, List<ChatHistory> chatHistory);
        Task<List<string>> ExtractEntitiesAsync(string userMessage);
        Task<double> CalculateConfidenceAsync(string userMessage, string intent, List<ChatHistory> chatHistory);
    }

    public class IntentResult
    {
        public string Intent { get; set; }
        public double Confidence { get; set; }
        public List<string> Entities { get; set; }
    }
}