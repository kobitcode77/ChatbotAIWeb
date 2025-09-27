using ChatbotAI_BE.Models;

namespace ChatbotAI_BE.Services
{
    public interface IUserService
    {
        Task<AppUser?> GetUserByProviderAsync(string provider, string providerId);
        Task<AppUser> SaveOrUpdateUserByProviderAsync(string provider, string providerId, string email, string name, string avatar);
        Task<AppUser?> RegisterAsync(string username, string password, string? email, string? name);
        Task<AppUser?> LoginAsync(string username, string password);
    }
}
