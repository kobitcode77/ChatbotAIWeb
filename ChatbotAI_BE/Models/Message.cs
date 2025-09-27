using ChatbotAI_BE.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChatbotAI_BE.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Sender { get; set; } = "guest";

        [MaxLength(100)]
        public string? Receiver { get; set; }

        [MaxLength(100)]
        public string? GroupName { get; set; }

        [Required, MaxLength(4000)]
        public string Content { get; set; } = "";

        [Required]
        public ChatScope Scope { get; set; } = ChatScope.World;

        public DateTime SendTime { get; set; } = DateTime.UtcNow;
    }
}
