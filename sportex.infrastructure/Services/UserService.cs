



using CloudinaryDotNet.Core;
using Microsoft.EntityFrameworkCore;
using Sportex.Application.DTOs.Auth;
using Sportex.Application.DTOs.Users;
using Sportex.Application.Interfaces;
using Sportex.Domain.Enums;
using Sportex.Infrastructure.Data;


namespace Sportex.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly SportexDbContext _context;
    public UserService(SportexDbContext context) => _context = context;

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            IsBlocked = u.isBlocked
        }).ToListAsync();
    }

    public async Task ToggleBlockAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        // 🚫 NEVER BLOCK ADMINS
        if (user.Role == nameof(UserRole.Admin))
            throw new Exception("Admin accounts cannot be blocked");

        // If admin is trying to BLOCK a customer
        if (!user.isBlocked)
        {
            bool hasOrders = await _context.Orders
                .AnyAsync(o => o.UserId == userId);

            if (hasOrders)
                throw new Exception("Cannot block a customer who has placed orders");
        }

        user.isBlocked = !user.isBlocked;
        await _context.SaveChangesAsync();
    }



    // ---------------- PROFILE ----------------
    public async Task<UserProfileDto> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        return new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Bio = user.Bio,
            AvatarUrl = user.ProfileImageUrl
        };
    }


    public async Task UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        user.Name = dto.Name.Trim();
        user.Phone = dto.Phone;
        user.Bio = dto.Bio;

        await _context.SaveChangesAsync();
    }


    // ---------------- ADMIN CONTROLS ----------------

    public async Task<List<UserDto>> GetBlockedUsersAsync()
    {
        return await _context.Users
            .Where(u => u.isBlocked)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                IsBlocked = u.isBlocked
            }).ToListAsync();
    }

    public async Task BlockUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        // 🚫 NEVER BLOCK ADMINS
        if (user.Role == nameof(UserRole.Admin))
            throw new Exception("Admin accounts cannot be blocked");

        bool hasOrders = await _context.Orders
            .AnyAsync(o => o.UserId == userId);

        if (hasOrders)
            throw new Exception("Cannot block a customer who has placed orders");

        user.isBlocked = true;
        await _context.SaveChangesAsync();
    }


    public async Task UnblockUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        user.isBlocked = false;
        await _context.SaveChangesAsync();
    }


    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task ChangeRoleAsync(int userId, UserRole role)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        user.Role = role.ToString();
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAvatarAsync(int userId, string imageUrl)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        user.ProfileImageUrl = imageUrl;
        await _context.SaveChangesAsync();
    }

    private async Task<bool> UserHasOrdersAsync(int userId)
    {
        return await _context.Orders
            .AnyAsync(o => o.UserId == userId);
    }

   


}
