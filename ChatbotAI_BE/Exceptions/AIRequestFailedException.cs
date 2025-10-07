namespace ChatbotAI_BE.Exceptions
{
    public class AIRequestFailedException : Exception
    {
        public AIRequestFailedException(string message) : base(message) { }
    }
}
