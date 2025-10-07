using System.ComponentModel.DataAnnotations;

namespace ChatbotAI_BE.Models
{
    public class AIModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = default!; 

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int MaxTokens { get; set; } = 4096;

        public int TotalContext { get; set; } = 8192; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<AIModelActivity> Activities { get; set; } = new();

    }
}
