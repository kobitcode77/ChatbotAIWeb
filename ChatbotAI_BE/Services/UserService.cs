using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Services
{
    public class UserService : IUserService
    {
        private readonly ChatDBContext _chatDB;

        public UserService(ChatDBContext chatDB)
        {
            _chatDB = chatDB;
        }
        public async Task<AppUser?> GetUserByProviderAsync(string provider, string providerId)
        {
            return await _chatDB.Users.FirstOrDefaultAsync(
                u => u.Provider == provider && u.ProviderId == providerId
            );
        }

        public async Task<AppUser> SaveOrUpdateUserByProviderAsync(string provider, string providerId, string email, string name, string avatar)
        {
            var user = await GetUserByProviderAsync(provider, providerId);

            if (user == null)
            {
                user = new AppUser
                {
                    Provider = provider,
                    ProviderId = providerId,
                    Email = email,
                    Name = name,
                    Avatar = avatar,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _chatDB.Users.Add(user);
            }
            else
            {
                user.Email = email ?? user.Email;
                user.Name = name ?? user.Name;
                user.Avatar = avatar ?? user.Avatar;
                user.UpdatedAt = DateTime.UtcNow;
                _chatDB.Users.Update(user);
            }

            await _chatDB.SaveChangesAsync();
            return user;
        }


        public async Task<AppUser?> RegisterAsync(string username, string password, string? email, string? name)
        {
            if (await _chatDB.Users.AnyAsync(u => u.Username == username))
            {
                throw new UserAlreadyExistsException();
            }
            var user = new AppUser
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                Name = name,
                Provider = "Local",
                ProviderId = username
            };
            _chatDB.Users.Add(user);
            await _chatDB.SaveChangesAsync();

            return user;
        }

        public async Task<AppUser?> LoginAsync(string username, string password)
        {
            var user = await _chatDB.Users.FirstOrDefaultAsync(u => u.Username == username && u.Provider == "Local");
            if (user == null)
                throw new UserNotFoundException();
            else if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new IncorrectPasswordException();
            return user;
        }
    }

}

