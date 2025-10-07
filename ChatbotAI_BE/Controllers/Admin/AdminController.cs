using ChatbotAI_BE.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotAI_BE.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Success(users, "Lấy danh sách người dùng thành công.");
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserDetail(Guid userId)
        {
            var user = await _adminService.GetUserDetailAsync(userId);
            return Success(user, "Lấy chi tiết người dùng thành công.");
        }

        [HttpPut("users/{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid userId, [FromBody] string newRole)
        {
            await _adminService.UpdateUserRoleAsync(userId, newRole);
            return Success<object>(null, "Cập nhật vai trò người dùng thành công.");
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            await _adminService.DeleteUserAsync(userId);
            return Success<object>(null, "Xóa người dùng và toàn bộ dữ liệu chat thành công.");
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> GetAllSessions()
        {
            var sessions = await _adminService.GetAllSessionsAsync();
            return Success(sessions, "Lấy danh sách phiên trò chuyện thành công.");
        }
    }
}
