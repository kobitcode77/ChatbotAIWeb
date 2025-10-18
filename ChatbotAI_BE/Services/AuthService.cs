using ChatbotAI_BE.Dtos.Auth;
using System.Security.Claims;

namespace ChatbotAI_BE.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginWithGoogleAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken = default);
        Task<AuthResponse> LoginWithFacebookAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken = default);
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }

    public class AuthService:IAuthService
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwt;

        public AuthService(IUserService userService, JwtService jwt)
        {
            _userService = userService;
            _jwt = jwt;
        }

        public async Task<AuthResponse> LoginWithGoogleAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            var provider = "Google";
            var providerId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value
                          ?? claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;

            if (string.IsNullOrEmpty(providerId))
                throw new Exception("Không tìm thấy ID nhà cung cấp");

            var user = await _userService.SaveOrUpdateUserByProviderAsync(provider, providerId, email, name, avatar, cancellationToken);
            var token = _jwt.GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task<AuthResponse> LoginWithFacebookAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            var provider = "Facebook";
            var providerId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            if (string.IsNullOrEmpty(providerId))
                throw new Exception("Không tìm thấy ID nhà cung cấp");

            var user = await _userService.SaveOrUpdateUserByProviderAsync(provider, providerId, email, name, avatar, cancellationToken);
            var token = _jwt.GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userService.RegisterAsync(request.Username, request.Password, request.Email, request.Name, cancellationToken);
            var token = _jwt.GenerateJwtToken(user);
            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userService.LoginAsync(request.Username, request.Password, cancellationToken);
            var token = _jwt.GenerateJwtToken(user);
            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name
            };
        }
    }
}
