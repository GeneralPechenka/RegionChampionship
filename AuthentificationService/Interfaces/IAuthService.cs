using DTOs.Authentification;

namespace AuthentificationService.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken token);
        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken token);
        Task<RefreshTokenDto> RefreshTokenAsync(string token, CancellationToken cancellation);
        Task LogoutAsync(string token, CancellationToken cancellation);
    }
}
