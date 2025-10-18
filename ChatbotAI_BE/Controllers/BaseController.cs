using ChatbotAI_BE.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatbotAI_BE.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Thành công", int statusCode = 200)
        {
            return StatusCode(statusCode, ApiResponse<T>.SuccessResponse(data, message, statusCode));
        }

        protected IActionResult Fail(string message, int statusCode = 400, IEnumerable<string>? errors = null)
        {
            return StatusCode(statusCode, ApiResponse<object>.ErrorResponse(message, statusCode, errors));
        }

        /// Lấy ID người dùng hiện tại từ JWT Token
        protected Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirst("id")
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("sub");

            if (idClaim == null || string.IsNullOrWhiteSpace(idClaim.Value))
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng trong token.");

            if (!Guid.TryParse(idClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Token không chứa ID người dùng hợp lệ.");

            return userId;
        }
    }
}

