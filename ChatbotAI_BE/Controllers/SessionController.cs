using ChatbotAI_BE.Dtos.Session;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotAI_BE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SessionController : BaseController
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// Lấy toàn bộ session (Chỉ Admin)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSessions(CancellationToken cancellationToken)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(cancellationToken);
            return Success(sessions, "Lấy danh sách tất cả session thành công.");
        }

        /// Lấy danh sách session của chính người dùng
        [HttpGet("me")]
        public async Task<IActionResult> GetMySessions(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var sessions = await _sessionService.GetUserSessionsAsync(userId, cancellationToken);
            return Success(sessions, "Lấy danh sách session của bạn thành công.");
        }

        /// Lấy chi tiết một session
        [HttpGet("{sessionId:guid}")]
        public async Task<IActionResult> GetSessionById(Guid sessionId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var session = await _sessionService.GetSessionByIdAsync(sessionId, userId, cancellationToken);
            if (session == null)
                return Fail("Không tìm thấy session hoặc bạn không có quyền truy cập.", 404);

            return Success(session, "Lấy thông tin session thành công.");
        }

        /// Tạo session mới
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request, CancellationToken cancellationToken)
        {
            if (request == null || request.ModelId == Guid.Empty)
                return Fail("Thiếu thông tin ModelId.");

            var userId = GetCurrentUserId();
            var session = await _sessionService.CreateSessionAsync(userId, request.ModelId, request.Title, cancellationToken);
            return Success(session, "Tạo session thành công.");
        }

        /// Xóa một session
        [HttpDelete("{sessionId:guid}")]
        public async Task<IActionResult> DeleteSession(Guid sessionId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            await _sessionService.DeleteSessionAsync(sessionId, userId, cancellationToken);
            return Success<object>(null, "Xóa session thành công.");
        }
    }

}
