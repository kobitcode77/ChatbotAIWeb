using ChatbotAI_BE.Models;

namespace ChatbotAI_BE.Dtos
{
    public class CreateSessionRequest
    {
        public AIModel Model { get; set; } = null!;
        public string? Title { get; set; }
    }
}
