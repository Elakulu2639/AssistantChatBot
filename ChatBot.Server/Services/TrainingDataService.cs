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

                using var reader = new StreamReader("Data/erp_case_data_expanded.csv");
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
            var normalizedMessage = userMessage.ToLower().Trim();
            _logger.LogInformation("Normalized user message: {NormalizedMessage}", normalizedMessage);

            // Find best match using fuzzy matching, specifically prioritizing exact matches
            var bestMatch = _trainingData
                .Select(t =>
                {
                    var trainingQuestionNormalized = t.Question.ToLower().Trim();
                    var score = Fuzz.Ratio(normalizedMessage, trainingQuestionNormalized);
                    _logger.LogDebug("Comparing '{UserMessage}' with '{TrainingQuestion}' - Score: {Score}",
                                   normalizedMessage, trainingQuestionNormalized, score);
                    return new
                    {
                        Data = t,
                        QuestionMatchScore = score,
                    };
                })
                .OrderByDescending(x => x.QuestionMatchScore) // Order by the question match score
                .FirstOrDefault();

            // If an exact match (Fuzz.Ratio == 100) is found, return its answer immediately
            if (bestMatch?.QuestionMatchScore == 100)
            {
                _logger.LogInformation("Found exact Fuzz.Ratio match for query: {Query} - Answer: {Answer}",
                    userMessage, bestMatch.Data.Answer);
                return bestMatch.Data.Answer;
            }

            // If no exact match, proceed with a high threshold for very similar matches
            if (bestMatch?.QuestionMatchScore >= 85) // Adjusted threshold for very close matches
            {
                _logger.LogInformation("Found very high fuzzy match for query: {Query} with score: {Score} - Answer: {Answer}",
                    userMessage, bestMatch.QuestionMatchScore, bestMatch.Data.Answer);
                return bestMatch.Data.Answer;
            }

            // Fallback if no strong match is found
            _logger.LogInformation("No good match found for query: {Query}", userMessage);
            return null;
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
        public string Entities { get; set; }
    }
}