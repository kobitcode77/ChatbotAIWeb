namespace ChatbotAI_BE.Dtos.AI
{
    public class AIResponse
    {
        public string Content { get; set; } = string.Empty;
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
}
