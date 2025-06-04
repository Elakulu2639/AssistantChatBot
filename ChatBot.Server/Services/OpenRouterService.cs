using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ChatBot.Server.Services
{
    public class OpenRouterService : IChatModelService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public OpenRouterService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenRouter:ApiKey"] ?? throw new ArgumentNullException("OpenRouter API key not found");
            _baseUrl = configuration["OpenRouter:BaseUrl"] ?? throw new ArgumentNullException("OpenRouter BaseUrl not found");
        }

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            var request = new
            {
                model = "deepseek/deepseek-r1-0528:free",  // replace with your model
                messages = new[]
                {
                    new { role = "user", content = userMessage }
                }
            };

            var jsonRequest = JsonSerializer.Serialize(request);
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions")
            {
                Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            httpRequest.Headers.Add("HTTP-Referer", "http://localhost:62963");
            httpRequest.Headers.Add("X-Title", "ERP Assistant");

            try
            {
                var response = await _httpClient.SendAsync(httpRequest);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"Error {response.StatusCode}: {response.ReasonPhrase}\n{jsonResponse}";
                }

                try
                {
                    using var doc = JsonDocument.Parse(jsonResponse);
                    var root = doc.RootElement;

                    if (!root.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                    {
                        return $"Invalid response format: Missing or empty choices array.\nRaw JSON: {jsonResponse}";
                    }

                    var firstChoice = choices[0];
                    if (!firstChoice.TryGetProperty("message", out var message))
                    {
                        return $"Invalid response format: Missing message property.\nRaw JSON: {jsonResponse}";
                    }

                    if (!message.TryGetProperty("content", out var content))
                    {
                        return $"Invalid response format: Missing content property.\nRaw JSON: {jsonResponse}";
                    }

                    var responseContent = content.GetString();
                    return responseContent ?? "No content returned by the model.";
                }
                catch (JsonException ex)
                {
                    return $"Invalid JSON response format.\nRaw JSON: {jsonResponse}\nError: {ex.Message}";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Network error while calling OpenRouter API: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Unexpected error: {ex.Message}";
            }
        }
    }
}
