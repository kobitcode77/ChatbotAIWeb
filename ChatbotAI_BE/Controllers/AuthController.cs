using ChatbotAI_BE.Data;
using ChatbotAI_BE.Dtos;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatbotAI_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly ChatDBContext _chatDB;
        private readonly UserService _userService;
        private readonly JwtService _jwt;
        public AuthController(IConfiguration config, ChatDBContext chatDB, UserService userService, JwtService jwt)
        {
            _config = config;
            _chatDB = chatDB;
            _userService = userService;
            _jwt = jwt;
        }
        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle([FromQuery] string? returnUrl)
        {
            // redirect back to this API endpoint after Google auth handled by cookie middleware
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback), new { returnUrl })
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl)
        {
            // Authenticate using the external cookie (set by AddGoogle -> AddCookie)
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

           if (!result.Succeeded || result.Principal == null)
                return Fail("Xác thực Google thất bại.", 400);

            var claims = result.Principal.Claims;
            var provider = "Google";
            var providerId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value
                          ?? claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;

           if (string.IsNullOrEmpty(providerId))
                return Fail("Không tìm thấy ID nhà cung cấp.", 400);

            // Save or update user in DB
            var user = await _userService.SaveOrUpdateUserByProviderAsync(provider, providerId, email, name, avatar);

            // generate JWT
            var token = _jwt.GenerateJwtToken(user);

            // Option A: redirect to frontend with token in query (commonly used when flow started from browser)
            var frontEndUri = _config["App:FrontEndRedirectUri"];
            var separator = frontEndUri.Contains("?") ? "&" : "?";
            var redirect = $"{frontEndUri}{separator}token={token}";

            // Sign out the external cookie (clean up)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Success(new { token }, "Đăng nhập Google thành công.");
            //return Redirect(redirect);
        }

        [HttpGet("login-facebook")]
        public IActionResult LoginWithFacebook([FromQuery] string? returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(FacebookCallback), new { returnUrl })
            };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet("facebook-callback")]
        public async Task<IActionResult> FacebookCallback([FromQuery] string? returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
                return Fail("Xác thực Facebook thất bại.", 400);

            var claims = result.Principal.Claims;

            var provider = "Facebook";
            var providerId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            if (string.IsNullOrEmpty(providerId))
                return Fail("Không tìm thấy ID nhà cung cấp.", 400);

            // Lưu hoặc cập nhật user trong DB
            var user = await _userService.SaveOrUpdateUserByProviderAsync(provider, providerId, email, name, avatar);

            // Tạo JWT token
            var token = _jwt.GenerateJwtToken(user);

            // Trả token cho frontend
            var frontEndUri = _config["App:FrontEndRedirectUri"];
            var separator = frontEndUri.Contains("?") ? "&" : "?";
            var redirect = $"{frontEndUri}{separator}token={token}";

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Success(new { token }, "Đăng nhập Facebook thành công.");
            //return Redirect(redirect);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _userService.RegisterAsync(request.Username, request.Password, request.Email, request.Name);
            var token = _jwt.GenerateJwtToken(user);
            return Success(new
            {
                token,
                user.Username,
                user.Email,
                user.Name
            }, "Đăng ký tài khoản thành công.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.LoginAsync(request.Username, request.Password);

            var token = _jwt.GenerateJwtToken(user);
            return Success(new
            {
                token,
                user.Username,
                user.Email,
                user.Name
            }, "Đăng nhập thành công.");
        }


        // Example protected endpoint using JWT
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userId == null)
                return Fail("Người dùng chưa được xác thực.", 401);

            var user = await _chatDB.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
                return Fail("Không tìm thấy người dùng.", 404);

            return Success(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Avatar,
                user.Provider
            }, "Lấy thông tin người dùng thành công.");
        }

    }
}