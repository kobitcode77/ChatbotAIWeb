using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser?> GetUserByIdAsync(Guid userId);
        Task<List<AppUser>> GetAllUsersAsync();
        Task<AppUser?> GetByProviderAsync(string provider, string providerId);
        Task<AppUser?> GetByUsernameAsync(string username);
        Task<bool> ExistsByUsernameAsync(string username);
        Task AddAsync(AppUser user);
        Task UpdateAsync(AppUser user);
        Task DeleteUserAsync(AppUser user);
        Task SaveChangesAsync();
    }

    public class UserRepository : IUserRepository
    {
        private readonly ChatDBContext _db;

        public UserRepository(ChatDBContext db)
        {
            _db = db;
        }
        public async Task<AppUser?> GetUserByIdAsync(Guid userId)
        {
            return await _db.Users
                .Include(u => u.Sessions)
                .ThenInclude(s => s.Messages)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<List<AppUser>> GetAllUsersAsync()
        {
            return await _db.Users
                .Include(u => u.Sessions)
                .ThenInclude(s => s.Messages)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<AppUser?> GetByProviderAsync(string provider, string providerId)
        {
            return await _db.Users.FirstOrDefaultAsync(
                u => u.Provider == provider && u.ProviderId == providerId);
        }

        public async Task<AppUser?> GetByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _db.Users.AnyAsync(u => u.Username == username);
        }

        public async Task AddAsync(AppUser user)
        {
            await _db.Users.AddAsync(user);
        }

        public async Task UpdateAsync(AppUser user)
        {
            _db.Users.Update(user);
        }
        public async Task DeleteUserAsync(AppUser user)
        {
            _db.Users.Remove(user);
            await Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
