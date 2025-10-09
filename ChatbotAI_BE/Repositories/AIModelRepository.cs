using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface IAIModelRepository
    {
        Task<List<AIModel>> GetAllModelsAsync();
        Task<AIModel?> GetModelByIdAsync(Guid id);
        Task<AIModel?> GetModelByCodeAsync(string code);
        Task AddModelAsync(AIModel model);
        Task UpdateModelAsync(AIModel model);
        Task DeleteModelAsync(AIModel model);
        Task<bool> ModelExistsByCodeAsync(string code);
        Task SaveChangesAsync();

    }
    public class AIModelRepository : IAIModelRepository
    {
        private readonly ChatDBContext _db;

        public AIModelRepository(ChatDBContext db)
        {
            _db = db;
        }

    
        public async Task<List<AIModel>> GetAllModelsAsync()
        {
            return await _db.AIModels
                .OrderByDescending(m => m.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<AIModel?> GetModelByIdAsync(Guid id)
        {
            return await _db.AIModels
                .Include(m => m.Activities)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }


        public async Task<AIModel?> GetModelByCodeAsync(string code)
        {
            return await _db.AIModels
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Code == code);
        }


        public async Task<bool> ModelExistsByCodeAsync(string code)
        {
            return await _db.AIModels.AnyAsync(m => m.Code == code);
        }


        public Task AddModelAsync(AIModel model)
        {
            _db.AIModels.Add(model);
            return Task.CompletedTask;
        }


        public Task UpdateModelAsync(AIModel model)
        {
            _db.AIModels.Update(model);
            return Task.CompletedTask;
        }

        public Task DeleteModelAsync(AIModel model)
        {
            _db.AIModels.Remove(model);
            return Task.CompletedTask;
        }

   
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
