using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;

namespace ChatbotAI_BE.Services
{
    public interface ISessionService
    {
        Task<List<ChatSession>> GetAllSessionsAsync(CancellationToken cancellationToken = default);
        Task<List<ChatSession>> GetUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ChatSession?> GetSessionByIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
        Task<ChatSession> CreateSessionAsync(Guid userId, Guid modelId, string? title, CancellationToken cancellationToken = default);
        Task DeleteSessionAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
    }

    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepo;
        private readonly IUserRepository _userRepo;
        private readonly IAIModelRepository _modelRepo;

        public SessionService(
            ISessionRepository sessionRepo,
            IUserRepository userRepo,
            IAIModelRepository modelRepo)
        {
            _sessionRepo = sessionRepo;
            _userRepo = userRepo;
            _modelRepo = modelRepo;
        }

        /// Lấy toàn bộ session (chỉ dành cho admin)
        public Task<List<ChatSession>> GetAllSessionsAsync(CancellationToken cancellationToken = default)
        {
            return _sessionRepo.GetAllAsync(cancellationToken);
        }

        /// Lấy danh sách session của một người dùng
        public Task<List<ChatSession>> GetUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return _sessionRepo.GetByUserIdAsync(userId, cancellationToken);
        }

        /// Lấy thông tin chi tiết một session theo ID và User ID (đảm bảo quyền truy cập)
        public Task<ChatSession?> GetSessionByIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
        {
            return _sessionRepo.GetByIdAndUserIdAsync(sessionId, userId, cancellationToken);
        }

        /// Tạo mới một session cho người dùng với mô hình AI cụ thể
        public async Task<ChatSession> CreateSessionAsync(Guid userId, Guid modelId, string? title, CancellationToken cancellationToken = default)
        {
            // Kiểm tra user có tồn tại không
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new UserNotFoundException();

            // Kiểm tra model có tồn tại không
            var model = await _modelRepo.GetModelByIdAsync(modelId, cancellationToken);
            if (model == null)
                throw new AIModelNotFoundException();

            var session = new ChatSession
            {
                UserId = userId,
                ModelId = modelId,
                Title = title ?? $"Cuộc trò chuyện {DateTime.UtcNow:yyyy-MM-dd HH:mm}",

            };

            await _sessionRepo.AddAsync(session);
            await _sessionRepo.SaveChangesAsync(cancellationToken);

            return session;
        }

        /// Xóa một session của người dùng
        public async Task DeleteSessionAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
        {
            var session = await _sessionRepo.GetByIdAndUserIdAsync(sessionId, userId, cancellationToken);
            if (session == null)
                throw new SessionNotFoundException();

            await _sessionRepo.DeleteAsync(session);
            await _sessionRepo.SaveChangesAsync(cancellationToken);
        }
    }
}
