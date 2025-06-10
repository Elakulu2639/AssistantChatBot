using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ChatBot.Server.Data;
using ChatBot.Server.Models;
using ChatBot.Server.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;


namespace ChatBot.Server.Services
{
    public class OpenRouterService : IChatModelService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenRouterService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly INlpService _nlpService;
        private readonly TrainingDataService _trainingDataService;
        private readonly IOptions<OpenRouterSettings> _settings;

        public OpenRouterService(
            HttpClient httpClient,
            ILogger<OpenRouterService> logger,
            ApplicationDbContext dbContext,
            INlpService nlpService,
            TrainingDataService trainingDataService,
            IOptions<OpenRouterSettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _dbContext = dbContext;
            _nlpService = nlpService;
            _trainingDataService = trainingDataService;
            _settings = settings;

            var apiKey = settings.Value.ApiKey;
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey), "OpenRouter API key is not configured");
            }

            _httpClient.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:62963");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "ERP Assistant");
        }

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            try
            {
                // Log the incoming message
                _logger.LogInformation("Processing message: {Message}", userMessage);

                // First, try to get response from training data
                var trainingResponse = _trainingDataService.GetResponseForQuery(userMessage);
                if (!string.IsNullOrEmpty(trainingResponse))
                {
                    _logger.LogInformation("Found matching training data: {Response}", trainingResponse);

                    // Use the training data as context for a more natural response
                    var systemPrompt = @"You are an intelligent ERP assistant that helps users with business processes, HR policies, and organizational tasks. 
                    You have access to the following information:
                    - HR policies including attendance, leave, conduct, and performance evaluation
                    - Business processes and workflows
                    - Organizational data and procedures
                    
                    Guidelines for responses:
                    1. For general greetings or casual questions:
                       - Respond naturally and briefly
                       - Be friendly but professional
                       - Offer to help with specific tasks
                       - Mention key areas you can assist with (HR, Sales, Finance, etc.)
                    
                    2. For specific questions:
                       - Use the provided training data as context
                       - Provide a natural, conversational response
                       - Include all necessary information from the training data
                       - Make the response sound human and helpful
                       - Don't just repeat the training data verbatim
                       - Add helpful context or suggestions when appropriate";

                    var messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = $"Based on this training data: '{trainingResponse}', please provide a natural response to: '{userMessage}'" }
                    };

                    var jsonPayload = JsonSerializer.Serialize(new
                    {
                        model = "deepseek/deepseek-r1-0528-qwen3-8b:free",
                        messages = messages,
                        temperature = 0.7,
                        max_tokens = 1000,
                        top_p = 0.9,
                        presence_penalty = 0.6,
                        frequency_penalty = 0.3
                    });

                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("chat/completions", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("OpenRouter API error: {StatusCode}, Response: {ErrorContent}",
                            response.StatusCode,
                            responseContent);
                        throw new Exception($"API request failed with status code {response.StatusCode}");
                    }

                    var botResponse = ExtractResponseFromJson(responseContent);
                    if (string.IsNullOrWhiteSpace(botResponse))
                    {
                        throw new Exception("Empty response from API");
                    }

                    // Get intent analysis
                    var intentResult = await _nlpService.AnalyzeIntentAsync(userMessage);

                    // Save chat history
                    await SaveChatHistory(userMessage, botResponse, intentResult);

                    return botResponse;
                }

                _logger.LogInformation("No training data match found, using OpenRouter API for general response");

                // If no training data match, use OpenRouter API for general response
                var generalSystemPrompt = @"You are an intelligent ERP assistant that helps users with business processes, HR policies, and organizational tasks. 
                You have access to the following information:
                - HR policies including attendance, leave, conduct, and performance evaluation
                - Business processes and workflows
                - Organizational data and procedures
                
                Guidelines for responses:
                1. For general greetings or casual questions:
                   - Respond naturally and briefly
                   - Be friendly but professional
                   - Offer to help with specific tasks
                   - Mention key areas you can assist with (HR, Sales, Finance, etc.)
                
                2. For specific questions:
                   - Provide helpful and accurate information
                   - Be clear and concise
                   - If you're not sure about something, say so
                   - Suggest relevant areas or departments that might help";

                var generalMessages = new[]
                {
                    new { role = "system", content = generalSystemPrompt },
                    new { role = "user", content = userMessage }
                };

                var generalJsonPayload = JsonSerializer.Serialize(new
                {
                    model = "deepseek/deepseek-r1-0528-qwen3-8b:free",
                    messages = generalMessages,
                    temperature = 0.7,
                    max_tokens = 1000,
                    top_p = 0.9,
                    presence_penalty = 0.6,
                    frequency_penalty = 0.3
                });

                var generalContent = new StringContent(generalJsonPayload, Encoding.UTF8, "application/json");

                var generalResponse = await _httpClient.PostAsync("chat/completions", generalContent);
                var generalResponseContent = await generalResponse.Content.ReadAsStringAsync();

                if (!generalResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("OpenRouter API error: {StatusCode}, Response: {ErrorContent}",
                        generalResponse.StatusCode,
                        generalResponseContent);
                    throw new Exception($"API request failed with status code {generalResponse.StatusCode}");
                }

                var generalBotResponse = ExtractResponseFromJson(generalResponseContent);
                if (string.IsNullOrWhiteSpace(generalBotResponse))
                {
                    throw new Exception("Empty response from API");
                }

                // Get intent analysis
                var generalIntentResult = await _nlpService.AnalyzeIntentAsync(userMessage);

                // Save chat history
                await SaveChatHistory(userMessage, generalBotResponse, generalIntentResult);

                return generalBotResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message: {Message}", ex.Message);
                throw; // Let the controller handle the error
            }
        }

        private string ExtractResponseFromJson(string json)
        {
            try
            {
                _logger.LogInformation("Attempting to parse response JSON: {Json}", json);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Try different response formats
                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    if (firstChoice.TryGetProperty("message", out var message) &&
                        message.TryGetProperty("content", out var content))
                    {
                        var botResponse = content.GetString();
                        _logger.LogInformation("Found response in choices[0].message.content: {Response}", botResponse);
                        if (string.IsNullOrWhiteSpace(botResponse))
                        {
                            throw new Exception("Empty response content from API");
                        }
                        return botResponse;
                    }
                }

                // Try alternative format
                if (root.TryGetProperty("response", out var altResponse))
                {
                    var responseText = altResponse.GetString();
                    _logger.LogInformation("Found response in response property: {Response}", responseText);
                    if (string.IsNullOrWhiteSpace(responseText))
                    {
                        throw new Exception("Empty response from API");
                    }
                    return responseText;
                }

                // If we can't find the expected format, log the structure
                _logger.LogError("Unexpected response format. Available properties: {Properties}",
                    string.Join(", ", root.EnumerateObject().Select(p => p.Name)));
                throw new Exception("Unexpected response format from API");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing OpenRouter response: {Json}", json);
                throw new Exception("Invalid JSON response from API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error parsing response");
                throw;
            }
        }

        private async Task SaveChatHistory(string userMessage, string botResponse, IntentResult intentResult)
        {
            try
            {
                _logger.LogInformation("Starting to save chat history...");
                _logger.LogInformation("User Message: {UserMessage}", userMessage);
                _logger.LogInformation("Bot Response: {BotResponse}", botResponse);
                _logger.LogInformation("Intent: {Intent}, Confidence: {Confidence}", intentResult.Intent, intentResult.Confidence);

                var chatHistory = new ChatHistory
                {
                    SessionId = Guid.NewGuid().ToString(),
                    UserMessage = userMessage,
                    BotResponse = botResponse,
                    Timestamp = DateTime.UtcNow,
                    Intent = intentResult.Intent,
                    Entities = string.Join(",", intentResult.Entities ?? new List<string>()),
                    Confidence = intentResult.Confidence
                };

                _logger.LogInformation("Created ChatHistory object with SessionId: {SessionId}", chatHistory.SessionId);

                _dbContext.ChatHistories.Add(chatHistory);
                _logger.LogInformation("Added ChatHistory to DbContext");

                var result = await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully saved chat history. Rows affected: {RowsAffected}", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving chat history: {ErrorMessage}", ex.Message);
                _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
                // Don't throw the exception - we don't want to break the chat flow if history saving fails
            }
        }
    }
}
