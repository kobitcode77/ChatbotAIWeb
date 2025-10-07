using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;

namespace ChatbotAI_BE.Services
{
    public interface IAIService
    {
        Task<string> AskAIAsync(Guid userId, string userMessage, AIModel model, Guid? sessionId = null);
    }
    public class AIService : IAIService
    {
        private readonly ISessionRepository _sessionRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly AIApiClient _aiClient;

        public AIService(ISessionRepository sessionRepo, IMessageRepository messageRepo, AIApiClient aiClient)
        {
            _sessionRepo = sessionRepo;
            _messageRepo = messageRepo;
            _aiClient = aiClient;
        }

        public async Task<string> AskAIAsync(Guid userId, string userMessage, AIModel model, Guid? sessionId = null)
        {
            ChatSession session;
            try
            {
                // Tạo hoặc lấy session
                if (sessionId == null)
                {
                    session = new ChatSession { UserId = userId, Model = model };
                    await _sessionRepo.AddSessionAsync(session);
                }
                else
                {
                    session = await _sessionRepo.GetSessionAsync(sessionId.Value, userId)
                        ?? throw new SessionNotFoundException();
                }

                // Lưu tin nhắn của user
                var userMsg = new ChatMessage
                {
                    ChatSession = session,
                    Role = ChatRole.User,
                    Content = userMessage
                };
                await _messageRepo.AddMessageAsync(userMsg);
                await _messageRepo.SaveChangesAsync();

                // Gọi API AI
                var aiResponse = await _aiClient.AskAsync(model, userMessage);

                // Lưu phản hồi của AI
                var aiMsg = new ChatMessage
                {
                    ChatSession = session,
                    Role = ChatRole.AI,
                    Content = aiResponse
                };
                await _messageRepo.AddMessageAsync(aiMsg);
                await _messageRepo.SaveChangesAsync();

                return aiResponse;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
