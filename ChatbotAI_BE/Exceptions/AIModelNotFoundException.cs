namespace ChatbotAI_BE.Exceptions
{
    public class AIModelNotFoundException : Exception
    {
        public AIModelNotFoundException() : base("AI model does not exist!") { }
    }
}
