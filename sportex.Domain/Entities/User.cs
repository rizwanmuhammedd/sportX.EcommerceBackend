using Sportex.Domain.Common;

namespace Sportex.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = "";      // 🔥 ADD THIS
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

}
