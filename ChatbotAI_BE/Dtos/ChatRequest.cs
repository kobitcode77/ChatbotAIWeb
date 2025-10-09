

using ChatbotAI_BE.Models;

namespace ChatbotAI_BE.Dtos
{
    public class ChatRequest
    {
        public string Message { get; set; } = null!;
        public AIModel Model { get; set; } = null!;
        public Guid? SessionId { get; set; }
    }
}
