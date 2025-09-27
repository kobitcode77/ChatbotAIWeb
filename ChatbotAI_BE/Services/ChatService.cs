using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Services;
using ChatbotAI_BE.Data;
using Microsoft.EntityFrameworkCore;
using ChatbotAI_BE.Models;

namespace ChatbotAI_BE.Services
{
    public class ChatService : IChatService
    {
        private readonly ChatDBContext _db;

        public ChatService(ChatDBContext db) => _db = db;

        public async Task<Message> SaveWorldAsync(string sender, string content)
        {
            var msg = new Message { Sender = sender, Content = content, Scope = ChatScope.World, SendTime = DateTime.UtcNow };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();
            return msg;
        }

        public async Task<Message> SaveDirectAsync(string sender, string receiver, string content)
        {
            var msg = new Message { Sender = sender, Receiver = receiver, Content = content, Scope = ChatScope.Direct, SendTime = DateTime.UtcNow };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();
            return msg;
        }

        public async Task<Message> SaveGroupAsync(string sender, string groupName, string content)
        {
            var msg = new Message { Sender = sender, GroupName = groupName, Content = content, Scope = ChatScope.Group, SendTime = DateTime.UtcNow };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();
            return msg;
        }

        public async Task<IReadOnlyList<Message>> GetWorldAsync(int limit = 200)
            => await _db.Messages
                        .Where(m => m.Scope == ChatScope.World)
                        .OrderByDescending(m => m.SendTime).Take(limit)
                        .OrderBy(m => m.SendTime).ToListAsync();

        public async Task<IReadOnlyList<Message>> GetDirectAsync(string user1, string user2, int limit = 200)
            => await _db.Messages
                        .Where(m => m.Scope == ChatScope.Direct &&
                               ((m.Sender == user1 && m.Receiver == user2) ||
                                (m.Sender == user2 && m.Receiver == user1)))
                        .OrderByDescending(m => m.SendTime).Take(limit)
                        .OrderBy(m => m.SendTime).ToListAsync();

        public async Task<IReadOnlyList<Message>> GetGroupAsync(string groupName, int limit = 200)
            => await _db.Messages
                        .Where(m => m.Scope == ChatScope.Group && m.GroupName == groupName)
                        .OrderByDescending(m => m.SendTime).Take(limit)
                        .OrderBy(m => m.SendTime).ToListAsync();
    }
}
