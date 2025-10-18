using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;

namespace ChatbotAI_BE.Services
{
    public interface IUserService
    {
        Task<AppUser> RegisterAsync(string username, string password, string? email, string? name, CancellationToken cancellationToken = default);
        Task<AppUser> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<AppUser> SaveOrUpdateUserByProviderAsync(string provider, string providerId, string? email, string? name, string? avatar, CancellationToken cancellationToken = default);
        Task<AppUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<AppUser>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<AppUser> AddUserAsync(AppUser user, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(AppUser user, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<AppUser> RegisterAsync(string username, string password, string? email, string? name, CancellationToken cancellationToken = default)
        {
            if (await _userRepo.ExistsByUsernameAsync(username, cancellationToken))
                throw new UserAlreadyExistsException();

            var user = new AppUser
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                Name = name,
                Provider = "Local",
                ProviderId = username,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<AppUser> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByUsernameAsync(username, cancellationToken);
            if (user == null)
                throw new UserNotFoundException();

            if (user.Password == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new IncorrectPasswordException();

            return user;
        }

        public async Task<AppUser> SaveOrUpdateUserByProviderAsync(string provider, string providerId, string? email, string? name, string? avatar, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByProviderAsync(provider, providerId, cancellationToken);

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

            await _userRepo.SaveChangesAsync(cancellationToken);
            return user;
        }

        public Task<AppUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return _userRepo.GetByIdAsync(userId, cancellationToken);
        }

        public Task<List<AppUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return _userRepo.GetAllAsync(cancellationToken);
        }
        public async Task<AppUser> AddUserAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            // Kiểm tra username đã tồn tại
            if (await _userRepo.ExistsByUsernameAsync(user.Username, cancellationToken))
                throw new UserAlreadyExistsException();

            // Kiểm tra role có hợp lệ không
            if (!Enum.TryParse<UserRole>(user.Role.ToString(), true, out var userRole))
                throw new InvalidRoleException();

            // Tạo object user mới để đảm bảo dữ liệu sạch và có đầy đủ thông tin
            var newUser = new AppUser
            {
                Username = user.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Email = user.Email,
                Name = user.Name,
                Role = userRole,
                Provider = "Local",
                ProviderId = user.Username,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(newUser);
            await _userRepo.SaveChangesAsync(cancellationToken);
            return newUser;
        }

        public async Task UpdateUserAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new UserNotFoundException();

            await _userRepo.DeleteAsync(user);
            await _userRepo.SaveChangesAsync(cancellationToken);
        }


    }

}
