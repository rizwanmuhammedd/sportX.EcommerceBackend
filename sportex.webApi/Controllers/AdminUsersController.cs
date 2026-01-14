using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
using Sportex.Application.Interfaces;
using Sportex.Domain.Enums;

[Authorize(Roles = nameof(UserRole.Admin))]
[ApiController]
[Route("api/admin/users")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _service;
    public AdminUsersController(IUserService service) => _service = service;

    // GET ALL USERS
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllUsersAsync();
        return Ok(ApiResponse.Success("Users fetched", data));
    }

    // GET ONLY BLOCKED USERS
    [HttpGet("blocked")]
    public async Task<IActionResult> GetBlockedUsers()
    {
        var data = await _service.GetBlockedUsersAsync();
        return Ok(ApiResponse.Success("Blocked users fetched", data));
    }

    // BLOCK / UNBLOCK USER
    [HttpPatch("{id}/toggle-block")]
    public async Task<IActionResult> ToggleBlock(int id)
    {
        await _service.ToggleBlockAsync(id);
        return Ok(ApiResponse.Success("User block status updated"));
    }

    // FORCE BLOCK
    [HttpPatch("{id}/block")]
    public async Task<IActionResult> Block(int id)
    {
        await _service.BlockUserAsync(id);
        return Ok(ApiResponse.Success("User blocked"));
    }

    // FORCE UNBLOCK
    [HttpPatch("{id}/unblock")]
    public async Task<IActionResult> Unblock(int id)
    {
        await _service.UnblockUserAsync(id);
        return Ok(ApiResponse.Success("User unblocked"));
    }

    // DELETE USER
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteUserAsync(id);
        return Ok(ApiResponse.Success("User deleted"));
    }

    // CHANGE USER ROLE
    [HttpPatch("{id}/role/{role}")]
    public async Task<IActionResult> ChangeRole(int id, UserRole role)
    {
        await _service.ChangeRoleAsync(id, role);
        return Ok(ApiResponse.Success("User role updated"));
    }
}
