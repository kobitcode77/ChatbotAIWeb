using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Models;

namespace ChatbotAI_BE.AI
{
    public class ConversationBuilder
    {
        private readonly AISettings _settings;

        public ConversationBuilder(AISettings settings)
        {
            _settings = settings;
        }

        public object[] BuildForAI(IEnumerable<ChatMessage> messages)
        {
            return messages
                .OrderBy(m => m.SentAt)
                .TakeLast(_settings.MaxMessageHistory)
                .Select(m => new
                {
                    role = m.Role == ChatRole.User ? "user" : "assistant",
                    content = m.Content
                })
                .ToArray();
        }
    }
}
