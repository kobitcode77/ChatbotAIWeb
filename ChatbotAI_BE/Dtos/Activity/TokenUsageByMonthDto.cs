namespace ChatbotAI_BE.Dtos.Activity
{
    public class TokenUsageByMonthDto
    {
        public string ModelCode { get; set; } = default!;
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalTokens { get; set; }
    }
}
