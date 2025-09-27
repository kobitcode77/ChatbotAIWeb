namespace ChatbotAI_BE.Dtos
{
    public class RegisterRequest
    {
        public string Username { get; set; } = null;
        public string Password { get; set; } = null;
        public string? Email { get; set; }
        public string? Name { get; set; }

    }
}
