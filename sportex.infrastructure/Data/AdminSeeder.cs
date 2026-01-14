using BCrypt.Net;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;

public static class AdminSeeder
{
    public static void SeedAdmin(SportexDbContext context)
    {
        if (!context.Users.Any(u => u.Role == "Admin"))
        {
            var admin = new User
            {
                Name = "Super Admin",
                Email = "risvan@sportx.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Risvan@123"),
                Role = "Admin",
                isBlocked = false,
                CreatedOn = DateTime.UtcNow
            };

            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}
