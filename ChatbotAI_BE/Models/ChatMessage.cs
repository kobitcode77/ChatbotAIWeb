using ChatbotAI_BE.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChatbotAI_BE.Models
{
    public class ChatMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; } = null!;

        [Required]
        public ChatRole Role { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
