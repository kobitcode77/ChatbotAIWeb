using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser?> GetUserByIdAsync(Guid userId);
        Task<List<AppUser>> GetAllUsersAsync();
        Task<AppUser?> GetUserByProviderAsync(string provider, string providerId);
        Task<AppUser?> GetUserByUsernameAsync(string username);
        Task<bool> UserExistsByUsernameAsync(string username);
        Task AddUserAsync(AppUser user);
        Task UpdateUserAsync(AppUser user);
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
                 .Include(u => u.Activities)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<List<AppUser>> GetAllUsersAsync()
        {
            return await _db.Users
                .Include(u => u.Sessions)
                .ThenInclude(s => s.Messages)
                  .Include(u => u.Activities)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<AppUser?> GetUserByProviderAsync(string provider, string providerId)
        {
            return await _db.Users.FirstOrDefaultAsync(
                u => u.Provider == provider && u.ProviderId == providerId);
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UserExistsByUsernameAsync(string username)
        {
            return await _db.Users.AnyAsync(u => u.Username == username);
        }

        public  Task AddUserAsync(AppUser user)
        {
             _db.Users.AddAsync(user);
            return Task.CompletedTask;

        }

        public Task UpdateUserAsync(AppUser user)
        {
            _db.Users.Update(user);
            return Task.CompletedTask;

        }
        public Task DeleteUserAsync(AppUser user)
        {
            _db.Users.Remove(user);
            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
