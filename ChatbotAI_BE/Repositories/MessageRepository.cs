using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface IMessageRepository
    {
        Task<List<ChatMessage>> GetMessagesAsync(Guid sessionId, Guid userId);
        Task<ChatMessage?> GetMessageAsync(Guid messageId, Guid userId);
        Task AddMessageAsync(ChatMessage message);
        Task DeleteMessageAsync(ChatMessage message);
        Task SaveChangesAsync();
    }
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatDBContext _db;

        public MessageRepository(ChatDBContext db)
        {
            _db = db;
        }

        public async Task<List<ChatMessage>> GetMessagesAsync(Guid sessionId, Guid userId)
        {
            return await _db.ChatMessages
                .Where(m => m.ChatSessionId == sessionId && m.ChatSession.UserId == userId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }


        public async Task<ChatMessage?> GetMessageAsync(Guid messageId, Guid userId)
        {
            return await _db.ChatMessages
                .Include(m => m.ChatSession)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ChatSession.UserId == userId);
        }



        public async Task AddMessageAsync(ChatMessage message)
        {
            _db.ChatMessages.Add(message);
            await Task.CompletedTask;
        }


        public async Task DeleteMessageAsync(ChatMessage message)
        {
            _db.ChatMessages.Remove(message);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}

