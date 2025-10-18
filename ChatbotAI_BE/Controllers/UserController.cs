using ChatbotAI_BE.Models;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotAI_BE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// Lấy danh sách tất cả người dùng
        [HttpGet]
        [Authorize(Roles = "Admin")] // Chỉ admin mới được lấy danh sách user
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            return Success(users, "Lấy danh sách người dùng thành công.");
        }

        /// Lấy thông tin chi tiết 1 người dùng
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return Fail("Không tìm thấy người dùng.", 404);

            return Success(user, "Lấy thông tin người dùng thành công.");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser([FromBody] AppUser newUser, CancellationToken cancellationToken)
        {
            if (newUser == null)
                return Fail("Dữ liệu không hợp lệ.", 400);

            if (string.IsNullOrWhiteSpace(newUser.Username) || string.IsNullOrWhiteSpace(newUser.Password))
                return Fail("Tên đăng nhập và mật khẩu là bắt buộc.", 400);

                var createdUser = await _userService.AddUserAsync(newUser, cancellationToken);
                return Success(createdUser, "Thêm người dùng thành công.");
        }

        /// Cập nhật thông tin người dùng
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AppUser updatedUser, CancellationToken cancellationToken)
        {
            if (id != updatedUser.Id)
                return Fail("ID không khớp.");

            // Nếu muốn: kiểm tra quyền, chỉ cho phép user tự sửa hoặc admin sửa
            var existingUser = await _userService.GetByIdAsync(id, cancellationToken);
            if (existingUser == null)
                return Fail("Không tìm thấy người dùng.", 404);

            await _userService.UpdateUserAsync(updatedUser, cancellationToken);
            return Success(updatedUser, "Cập nhật thông tin người dùng thành công.");
        }

        /// Xóa người dùng
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")] // Chỉ admin mới được xóa user
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            await _userService.DeleteUserAsync(id, cancellationToken);
            return Success<object>(null, "Xóa người dùng thành công.");
        }
    }
}
