namespace ChatbotAI_BE.Enums
{
    public enum AIModel
    {
        DeepSeekFree,
        GPT4,
        Claude35,
        Llama32
    }

    public static class AIModelExtensions
    {
        public static string GetModelName(this AIModel model)
        {
            return model switch
            {
                AIModel.DeepSeekFree => "deepseek/deepseek-r1-0528:free",
                AIModel.GPT4 => "openai/gpt-4o-mini",
                AIModel.Claude35 => "anthropic/claude-3.5-sonnet",
                AIModel.Llama32 => "meta-llama/llama-3.2-11b",
                _ => "deepseek/deepseek-r1-0528:free"
            };
        }
    }
}
