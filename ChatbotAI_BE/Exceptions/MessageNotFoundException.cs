namespace ChatbotAI_BE.Exceptions
{
    public class MessageNotFoundException : Exception
    {
        public MessageNotFoundException() : base("Chat message not found!") { }
    }
}
