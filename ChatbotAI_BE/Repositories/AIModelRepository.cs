using ChatbotAI_BE.Data;
using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Repositories
{
    public interface IAIModelRepository
    {
        Task<List<AIModel>> GetAllModelsAsync(CancellationToken cancellationToken = default);
        Task<AIModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AIModel?> GetModelByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task AddModelAsync(AIModel model);
        Task UpdateModelAsync(AIModel model);
        Task DeleteModelAsync(AIModel model);
        Task<bool> ModelExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class AIModelRepository : IAIModelRepository
    {
        private readonly ChatDBContext _db;

        public AIModelRepository(ChatDBContext db)
        {
            _db = db;
        }

        public async Task<List<AIModel>> GetAllModelsAsync(CancellationToken cancellationToken = default)
        {
            return await _db.AIModels
                .OrderByDescending(m => m.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<AIModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.AIModels
                .Include(m => m.Activities)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<AIModel?> GetModelByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _db.AIModels
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Code == code, cancellationToken);
        }

        public async Task<bool> ModelExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _db.AIModels
                .AnyAsync(m => m.Code == code, cancellationToken);
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

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
