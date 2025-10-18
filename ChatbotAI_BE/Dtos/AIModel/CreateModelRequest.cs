namespace ChatbotAI_BE.Dtos.AIModel
{
    public class CreateModelRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MaxTokens { get; set; } = 4096;
        public int TotalContext { get; set; } = 8192;
    }
}
