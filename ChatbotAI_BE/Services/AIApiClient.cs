using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Exceptions;

namespace ChatbotAI_BE.Services
{
    public class AIApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AIApiClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;

            _httpClient.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config["OpenAI:ApiKey"]);
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:4200");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "ChatBox AI");
        }

        public async Task<string> AskAsync(AIModel model, string userMessage)
        {
            try
            {
                var payload = new
                {
                    model = model.GetModelName(),
                    max_tokens = model.MaxTokens, // giới hạn token theo model
                    temperature = 0.7, // mức độ sáng tạo của response (0 - 2)
                    messages = new[]
                    {
                        new { role = "user", content = userMessage }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("chat/completions", content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new AIRequestFailedException($"AI API failed: {response.StatusCode} - {error}");
                }

                var body = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(body);
                return doc.RootElement
                          .GetProperty("choices")[0]
                          .GetProperty("message")
                          .GetProperty("content")
                          .GetString() ?? "Không có phản hồi từ AI.";
            }
            catch (HttpRequestException ex)
            {
                throw new AIRequestFailedException($"Failed to connect to AI service: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new AIRequestFailedException($"Invalid AI response format: {ex.Message}");
            }
            
        }
    }
}
