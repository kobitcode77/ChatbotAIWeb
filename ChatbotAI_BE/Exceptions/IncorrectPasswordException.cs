namespace ChatbotAI_BE.Exceptions
{
    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException() : base("Incorrect password!") { }
    }
}
