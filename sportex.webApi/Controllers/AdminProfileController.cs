using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
using Sportex.Application.DTOs.Users;
using Sportex.Application.Interfaces;

namespace Sportex.WebApi.Controllers.Admin;

[ApiController]
[Route("api/admin/profile")]
[Authorize(Roles = "Admin")]
public class AdminProfileController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminProfileController(IUserService userService)
    {
        _userService = userService;
    }

    // 🔹 GET ADMIN PROFILE
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        int adminId = int.Parse(User.FindFirst("uid")!.Value);
        var profile = await _userService.GetProfileAsync(adminId);

        return Ok(ApiResponse.Success("Admin profile fetched", profile));
    }

    // 🔹 UPDATE ADMIN PROFILE
    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateAdminProfileDto dto)
    {
        int adminId = int.Parse(User.FindFirst("uid")!.Value);

        await _userService.UpdateProfileAsync(adminId, new UpdateProfileDto
        {
            Name = dto.Name,
            Phone = dto.Phone,
            Bio = dto.Bio
        });

        var updatedProfile = await _userService.GetProfileAsync(adminId);

        return Ok(ApiResponse.Success("Admin profile updated", updatedProfile));
    }
}
