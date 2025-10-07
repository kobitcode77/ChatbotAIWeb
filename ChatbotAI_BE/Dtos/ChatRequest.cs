using ChatbotAI_BE.Enums;

namespace ChatbotAI_BE.Dtos
{
    public class ChatRequest
    {
        public string Message { get; set; } = null!;
        public AIModel Model { get; set; } = AIModel.DeepSeekFree;
        public Guid? SessionId { get; set; }
    }
}
