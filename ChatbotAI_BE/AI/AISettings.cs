namespace ChatbotAI_BE.AI
{
    public class AISettings
    {
        public int MaxTokens { get; set; }
        public double Temperature { get; set; }
        public int MaxMessageHistory { get; set; }
        public string BaseAddress { get; set; } = string.Empty;
    }
}
