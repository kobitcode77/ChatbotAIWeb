using ChatbotAI_BE.Dtos;
using ChatbotAI_BE.Dtos.AI;
using ChatbotAI_BE.Dtos.Session;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotAI_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly IAIService _aiService;
        private readonly IAIModelService _modelService;
        private readonly ISessionRepository _sessionRepository;
        private readonly IMessageRepository _messageRepository;

        public ChatController(
            IAIService aiService,
            IAIModelService modelService,
            ISessionRepository sessionRepository,
            IMessageRepository messageRepository)
        {
            _aiService = aiService;
            _modelService = modelService;
            _sessionRepository = sessionRepository;
            _messageRepository = messageRepository;
        }

        /// Gửi tin nhắn đến AI và nhận phản hồi
        [HttpPost("ask")]
        public async Task<IActionResult> AskAI([FromBody] AskAIRequest request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            // Kiểm tra model có tồn tại không
            var model = await _modelService.GetByIdAsync(request.ModelId, cancellationToken);
            if (model == null)
                return Fail("Không tìm thấy model AI.", 404);

            // Gọi AI
            var aiReply = await _aiService.AskAIAsync(
                userId,
                request.UserMessage,
                model,
                request.SessionId,
                cancellationToken
            );

            return Success(new { reply = aiReply }, "Phản hồi từ AI thành công");
        }
    }
}
