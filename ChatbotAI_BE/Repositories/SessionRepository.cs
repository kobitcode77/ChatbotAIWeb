using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface ISessionRepository
    {
        Task<List<ChatSession>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<ChatSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ChatSession?> GetByIdAndUserIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(ChatSession session);
        Task DeleteAsync(ChatSession session);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class SessionRepository : ISessionRepository
    {
        private readonly ChatDBContext _db;

        public SessionRepository(ChatDBContext db)
        {
            _db = db;
        }

        public async Task<List<ChatSession>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.ChatSessions
                .Include(s => s.User)
                .Include(s => s.Messages)
                .OrderByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ChatSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.ChatSessions
                .Where(s => s.UserId == userId)
                .Include(s => s.Messages)
                .OrderByDescending(s => s.LastUpdatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ChatSession?> GetByIdAndUserIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.ChatSessions
                .Include(s => s.Messages)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId, cancellationToken);
        }

        public Task AddAsync(ChatSession session)
        {
            _db.ChatSessions.Add(session);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ChatSession session)
        {
            _db.ChatSessions.Remove(session);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
