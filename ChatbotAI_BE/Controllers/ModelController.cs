using ChatbotAI_BE.Dtos.AIModel;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotAI_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ModelController : BaseController
    {
        private readonly IAIModelService _aiModelService;

        public ModelController(IAIModelService aiModelService)
        {
            _aiModelService = aiModelService;
        }

        /// Lấy danh sách tất cả model AI (ai cũng có thể truy cập sau khi login)
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var models = await _aiModelService.GetAllAsync(cancellationToken);
            return Success(models, "Lấy danh sách model AI thành công.");
        }

        /// Lấy chi tiết một model theo ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var model = await _aiModelService.GetByIdAsync(id, cancellationToken);
            if (model == null)
                return Fail("Không tìm thấy model.", 404);

            return Success(model, "Lấy thông tin model thành công.");
        }

        /// Lấy model theo Code (mã model)
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken)
        {
            var model = await _aiModelService.GetByCodeAsync(code, cancellationToken);
            if (model == null)
                return Fail($"Không tìm thấy model với code '{code}'.", 404);

            return Success(model, "Lấy thông tin model thành công.");
        }

        /// Tạo mới một model AI (chỉ Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateModelRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
                return Fail("Code và Tên model là bắt buộc.", 400);

            var model = await _aiModelService.CreateAsync(
                request.Code,
                request.Name,
                request.Description,
                request.MaxTokens,
                request.TotalContext,
                cancellationToken
            );

            return Success(model, "Tạo mới model thành công.", 201);
        }

        /// Cập nhật thông tin model AI (chỉ Admin)
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateModelRequest request, CancellationToken cancellationToken)
        {
            var model = await _aiModelService.UpdateAsync(
                id,
                request.Code,
                request.Name,
                request.Description,
                request.IsActive,
                request.MaxTokens,
                request.TotalContext,
                cancellationToken
            );

            return Success(model, "Cập nhật model thành công.");
        }

        /// Xóa một model AI (chỉ Admin)
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _aiModelService.DeleteAsync(id, cancellationToken);
            return Success<object>(null, "Xóa model thành công.");
        }
    }


}
