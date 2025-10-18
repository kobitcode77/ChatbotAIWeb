using ChatbotAI_BE.Dtos.Auth;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatbotAI_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        public AuthController(IConfiguration config, IAuthService authService)
        {
            _config = config;
            _authService = authService;
        }
        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle([FromQuery] string? returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback), new { returnUrl })
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl, CancellationToken cancellation = default)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
                return Fail("Xác thực Google thất bại.");

            var response = await _authService.LoginWithGoogleAsync(result.Principal.Claims, cancellation);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Success(response, "Đăng nhập Google thành công.");
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
        public async Task<IActionResult> FacebookCallback([FromQuery] string? returnUrl, CancellationToken cancellation = default)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
                return Fail("Xác thực Facebook thất bại.");

            var response = await _authService.LoginWithFacebookAsync(result.Principal.Claims, cancellation);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Success(response, "Đăng nhập Facebook thành công.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellation = default)
        {
            var response = await _authService.RegisterAsync(request, cancellation);
            return Success(response, "Đăng ký tài khoản thành công.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellation = default)
        {
            var response = await _authService.LoginAsync(request, cancellation);
            return Success(response, "Đăng nhập thành công.");
        }
    }
}