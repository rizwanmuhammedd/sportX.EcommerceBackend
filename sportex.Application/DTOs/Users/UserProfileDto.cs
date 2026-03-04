namespace Sportex.Application.DTOs.Users;

public class UserProfileDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Email { get; set; } = "";

    public string? Phone { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }
}
