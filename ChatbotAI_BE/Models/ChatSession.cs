using System.ComponentModel.DataAnnotations;

namespace ChatbotAI_BE.Models
{
    public class ChatSession
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        [Required]
        public Guid ModelId { get; set; }
        public AIModel Model { get; set; } = null!;

        [MaxLength(200)]
        public string? Title { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        public List<ChatMessage> Messages { get; set; } = new();

    }
}
