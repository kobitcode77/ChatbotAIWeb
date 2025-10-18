using ChatbotAI_BE.Enums;

namespace ChatbotAI_BE.Dtos
{
    public class SendMessageRequest
    {
        public string Content { get; set; } = string.Empty;
        public ChatRole Role { get; set; } = ChatRole.User;
    }
}
