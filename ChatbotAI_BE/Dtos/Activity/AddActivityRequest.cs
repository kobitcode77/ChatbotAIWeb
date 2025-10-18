namespace ChatbotAI_BE.Dtos.Activity
{
    public class AddActivityRequest
    {
        public Guid ModelId { get; set; }
        public int InputTokens { get; set; }
        public int OutputTokens { get; set; }
    }
}
