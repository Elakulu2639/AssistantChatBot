using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChatBot.Server.Models;

namespace ChatBot.Server.Services
{
    public interface IChatModelService
    {
        ChatResponse GetResponse(string message);
        void TrainModel(List<ChatTrainingData> trainingData);
    }

    public class ChatModelService : IChatModelService
    {
        private List<ChatTrainingData> _trainingData;
        private readonly Dictionary<string, double> _categoryWeights;

        public ChatModelService()
        {
            _trainingData = new List<ChatTrainingData>();
            _categoryWeights = new Dictionary<string, double>
            {
                { "inventory", 1.0 },
                { "hr", 1.0 },
                { "finance", 1.0 },
                { "sales", 1.0 },
                { "purchase", 1.0 }
            };
        }

        public void TrainModel(List<ChatTrainingData> trainingData)
        {
            _trainingData = trainingData;
        }

        public ChatResponse GetResponse(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return new ChatResponse
                {
                    Response = "I didn't understand that. Could you please rephrase?",
                    Category = "unknown",
                    Confidence = 0.0
                };
            }

            message = message.ToLower();
            var bestMatch = FindBestMatch(message);

            if (bestMatch.Confidence < 0.3)
            {
                return new ChatResponse
                {
                    Response = "I'm not sure I understand. Could you try asking about:\n" +
                             "- Inventory management\n" +
                             "- HR operations\n" +
                             "- Financial management\n" +
                             "- Sales operations\n" +
                             "- Purchase management",
                    Category = "unknown",
                    Confidence = bestMatch.Confidence
                };
            }

            return bestMatch;
        }

        private ChatResponse FindBestMatch(string message)
        {
            var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var bestMatch = new ChatResponse { Confidence = 0.0 };
            var maxScore = 0.0;

            foreach (var training in _trainingData)
            {
                var score = CalculateMatchScore(message, training);
                if (score > maxScore)
                {
                    maxScore = score;
                    bestMatch = new ChatResponse
                    {
                        Response = training.Answer,
                        Category = training.Category,
                        Confidence = score
                    };
                }
            }

            return bestMatch;
        }

        private double CalculateMatchScore(string message, ChatTrainingData training)
        {
            var score = 0.0;

            // Check for exact matches in keywords
            foreach (var keyword in training.Keywords)
            {
                if (message.Contains(keyword.ToLower()))
                {
                    score += 0.3;
                }
            }

            // Check for category matches
            if (_categoryWeights.ContainsKey(training.Category))
            {
                score += _categoryWeights[training.Category] * 0.2;
            }

            // Check for similar words
            var messageWords = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var trainingWords = training.Question.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in messageWords)
            {
                if (trainingWords.Contains(word))
                {
                    score += 0.1;
                }
            }

            return Math.Min(score, 1.0);
        }
    }
} 