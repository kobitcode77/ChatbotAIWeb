using ChatbotAI_BE.Dtos;
using ChatbotAI_BE.Enums;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotAI_BE.Controllers
{
    [ApiController]
    [Authorize] 
    [Route("api/[controller]")]
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

 
        /// Lấy toàn bộ tin nhắn trong một phiên chat
        [HttpGet("session/{sessionId:guid}")]
        public async Task<IActionResult> GetMessagesBySessionId(Guid sessionId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var messages = await _messageService.GetMessagesBySessionAndUserIdAsync(sessionId, userId, cancellationToken);
            return Success(messages, "Lấy danh sách tin nhắn thành công.");
        }

        /// Lấy chi tiết một tin nhắn theo ID
        [HttpGet("{messageId:guid}")]
        public async Task<IActionResult> GetMessageById(Guid messageId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.GetMessageByIdAndUserIdAsync(messageId, userId, cancellationToken);

            if (message == null)
                return Fail("Tin nhắn không tồn tại hoặc bạn không có quyền truy cập.", 404);

            return Success(message, "Lấy thông tin tin nhắn thành công.");
        }

        /// Gửi một tin nhắn mới trong phiên chat
        [HttpPost("session/{sessionId:guid}")]
        public async Task<IActionResult> SendMessage(Guid sessionId, [FromBody] SendMessageRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return Fail("Nội dung tin nhắn không được để trống.", 400);

            var userId = GetCurrentUserId();
            var message = await _messageService.SendMessageAsync(sessionId, userId, request.Content, request.Role, cancellationToken);

            return Success(message, "Gửi tin nhắn thành công.");
        }

        /// Xóa một tin nhắn của người dùng
        [HttpDelete("{messageId:guid}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _messageService.DeleteMessageAsync(messageId, userId, cancellationToken);
            return Success<object>(null, "Xóa tin nhắn thành công.");
        }
    }

}
