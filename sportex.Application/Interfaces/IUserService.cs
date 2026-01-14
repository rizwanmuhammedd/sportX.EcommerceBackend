using sportex.Application.DTOs.Users;
using Sportex.Application.DTOs.Auth;
using Sportex.Application.DTOs.Users;
using Sportex.Domain.Enums;

namespace Sportex.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task ToggleBlockAsync(int userId);

    // ADMIN USER MANAGEMENT
    Task<List<UserDto>> GetBlockedUsersAsync();
    Task BlockUserAsync(int userId);
    Task UnblockUserAsync(int userId);
    Task DeleteUserAsync(int userId);
    Task ChangeRoleAsync(int userId, UserRole role);

    // USER PROFILE
    Task<UserProfileDto> GetProfileAsync(int userId);
    Task UpdateProfileAsync(int userId, UpdateProfileDto dto);
}
