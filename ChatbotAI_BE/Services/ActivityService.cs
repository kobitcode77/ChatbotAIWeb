using ChatbotAI_BE.Dtos.Activity;
using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;

namespace ChatbotAI_BE.Services
{
    public interface IActivityService
    {
        Task<List<AIModelActivity>> GetAllActivitiesAsync(CancellationToken cancellationToken = default);
        Task<List<AIModelActivity>> GetActivitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<AIModelActivity>> GetActivitiesByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default);
        Task<List<AIModelActivity>> GetActivitiesByUserAndModelAsync(Guid userId, Guid modelId, CancellationToken cancellationToken = default);
        Task<AIModelActivity?> GetActivityByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AIModelActivity> AddActivityAsync(Guid userId, Guid modelId, int inputTokens, int outputTokens, CancellationToken cancellationToken = default);
        Task<bool> DeleteActivityAsync(Guid activityId, CancellationToken cancellationToken = default);

        // Thống kê token theo ngày của 1 user
        Task<List<TokenUsageByDayDto>> GetDailyTokenUsageByUserAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        // Thống kê token theo tháng của tất cả người dùng
        Task<List<TokenUsageByMonthDto>> GetMonthlyTokenUsageAllUsersAsync(int year, CancellationToken cancellationToken = default);
    }

    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IAIModelRepository _modelRepository;

        public ActivityService(IActivityRepository activityRepository, IAIModelRepository modelRepository)
        {
            _activityRepository = activityRepository;
            _modelRepository = modelRepository;
        }

        public async Task<List<AIModelActivity>> GetAllActivitiesAsync(CancellationToken cancellationToken = default)
        {
            return await _activityRepository.GetAllActivitiesAsync(cancellationToken);
        }

        public async Task<List<AIModelActivity>> GetActivitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _activityRepository.GetActivitiesByUserIdAsync(userId, cancellationToken);
        }

        public async Task<List<AIModelActivity>> GetActivitiesByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default)
        {
            return await _activityRepository.GetActivitiesByModelIdAsync(modelId, cancellationToken);
        }

        public async Task<List<AIModelActivity>> GetActivitiesByUserAndModelAsync(Guid userId, Guid modelId, CancellationToken cancellationToken = default)
        {
            return await _activityRepository.GetActivitiesByUserAndModelAsync(userId, modelId, cancellationToken);
        }

        public async Task<AIModelActivity?> GetActivityByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _activityRepository.GetActivityByIdAsync(id, cancellationToken);
        }

        public async Task<AIModelActivity> AddActivityAsync(Guid userId, Guid modelId, int inputTokens, int outputTokens, CancellationToken cancellationToken = default)
        {
            // kiểm tra model có tồn tại không
            var model = await _modelRepository.GetModelByIdAsync(modelId, cancellationToken);
            if (model == null)
                throw new AIModelNotFoundException();

            var activity = new AIModelActivity
            {
                UserId = userId,
                ModelId = modelId,
                InputTokens = inputTokens,
                OutputTokens = outputTokens,
                UsedAt = DateTime.UtcNow
            };

            await _activityRepository.AddActivityAsync(activity);
            await _activityRepository.SaveChangesAsync(cancellationToken);

            return activity;
        }

        public async Task<bool> DeleteActivityAsync(Guid activityId, CancellationToken cancellationToken = default)
        {
            var activity = await _activityRepository.GetActivityByIdAsync(activityId, cancellationToken);
            if (activity == null) return false;

            await _activityRepository.DeleteActivityAsync(activity);
            await _activityRepository.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<TokenUsageByDayDto>> GetDailyTokenUsageByUserAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _activityRepository.GetDailyTokenUsageByUserAsync(userId, startDate, endDate, cancellationToken);
        }

        public async Task<List<TokenUsageByMonthDto>> GetMonthlyTokenUsageAllUsersAsync(int year, CancellationToken cancellationToken = default)
        {
            return await _activityRepository.GetMonthlyTokenUsageAllUsersAsync(year, cancellationToken);
        }
    }
}
