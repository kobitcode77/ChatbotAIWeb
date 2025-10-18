using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<AppUser>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<AppUser?> GetByProviderAsync(string provider, string providerId, CancellationToken cancellationToken = default);
        Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task AddAsync(AppUser user);
        Task UpdateAsync(AppUser user);
        Task DeleteAsync(AppUser user);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ChatDBContext _db;

        public UserRepository(ChatDBContext db)
        {
            _db = db;
        }

        public async Task<AppUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.Users
                .Include(u => u.Sessions)
                    .ThenInclude(s => s.Messages)
                .Include(u => u.Activities)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<List<AppUser>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Users
                .Include(u => u.Sessions)
                    .ThenInclude(s => s.Messages)
                .Include(u => u.Activities)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<AppUser?> GetByProviderAsync(string provider, string providerId, CancellationToken cancellationToken = default)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Provider == provider && u.ProviderId == providerId, cancellationToken);
        }

        public async Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _db.Users
                .AnyAsync(u => u.Username == username, cancellationToken);
        }

        public Task AddAsync(AppUser user)
        {
            _db.Users.Add(user);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(AppUser user)
        {
            _db.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(AppUser user)
        {
            _db.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
