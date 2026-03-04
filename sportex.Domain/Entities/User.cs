using Sportex.Domain.Common;

public class User : BaseEntity
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";

    // 🔥 ROLE STORED AS STRING (Admin / User)
    public string Role { get; set; } = "User";

    public bool isBlocked { get; set; } = false;

    public string PasswordHash { get; set; } = "";

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public string? Otp { get; set; }
    public DateTime? OtpExpiry { get; set; }

    // ✅ NEW PROFILE FIELDS
    public string? Phone { get; set; }
    public string? Bio { get; set; }

    public string? ProfileImageUrl { get; set; }
}
