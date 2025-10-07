using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;

namespace ChatbotAI_BE.Services
{
    public interface IUserService
    {
        Task<AppUser> SaveOrUpdateUserByProviderAsync(string provider, string providerId, string email, string name, string avatar);
        Task<AppUser> RegisterAsync(string username, string password, string? email, string? name);
        Task<AppUser> LoginAsync(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<AppUser> SaveOrUpdateUserByProviderAsync(string provider, string providerId, string email, string name, string avatar)
        {
            try
            {
                var user = await _userRepo.GetByProviderAsync(provider, providerId);

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
                    await _userRepo.AddAsync(user);
                }
                else
                {
                    user.Email = email ?? user.Email;
                    user.Name = name ?? user.Name;
                    user.Avatar = avatar ?? user.Avatar;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepo.UpdateAsync(user);
                }

                await _userRepo.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<AppUser> RegisterAsync(string username, string password, string? email, string? name)
        {
            if (await _userRepo.ExistsByUsernameAsync(username))
                throw new UserAlreadyExistsException();
            try
            {
                var user = new AppUser
                {
                    Username = username,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Email = email,
                    Name = name,
                    Provider = "Local",
                    ProviderId = username
                };

                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<AppUser> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userRepo.GetByUsernameAsync(username);
                if (user == null)
                    throw new UserNotFoundException();

                if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                    throw new IncorrectPasswordException();

                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
