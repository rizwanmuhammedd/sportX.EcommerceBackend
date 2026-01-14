using Sportex.Application.DTOs.Auth;

namespace Sportex.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto dto);
    Task<TokenResponseDto> LoginAsync(LoginDto dto);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);



    // 🔽 NEW AUTH FEATURES
    Task SendOtpAsync(string email);
    Task ForgotPasswordAsync(string email);
    Task ResetPasswordAsync(ResetPasswordDto dto);
}
