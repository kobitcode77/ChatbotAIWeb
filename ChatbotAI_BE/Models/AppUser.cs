using System.ComponentModel.DataAnnotations;

namespace ChatbotAI_BE.Models
{
    public class AppUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(50)]
        public string? Username { get; set; }

        [MaxLength(255)]
        public string? Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string Provider { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string ProviderId { get; set; } = null!;

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Avatar { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

