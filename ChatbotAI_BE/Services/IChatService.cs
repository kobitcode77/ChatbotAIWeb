using ChatbotAI_BE.Models;
using ChatbotAI_BE.Models;

namespace ChatbotAI_BE.Services
{
    public interface IChatService
    {
        Task<Message> SaveWorldAsync(string sender, string content);
        Task<Message> SaveDirectAsync(string sender, string receiver, string content);
        Task<Message> SaveGroupAsync(string sender, string groupName, string content);

        Task<IReadOnlyList<Message>> GetWorldAsync(int limit = 200);
        Task<IReadOnlyList<Message>> GetDirectAsync(string user1, string user2, int limit = 200);
        Task<IReadOnlyList<Message>> GetGroupAsync(string groupName, int limit = 200);
    }
}
