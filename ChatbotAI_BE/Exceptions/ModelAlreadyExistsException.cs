namespace ChatbotAI_BE.Exceptions
{
    public class ModelAlreadyExistsException : Exception
    {
            public ModelAlreadyExistsException() : base("AI Model already exits!") { }

    }
}
