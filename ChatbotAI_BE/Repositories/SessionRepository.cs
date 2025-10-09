using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface ISessionRepository
    {
        Task<List<ChatSession>> GetAllSessionsAsync();
        Task<List<ChatSession>> GetSessionsByUserAsync(Guid userId);
        Task<ChatSession?> GetSessionByIdAndUserAsync(Guid sessionId, Guid userId);
        Task AddSessionAsync(ChatSession session);
        Task DeleteSessionAsync(ChatSession session);
        Task SaveChangesAsync();
    }
    public class SessionRepository : ISessionRepository
    {
        private readonly ChatDBContext _db;

        public SessionRepository(ChatDBContext db)
        {
            _db = db;
        }
        public async Task<List<ChatSession>> GetAllSessionsAsync()
        {
            return await _db.ChatSessions
                .Include(s => s.User)
                .Include(s => s.Messages)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<ChatSession>> GetSessionsByUserAsync(Guid userId)
        {
            return await _db.ChatSessions
                .Where(s => s.UserId == userId)
                .Include(s => s.Messages)
                .OrderByDescending(s => s.LastUpdatedAt)
                .ToListAsync();
        }

        public async Task<ChatSession?> GetSessionByIdAndUserAsync(Guid sessionId, Guid userId)
        {
            return await _db.ChatSessions
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
        }

        public Task AddSessionAsync(ChatSession session)
        {
            _db.ChatSessions.Add(session);
            return Task.CompletedTask;
        }

        public Task DeleteSessionAsync(ChatSession session)
        {
            _db.ChatSessions.Remove(session);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
