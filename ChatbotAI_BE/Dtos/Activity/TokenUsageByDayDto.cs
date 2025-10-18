namespace ChatbotAI_BE.Dtos.Activity
{
    public class TokenUsageByDayDto
    {
        public string ModelCode { get; set; } = default!;
        public DateTime Date { get; set; }
        public int TotalTokens { get; set; }
    }
}
