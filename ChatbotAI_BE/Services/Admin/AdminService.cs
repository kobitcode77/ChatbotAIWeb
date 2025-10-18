using ChatbotAI_BE.Dtos.Activity;
using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;
using System.Threading;

namespace ChatbotAI_BE.Services.Admin
{
    public interface IAdminService
    {
        // User
        Task<IEnumerable<AppUser>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<AppUser?> GetUserDetailAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> UpdateUserRoleAsync(Guid userId, string newRole, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);

        // Session & Message
        Task<IEnumerable<ChatSession>> GetAllSessionsAsync(CancellationToken cancellationToken = default);

        // AI Model
        Task<IEnumerable<AIModel>> GetAllModelsAsync(CancellationToken cancellationToken = default);
        Task<AIModel?> GetModelDetailAsync(Guid modelId, CancellationToken cancellationToken = default);
        Task<AIModel> CreateModelAsync(string code, string name, string? description, int maxTokens, int totalContext, CancellationToken cancellationToken = default);
        Task<bool> UpdateModelAsync(Guid modelId, string name, string? description, bool isActive, int maxTokens, int totalContext, CancellationToken cancellationToken = default);
        Task<bool> DeleteModelAsync(Guid modelId, CancellationToken cancellationToken = default);

        // Activity
        Task<IEnumerable<AIModelActivity>> GetAllActivitiesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<AIModelActivity>> GetActivitiesByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AIModelActivity>> GetActivitiesByModelAsync(Guid modelId, CancellationToken cancellationToken = default);
        Task<List<TokenUsageByMonthDto>> GetMonthlyTokenUsageAllUsersAsync(int year, CancellationToken cancellationToken = default);
    }

    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepo;
        private readonly ISessionRepository _sessionRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IAIModelRepository _modelRepo;
        private readonly IActivityRepository _activityRepo;

        public AdminService(
            IUserRepository userRepo,
            ISessionRepository sessionRepo,
            IMessageRepository messageRepo,
            IAIModelRepository modelRepo,
            IActivityRepository activityRepo)
        {
            _userRepo = userRepo;
            _sessionRepo = sessionRepo;
            _messageRepo = messageRepo;
            _modelRepo = modelRepo;
            _activityRepo = activityRepo;
        }

        #region USER
        public async Task<IEnumerable<AppUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepo.GetAllAsync(cancellationToken);
            if (!users.Any())
                throw new UserNotFoundException();

            return users;
        }

        public async Task<AppUser?> GetUserDetailAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new UserNotFoundException();

            return user;
        }

        public async Task<bool> UpdateUserRoleAsync(Guid userId, string newRole, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new UserNotFoundException();

            if (!Enum.TryParse<UserRole>(newRole, true, out var role))
                throw new InvalidRoleException();

            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new UserNotFoundException();

            try
            {
                // Lấy tất cả session của user
                var sessions = await _sessionRepo.GetByUserIdAsync(userId, cancellationToken);

                foreach (var session in sessions)
                {
                    // Lấy tin nhắn trong từng session
                    var messages = await _messageRepo.GetBySessionAndUserIdAsync(session.Id, userId, cancellationToken);
                    foreach (var message in messages)
                    {
                        await _messageRepo.DeleteAsync(message);
                    }
                    await _messageRepo.SaveChangesAsync(cancellationToken);

                    // Xóa session
                    await _sessionRepo.DeleteAsync(session);
                }
                await _sessionRepo.SaveChangesAsync(cancellationToken);

                // Cuối cùng xóa user
                await _userRepo.DeleteAsync(user);
                await _userRepo.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch
            {
                throw new DeleteUserFailedException();
            }
        }
        #endregion

        #region SESSION
        public async Task<IEnumerable<ChatSession>> GetAllSessionsAsync(CancellationToken cancellationToken = default)
        {
            var sessions = await _sessionRepo.GetAllAsync(cancellationToken);
            if (!sessions.Any())
                throw new SessionNotFoundException();

            return sessions;
        }
        #endregion

        #region AI MODEL
        public async Task<IEnumerable<AIModel>> GetAllModelsAsync(CancellationToken cancellationToken = default)
        {
            var models = await _modelRepo.GetAllModelsAsync(cancellationToken);
            return models;
        }

        public async Task<AIModel?> GetModelDetailAsync(Guid modelId, CancellationToken cancellationToken = default)
        {
            var model = await _modelRepo.GetModelByIdAsync(modelId, cancellationToken);
            if (model == null)
                throw new AIModelNotFoundException();

            return model;
        }

        public async Task<AIModel> CreateModelAsync(string code, string name, string? description, int maxTokens, int totalContext, CancellationToken cancellationToken = default)
        {
            if (await _modelRepo.ModelExistsByCodeAsync(code, cancellationToken))
                throw new ModelAlreadyExistsException();

            var model = new AIModel
            {
                Code = code,
                Name = name,
                Description = description,
                MaxTokens = maxTokens,
                TotalContext = totalContext,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _modelRepo.AddModelAsync(model);
            await _modelRepo.SaveChangesAsync(cancellationToken);

            return model;
        }

        public async Task<bool> UpdateModelAsync(Guid modelId, string name, string? description, bool isActive, int maxTokens, int totalContext, CancellationToken cancellationToken = default)
        {
            var model = await _modelRepo.GetModelByIdAsync(modelId, cancellationToken);
            if (model == null)
                throw new AIModelNotFoundException();

            model.Name = name;
            model.Description = description;
            model.IsActive = isActive;
            model.MaxTokens = maxTokens;
            model.TotalContext = totalContext;
            model.UpdatedAt = DateTime.UtcNow;

            await _modelRepo.UpdateModelAsync(model);
            await _modelRepo.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteModelAsync(Guid modelId, CancellationToken cancellationToken = default)
        {
            var model = await _modelRepo.GetModelByIdAsync(modelId, cancellationToken);
            if (model == null)
                throw new AIModelNotFoundException();

            await _modelRepo.DeleteModelAsync(model);
            await _modelRepo.SaveChangesAsync(cancellationToken);
            return true;
        }
        #endregion

        #region ACTIVITY
        public async Task<IEnumerable<AIModelActivity>> GetAllActivitiesAsync(CancellationToken cancellationToken = default)
        {
            return await _activityRepo.GetAllActivitiesAsync(cancellationToken);
        }

        public async Task<IEnumerable<AIModelActivity>> GetActivitiesByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _activityRepo.GetActivitiesByUserIdAsync(userId, cancellationToken);
        }

        public async Task<IEnumerable<AIModelActivity>> GetActivitiesByModelAsync(Guid modelId, CancellationToken cancellationToken = default)
        {
            return await _activityRepo.GetActivitiesByModelIdAsync(modelId, cancellationToken);
        }

        public async Task<List<TokenUsageByMonthDto>> GetMonthlyTokenUsageAllUsersAsync(int year, CancellationToken cancellationToken = default)
        {
            return await _activityRepo.GetMonthlyTokenUsageAllUsersAsync(year, cancellationToken);
        }
        #endregion
    }
}
