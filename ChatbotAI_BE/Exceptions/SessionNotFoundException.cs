namespace ChatbotAI_BE.Exceptions
{
    public class SessionNotFoundException : Exception
    {
        public SessionNotFoundException() : base("Chat session not found!") { }
    }
}
