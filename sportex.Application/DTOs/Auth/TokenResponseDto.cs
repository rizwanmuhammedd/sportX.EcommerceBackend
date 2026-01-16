namespace Sportex.Application.DTOs.Auth;

public class TokenResponseDto
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";

    // 🔥 ADD THESE
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string? ProfileImageUrl { get; set; }
}
