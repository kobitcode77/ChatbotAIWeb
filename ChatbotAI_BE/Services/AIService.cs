using ChatbotAI_BE.AI;
using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;

namespace ChatbotAI_BE.Services
{
    public interface IAIService
    {
        Task<string> AskAIAsync(Guid userId, string userMessage, AIModel model, Guid? sessionId = null, CancellationToken cancellationToken = default);
    }
    public class AIService : IAIService
    {
        private readonly ISessionRepository _sessionRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IActivityRepository _activityRepo;
        private readonly AIApiClient _aiClient;
        private readonly ConversationBuilder _conversationBuilder;

        public AIService(
                   ISessionRepository sessionRepo,
                   IMessageRepository messageRepo, IActivityRepository activityRepository,
                   AIApiClient aiClient,
                   ConversationBuilder conversationBuilder)
        {
            _sessionRepo = sessionRepo;
            _messageRepo = messageRepo;
            _activityRepo = activityRepository;
            _aiClient = aiClient;
            _conversationBuilder = conversationBuilder;
        }

        public async Task<string> AskAIAsync(Guid userId, string userMessage, AIModel model, Guid? sessionId = null, CancellationToken cancellationToken = default)
        {
            ChatSession session;
     
                // Tạo hoặc lấy session
                if (sessionId == null)
                {
                    session = new ChatSession
                    {
                        UserId = userId,
                        Model = model,
                        Title =  $"Cuộc trò chuyện {DateTime.UtcNow:yyyy-MM-dd HH:mm}"

                    };
                    await _sessionRepo.AddAsync(session);
                }
                else
                {
                    session = await _sessionRepo.GetByIdAndUserIdAsync(sessionId.Value, userId)
                        ?? throw new SessionNotFoundException();
                }

                // Lưu tin nhắn của user
                var userMsg = new ChatMessage
                {
                    ChatSession = session,
                    Role = ChatRole.User,
                    Content = userMessage
                };
                await _messageRepo.AddAsync(userMsg);
                await _messageRepo.SaveChangesAsync(cancellationToken);

                // Lấy toàn bộ tin nhắn trong session
                var allMessages = await _messageRepo.GetBySessionAndUserIdAsync(session.Id, userId, cancellationToken);


                // Gọi AI API với toàn bộ lịch sử hội thoại
                var aiResponse = await _aiClient.AskAsync(model, allMessages, cancellationToken);

                // Lưu phản hồi của AI
                var aiMsg = new ChatMessage
                {
                    ChatSession = session,
                    Role = ChatRole.AI,
                    Content = aiResponse.Content
                };
                await _messageRepo.AddAsync(aiMsg);
                await _messageRepo.SaveChangesAsync(cancellationToken);

                var activity = new AIModelActivity
                {
                    UserId = userId,
                    ModelId = model.Id,
                    InputTokens = aiResponse.PromptTokens,
                    OutputTokens = aiResponse.CompletionTokens,
                    UsedAt = DateTime.UtcNow
                };

                await _activityRepo.AddActivityAsync(activity);
                await _activityRepo.SaveChangesAsync(cancellationToken);

                return aiResponse.Content;
            }
        
    }
}
