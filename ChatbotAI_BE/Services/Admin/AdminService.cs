using ChatbotAI_BE.Data;
using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Services.Admin
{
    public interface IAdminService
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser?> GetUserDetailAsync(Guid userId);
        Task<bool> UpdateUserRoleAsync(Guid userId, string newRole);
        Task<IEnumerable<ChatSession>> GetAllSessionsAsync();
        Task<bool> DeleteUserAsync(Guid userId);

    }
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepo;
        private readonly ISessionRepository _sessionRepo;

        public AdminService(IUserRepository userRepo, ISessionRepository sessionRepo)
        {
            _userRepo = userRepo;
            _sessionRepo = sessionRepo;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllUsersAsync();

            if (!users.Any())
                throw new UserNotFoundException();

            return users;
        }

        public async Task<AppUser?> GetUserDetailAsync(Guid userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null)
                throw new UserNotFoundException();

            return user;
        }

        public async Task<bool> UpdateUserRoleAsync(Guid userId, string newRole)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();

            if (!Enum.TryParse<UserRole>(newRole, true, out var role))
                throw new InvalidRoleException();

            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();

            try
            {
                // Xóa thủ công tin nhắn và phiên
                var messages = user.Sessions.SelectMany(s => s.Messages).ToList();
                if (messages.Any())
                    user.Sessions.ForEach(s => s.Messages.Clear());

                user.Sessions.Clear();

                await _userRepo.DeleteUserAsync(user);
                await _userRepo.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw new DeleteUserFailedException();
            }
        }

        public async Task<IEnumerable<ChatSession>> GetAllSessionsAsync()
        {
            var sessions = await _sessionRepo.GetAllSessionsAsync();

            if (!sessions.Any())
                throw new SessionNotFoundException();

            return sessions;
        }
    }
}