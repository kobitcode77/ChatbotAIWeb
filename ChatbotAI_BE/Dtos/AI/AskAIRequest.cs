namespace ChatbotAI_BE.Dtos.AI
{
    public class AskAIRequest
    {
        public Guid ModelId { get; set; }
        public string UserMessage { get; set; } = string.Empty;
        public Guid? SessionId { get; set; }
    }
}
