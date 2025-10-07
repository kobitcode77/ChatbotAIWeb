using System.Net;
using ChatbotAI_BE.Dtos;
using ChatbotAI_BE.Exceptions;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        string message = "Đã xảy ra lỗi không mong muốn.";
        List<string>? errors = new();

        switch (ex)
        {
            case UserAlreadyExistsException:
                statusCode = HttpStatusCode.Conflict;
                message = "Người dùng đã tồn tại.";
                errors.Add(nameof(UserAlreadyExistsException));
                break;
            case UserNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = "Không tìm thấy người dùng.";
                errors.Add(nameof(UserNotFoundException));
                break;
            case IncorrectPasswordException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "Mật khẩu không chính xác.";
                errors.Add(nameof(IncorrectPasswordException));
                break;
            case SessionNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = "Không tìm thấy phiên trò chuyện.";
                errors.Add(nameof(SessionNotFoundException));
                break;
            case MessageNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = "Không tìm thấy tin nhắn.";
                errors.Add(nameof(MessageNotFoundException));
                break;
            case InvalidRoleException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Vai trò người dùng không hợp lệ.";
                errors.Add(nameof(InvalidRoleException));
                break;
            case DeleteUserFailedException:
                statusCode = HttpStatusCode.InternalServerError;
                message = "Xóa người dùng thất bại.";
                errors.Add(nameof(DeleteUserFailedException));
                break;
            default:
                errors.Add(ex.GetType().Name);
                break;
        }

        var response = ApiResponse<object>.ErrorResponse(message, (int)statusCode, errors);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}
