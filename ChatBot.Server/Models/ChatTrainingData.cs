using System.Collections.Generic;

namespace ChatBot.Server.Models
{
    public class ChatTrainingData
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Category { get; set; }
        public List<string> Keywords { get; set; }
    }

    public class ChatResponse
    {
        public string? Response { get; set; }
        public string Category { get; set; }
        public double Confidence { get; set; }
    }
} 