using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FuzzySharp;

namespace ChatBot.Server.Services
{
    public class TrainingDataService
    {
        private readonly List<TrainingData> _trainingData;
        private readonly ILogger<TrainingDataService> _logger;

        public TrainingDataService(ILogger<TrainingDataService> logger)
        {
            _logger = logger;
            _trainingData = LoadTrainingData();
        }

        private List<TrainingData> LoadTrainingData()
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null
                };

                using var reader = new StreamReader("Data/erp_case_data_large.csv");
                using var csv = new CsvReader(reader, config);

                return csv.GetRecords<TrainingData>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading training data from CSV");
                return new List<TrainingData>();
            }
        }

        public string GetResponseForQuery(string userMessage)
        {
            var normalizedMessage = userMessage.ToLower();

            // Find exact match first
            var exactMatch = _trainingData.FirstOrDefault(t =>
                t.Question.ToLower() == normalizedMessage);

            if (exactMatch != null)
            {
                _logger.LogInformation("Found exact match for query: {Query}", userMessage);
                return exactMatch.Answer;
            }

            // Find best match using fuzzy matching
            var bestMatch = _trainingData
                .Select(t => new
                {
                    Data = t,
                    QuestionScore = Fuzz.Ratio(normalizedMessage, t.Question.ToLower()),
                    KeywordScore = CalculateKeywordScore(normalizedMessage, t),
                    PartialMatchScore = CalculatePartialMatchScore(normalizedMessage, t.Question.ToLower())
                })
                .Select(x => new
                {
                    Data = x.Data,
                    Score = Math.Max(x.QuestionScore, Math.Max(x.KeywordScore, x.PartialMatchScore))
                })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            if (bestMatch?.Score >= 70) // Threshold for considering it a match
            {
                _logger.LogInformation("Found fuzzy match for query: {Query} with score: {Score}",
                    userMessage, bestMatch.Score);
                return bestMatch.Data.Answer;
            }

            _logger.LogInformation("No good match found for query: {Query}", userMessage);
            return null; // No good match found
        }

        private double CalculateKeywordScore(string userMessage, TrainingData trainingData)
        {
            if (string.IsNullOrEmpty(trainingData.Keywords))
                return 0;

            var userWords = userMessage.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var keywordWords = trainingData.Keywords.ToLower().Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k))
                .ToList();

            if (!keywordWords.Any())
                return 0;

            var matchingWords = userWords.Count(word =>
                keywordWords.Any(keyword =>
                    word.Contains(keyword) || keyword.Contains(word)));

            return (double)matchingWords / Math.Max(userWords.Length, keywordWords.Count) * 100;
        }

        private double CalculatePartialMatchScore(string userMessage, string question)
        {
            var userWords = userMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var questionWords = question.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var matchingWords = userWords.Count(word =>
                questionWords.Any(qWord =>
                    Fuzz.Ratio(word, qWord) > 80));

            return (double)matchingWords / Math.Max(userWords.Length, questionWords.Length) * 100;
        }
    }

    public class TrainingData
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Category { get; set; }
        public string Keywords { get; set; }
    }
}