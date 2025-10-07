using System.ComponentModel.DataAnnotations;

namespace ChatbotAI_BE.Models
{
    public class AIModelActivity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        [Required]
        public Guid ModelId { get; set; }
        public AIModel Model { get; set; } = null!;

        public int InputTokens { get; set; }       // prompt_tokens
        public int OutputTokens { get; set; }      // completion_tokens
        public int TotalTokens => InputTokens + OutputTokens;

        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}
