using ChatbotAI_BE.Models;
namespace ChatbotAI_BE.Dtos.Session
{
    public class CreateSessionRequest
    {
        public Guid ModelId { get; set; }
        public string? Title { get; set; }
    }
}
