using Sportex.Application.DTOs.Auth;

namespace Sportex.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto dto);
     Task<TokenResponseDto> LoginAsync(LoginDto dto);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);

}
