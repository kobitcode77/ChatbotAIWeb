using ChatbotAI_BE.Data;
using ChatbotAI_BE.Dtos;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotAI_BE.Controllers
{
    [ApiController]
    [Authorize] // Đảm bảo chỉ người dùng đăng nhập mới gọi được
    [Route("api/[controller]")]
    public class ChatController : BaseController
    {
        private readonly AIService _ai;
        private readonly ChatDBContext _db;
        private readonly IChatService _chatService;

        public ChatController(AIService ai, ChatDBContext db, IChatService chatService)
        {
            _ai = ai;
            _db = db;
            _chatService = chatService;
        }

        // 🧠 Hỏi AI
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var reply = await _ai.AskAIAsync(userId, request.Message, request.Model, request.SessionId);

            return Success(new { reply }, "Nhận phản hồi từ AI thành công.");
        }

        // 💬 Lấy danh sách session
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var sessions = await _chatService.GetSessionsAsync(userId);

            var result = sessions.Select(s => new
            {
                s.Id,
                s.Model,
                s.Title,
                s.CreatedAt,
                s.LastUpdatedAt
            });

            return Success(result, "Lấy danh sách phiên trò chuyện thành công.");
        }

        // 🧾 Lấy danh sách tin nhắn trong 1 session
        [HttpGet("messages/{sessionId}")]
        public async Task<IActionResult> GetMessages(Guid sessionId)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var messages = await _chatService.GetMessagesAsync(sessionId, userId);

            var result = messages.Select(m => new
            {
                m.Id,
                m.Role,
                m.Content,
                m.SentAt
            });

            return Success(result, "Lấy danh sách tin nhắn thành công.");
        }

        // 🪄 Tạo session mới
        [HttpPost("sessions")]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var session = await _chatService.CreateSessionAsync(userId, request);

            var result = new
            {
                session.Id,
                session.Model,
                session.Title,
                session.CreatedAt,
                session.LastUpdatedAt
            };

            return Success(result, "Tạo phiên trò chuyện mới thành công.");
        }

        // ❌ Xóa session
        [HttpDelete("sessions/{sessionId}")]
        public async Task<IActionResult> DeleteSession(Guid sessionId)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            await _chatService.DeleteSessionAsync(sessionId, userId);

            return Success<object>(null, "Xóa phiên trò chuyện thành công.");
        }

        // ❌ Xóa message
        [HttpDelete("messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            await _chatService.DeleteMessageAsync(messageId, userId);

            return Success<object>(null, "Xóa tin nhắn thành công.");
        }
    }
}
