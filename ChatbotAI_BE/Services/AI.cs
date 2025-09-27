using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace ChatbotAI_BE.Services
{
    public class AI
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AI(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["OpenAI:ApiKey"]);
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:4200");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "ChatBox AI");
        }

        public async Task<string> AskAI(string userMessage)
        {
            var payload = new
            {
                model = "deepseek/deepseek-r1-0528:free",
                messages = new[] {
                    new { role = "user", content = userMessage }
                }
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            var aiReply = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();

            return aiReply ?? "Không có phản hồi từ AI.";
        }
    }
}
