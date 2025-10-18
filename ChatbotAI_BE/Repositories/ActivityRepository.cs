using ChatbotAI_BE.Data;
using ChatbotAI_BE.Dtos.Activity;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ChatbotAI_BE.Repositories
{
    public interface IActivityRepository
    {
        Task<List<AIModelActivity>> GetAllActivitiesAsync(CancellationToken cancellationToken = default);
        Task<List<AIModelActivity>> GetActivitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<AIModelActivity>> GetActivitiesByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default);
        Task<List<AIModelActivity>> GetActivitiesByUserAndModelAsync(Guid userId, Guid modelId, CancellationToken cancellationToken = default);
        Task<AIModelActivity?> GetActivityByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddActivityAsync(AIModelActivity activity);           
        Task DeleteActivityAsync(AIModelActivity activity);         
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        // Thống kê
        Task<List<TokenUsageByDayDto>> GetDailyTokenUsageByUserAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<List<TokenUsageByMonthDto>> GetMonthlyTokenUsageAllUsersAsync(int year, CancellationToken cancellationToken = default);
    }

    public class ActivityRepository : IActivityRepository
    {
        private readonly ChatDBContext _db;

        public ActivityRepository(ChatDBContext db)
        {
            _db = db;
        }

        public async Task<List<AIModelActivity>> GetAllActivitiesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.AIModelActivities
                .Include(a => a.User)
                .Include(a => a.Model)
                .OrderByDescending(a => a.UsedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AIModelActivity>> GetActivitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.AIModelActivities
                .Where(a => a.UserId == userId)
                .Include(a => a.Model)
                .OrderByDescending(a => a.UsedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AIModelActivity>> GetActivitiesByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default)
        {
            return await _db.AIModelActivities
                .Where(a => a.ModelId == modelId)
                .Include(a => a.User)
                .OrderByDescending(a => a.UsedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AIModelActivity>> GetActivitiesByUserAndModelAsync(Guid userId, Guid modelId, CancellationToken cancellationToken = default)
        {
            return await _db.AIModelActivities
                .Where(a => a.UserId == userId && a.ModelId == modelId)
                .OrderByDescending(a => a.UsedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<AIModelActivity?> GetActivityByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.AIModelActivities
                .Include(a => a.User)
                .Include(a => a.Model)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public Task AddActivityAsync(AIModelActivity activity)
        {
            _db.AIModelActivities.Add(activity);
            return Task.CompletedTask;
        }

        public Task DeleteActivityAsync(AIModelActivity activity)
        {
            _db.AIModelActivities.Remove(activity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        // Thống kê token theo ngày cho 1 user
        public async Task<List<TokenUsageByDayDto>> GetDailyTokenUsageByUserAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _db.AIModelActivities
                .Where(a => a.UserId == userId && a.UsedAt.Date >= startDate.Date && a.UsedAt.Date <= endDate.Date)
                .GroupBy(a => new { a.Model.Code, Date = a.UsedAt.Date })
                .Select(g => new TokenUsageByDayDto
                {
                    ModelCode = g.Key.Code,
                    Date = g.Key.Date,
                    TotalTokens = g.Sum(a => a.TotalTokens)
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);
        }

        // Thống kê token theo tháng của tất cả người dùng
        public async Task<List<TokenUsageByMonthDto>> GetMonthlyTokenUsageAllUsersAsync(int year, CancellationToken cancellationToken = default)
        {
            return await _db.AIModelActivities
                .Where(a => a.UsedAt.Year == year)
                .GroupBy(a => new { a.Model.Code, a.UsedAt.Year, a.UsedAt.Month })
                .Select(g => new TokenUsageByMonthDto
                {
                    ModelCode = g.Key.Code, 
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalTokens = g.Sum(a => a.TotalTokens)
                })
                .OrderBy(x => x.Month)
                .ToListAsync(cancellationToken);
        }
    }
}

