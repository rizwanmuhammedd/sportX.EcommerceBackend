namespace Sportex.Application.DTOs.Users
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";

        // 👇 ADD THIS
        public string? ProfileImageUrl { get; set; }
    }
}
