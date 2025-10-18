namespace ChatbotAI_BE.Dtos.Auth
{
    public class RegisterRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? Email { get; set; }
        public string? Name { get; set; }

    }
}
