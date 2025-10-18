using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;

namespace ChatbotAI_BE.Services
{
    public interface IMessageService
    {
        Task<List<ChatMessage>> GetMessagesBySessionAndUserIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
        Task<ChatMessage?> GetMessageByIdAndUserIdAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default);
        Task<ChatMessage> SendMessageAsync(Guid sessionId, Guid userId, string content, ChatRole role, CancellationToken cancellationToken = default);
        Task DeleteMessageAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default);
    }

    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ISessionRepository _sessionRepository;

        public MessageService(IMessageRepository messageRepository, ISessionRepository sessionRepository)
        {
            _messageRepository = messageRepository;
            _sessionRepository = sessionRepository;
        }

        /// Lấy toàn bộ tin nhắn trong một phiên chat của người dùng
        public async Task<List<ChatMessage>> GetMessagesBySessionAndUserIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _messageRepository.GetBySessionAndUserIdAsync(sessionId, userId, cancellationToken);
        }

        /// Lấy 1 tin nhắn cụ thể theo messageId và userId
        public async Task<ChatMessage?> GetMessageByIdAndUserIdAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _messageRepository.GetByIdAndUserIdAsync(messageId, userId, cancellationToken);
        }

        /// Gửi một tin nhắn mới trong phiên chat
        public async Task<ChatMessage> SendMessageAsync(Guid sessionId, Guid userId, string content, ChatRole role, CancellationToken cancellationToken = default)
        {
            // Xác thực quyền sở hữu session
            var session = await _sessionRepository.GetByIdAndUserIdAsync(sessionId, userId, cancellationToken);
            if (session == null)
                throw new UnauthorizedAccessException("Phiên chat không tồn tại hoặc không thuộc về người dùng này.");

            var message = new ChatMessage
            {
                ChatSessionId = sessionId,
                Role = role,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
            await _messageRepository.SaveChangesAsync(cancellationToken);

            return message;
        }

        /// Xóa một tin nhắn (chỉ xóa được nếu thuộc về user đó)
        public async Task DeleteMessageAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default)
        {
            var message = await _messageRepository.GetByIdAndUserIdAsync(messageId, userId, cancellationToken);
            if (message == null)
                throw new KeyNotFoundException("Tin nhắn không tồn tại hoặc bạn không có quyền xóa.");

            await _messageRepository.DeleteAsync(message);
            await _messageRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
