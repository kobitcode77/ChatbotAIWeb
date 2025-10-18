using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;

namespace ChatbotAI_BE.Services
{
    public interface IAIModelService
    {
        Task<List<AIModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<AIModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AIModel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<AIModel> CreateAsync(string code, string name, string? description, int maxTokens, int totalContext, CancellationToken cancellationToken = default);
        Task<AIModel> UpdateAsync(Guid id, string code, string name, string? description, bool isActive, int maxTokens, int totalContext, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class AIModelService : IAIModelService
    {
        private readonly IAIModelRepository _modelRepository;

        public AIModelService(IAIModelRepository modelRepository)
        {
            _modelRepository = modelRepository;
        }

        /// Lấy toàn bộ model AI
        public async Task<List<AIModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _modelRepository.GetAllModelsAsync(cancellationToken);
        }

        /// Lấy model AI theo Id
        public async Task<AIModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _modelRepository.GetModelByIdAsync(id, cancellationToken);
        }

        /// Lấy model AI theo Code
        public async Task<AIModel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _modelRepository.GetModelByCodeAsync(code, cancellationToken);
        }

        /// Tạo mới một model AI
        public async Task<AIModel> CreateAsync(string code, string name, string? description, int maxTokens, int totalContext, CancellationToken cancellationToken = default)
        {
            var exists = await _modelRepository.ModelExistsByCodeAsync(code, cancellationToken);
            if (exists)
                throw new InvalidOperationException($"Model với code '{code}' đã tồn tại.");

            var model = new AIModel
            {
                Code = code,
                Name = name,
                Description = description,
                MaxTokens = maxTokens,
                TotalContext = totalContext,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _modelRepository.AddModelAsync(model);
            await _modelRepository.SaveChangesAsync(cancellationToken);

            return model;
        }

        /// Cập nhật thông tin một model AI
        public async Task<AIModel> UpdateAsync(Guid id, string code,string name, string? description, bool isActive, int maxTokens, int totalContext, CancellationToken cancellationToken = default)
        {
            var model = await _modelRepository.GetModelByIdAsync(id, cancellationToken);
            if (model == null)
                throw new KeyNotFoundException("Không tìm thấy model.");

            model.Code = code;
            model.Name = name;
            model.Description = description;
            model.IsActive = isActive;
            model.MaxTokens = maxTokens;
            model.TotalContext = totalContext;
            model.UpdatedAt = DateTime.UtcNow;

            await _modelRepository.UpdateModelAsync(model);
            await _modelRepository.SaveChangesAsync(cancellationToken);

            return model;
        }

        /// Xóa một model AI
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var model = await _modelRepository.GetModelByIdAsync(id, cancellationToken);
            if (model == null)
                throw new KeyNotFoundException("Không tìm thấy model để xóa.");

            await _modelRepository.DeleteModelAsync(model);
            await _modelRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
