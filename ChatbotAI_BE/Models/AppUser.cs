using ChatbotAI_BE.Enums;
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

        [MaxLength(50)]
        public UserRole Role { get; set; } = UserRole.User;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public List<ChatSession> Sessions { get; set; } = new();    
        public List<AIModelActivity> Activities { get; set; } = new();

        //public AppUser(string username, string password, string provider, string providerId, string? email, string? name, UserRole role = UserRole.User)
        //{
        //    if (string.IsNullOrWhiteSpace(username))
        //        throw new ArgumentException("Username không hợp lệ.");
        //    if (string.IsNullOrWhiteSpace(password))
        //        throw new ArgumentException("Password không hợp lệ.");
        //    if (string.IsNullOrWhiteSpace(provider))
        //        throw new ArgumentException("Provider không hợp lệ.");
        //    if (string.IsNullOrWhiteSpace(providerId))
        //        throw new ArgumentException("ProviderId không hợp lệ.");

        //    Username = username;
        //    Password = password;
        //    Provider = provider;
        //    ProviderId = providerId;
        //    Email = email;
        //    Name = name;
        //    Role = role;
        //}

    }
}

