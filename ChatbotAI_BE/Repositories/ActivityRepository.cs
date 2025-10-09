using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface IActivityRepository
    {
        Task<List<AIModelActivity>> GetAllActivitiesAsync();
        Task<List<AIModelActivity>> GetActivitiesByUserIdAsync(Guid userId);
        Task<List<AIModelActivity>> GetActivitiesByModelIdAsync(Guid modelId);
        Task<List<AIModelActivity>> GetActivitiesByUserAndModelAsync(Guid userId, Guid modelId);
        Task<AIModelActivity?> GetActivityByIdAsync(Guid id);
        Task AddActivityAsync(AIModelActivity activity);
        Task DeleteActivityAsync(AIModelActivity activity);
        Task SaveChangesAsync();

    }

    public class ActivityRepository: IActivityRepository
    {
        private readonly ChatDBContext _db;

        public ActivityRepository(ChatDBContext db)
        {
            _db = db;
        }


        public async Task<List<AIModelActivity>> GetAllActivitiesAsync()
        {
            return await _db.AIModelActivities
                .Include(a => a.User)
                .Include(a => a.Model)
                .OrderByDescending(a => a.UsedAt)
                .AsNoTracking()
                .ToListAsync();
        }

  
        public async Task<List<AIModelActivity>> GetActivitiesByUserIdAsync(Guid userId)
        {
            return await _db.AIModelActivities
                .Where(a => a.UserId == userId)
                .Include(a => a.Model)
                .OrderByDescending(a => a.UsedAt)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<List<AIModelActivity>> GetActivitiesByModelIdAsync(Guid modelId)
        {
            return await _db.AIModelActivities
                .Where(a => a.ModelId == modelId)
                .Include(a => a.User)
                .OrderByDescending(a => a.UsedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<AIModelActivity>> GetActivitiesByUserAndModelAsync(Guid userId, Guid modelId)
        {
            return await _db.AIModelActivities
                .Where(a => a.UserId == userId && a.ModelId == modelId)
                .OrderByDescending(a => a.UsedAt)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<AIModelActivity?> GetActivityByIdAsync(Guid id)
        {
            return await _db.AIModelActivities
                .Include(a => a.User)
                .Include(a => a.Model)
                .FirstOrDefaultAsync(a => a.Id == id);
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

 
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
