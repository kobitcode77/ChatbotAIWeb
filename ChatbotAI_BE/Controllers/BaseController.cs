using ChatbotAI_BE.Dtos;
using Microsoft.AspNetCore.Mvc;

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
    }
}

