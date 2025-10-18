using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface IMessageRepository
    {
        Task<List<ChatMessage>> GetBySessionAndUserIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
        Task<ChatMessage?> GetByIdAndUserIdAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(ChatMessage message);
        Task DeleteAsync(ChatMessage message);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class MessageRepository : IMessageRepository
    {
        private readonly ChatDBContext _db;

        public MessageRepository(ChatDBContext db)
        {
            _db = db;
        }

        public async Task<List<ChatMessage>> GetBySessionAndUserIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.ChatMessages
                .Where(m => m.ChatSessionId == sessionId && m.ChatSession.UserId == userId)
                .OrderBy(m => m.SentAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ChatMessage?> GetByIdAndUserIdAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.ChatMessages
                .Include(m => m.ChatSession)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ChatSession.UserId == userId, cancellationToken);
        }

        public Task AddAsync(ChatMessage message)
        {
            _db.ChatMessages.Add(message);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ChatMessage message)
        {
            _db.ChatMessages.Remove(message);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
