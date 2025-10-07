using ChatbotAI_BE.Enums;

namespace ChatbotAI_BE.Dtos
{
    public class CreateSessionRequest
    {
        public AIModel Model { get; set; }
        public string? Title { get; set; }
    }
}
