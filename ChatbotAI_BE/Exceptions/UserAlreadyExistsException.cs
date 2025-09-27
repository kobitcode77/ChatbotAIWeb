namespace ChatbotAI_BE.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException() : base("User already exists!") { }
    }
}
