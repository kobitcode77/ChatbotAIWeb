using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ChatbotAI_BE.Dtos.AI;
using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;

namespace ChatbotAI_BE.AI
{
    public class AIApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly AISettings _aiSettings;
        private readonly ConversationBuilder _conversationBuilder;

        public AIApiClient(HttpClient httpClient, IConfiguration config, AISettings aiSettings, ConversationBuilder conversationBuilder)
        {
            _httpClient = httpClient;
            _config = config;
            _aiSettings = aiSettings;
            _conversationBuilder = conversationBuilder;

            _httpClient.BaseAddress = new Uri(_aiSettings.BaseAddress);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config["OpenAI:ApiKey"]);
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", _config["OpenAI:Referer"]);
            _httpClient.DefaultRequestHeaders.Add("X-Title", _config["OpenAI:Title"]);
        }

        public async Task<AIResponse> AskAsync(AIModel model, IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default)
        {
            try
            {
                var payload = new
                {
                    model = model.Name,
                    max_tokens = _aiSettings.MaxTokens,
                    temperature = _aiSettings.Temperature,
                    messages = _conversationBuilder.BuildForAI(messages)
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
                var messageContent = doc.RootElement
             .GetProperty("choices")[0]
             .GetProperty("message")
             .GetProperty("content")
             .GetString() ?? "Không có phản hồi từ AI.";

                var usage = doc.RootElement.GetProperty("usage");

                return new AIResponse
                {
                    Content = messageContent,
                    PromptTokens = usage.GetProperty("prompt_tokens").GetInt32(),
                    CompletionTokens = usage.GetProperty("completion_tokens").GetInt32(),
                    TotalTokens = usage.GetProperty("total_tokens").GetInt32()
                };
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
