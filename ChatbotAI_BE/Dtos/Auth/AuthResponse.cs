namespace ChatbotAI_BE.Dtos.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}
