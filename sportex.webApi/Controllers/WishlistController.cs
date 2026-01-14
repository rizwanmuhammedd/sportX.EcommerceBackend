using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
using Sportex.Application.Interfaces;
using System.Security.Claims;

namespace Sportex.webApi.Controllers;

[Authorize(Roles = "user")]
[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _service;

    public WishlistController(IWishlistService service) => _service = service;

    private int UserId =>
     int.Parse(User.FindFirst("uid")!.Value);


    // ❤️ TOGGLE WISHLIST (ADD / REMOVE)
    [HttpPost("{productId}")]
    public async Task<IActionResult> Toggle(int productId)
    {
        try
        {
            await _service.ToggleAsync(UserId, productId);
            return Ok(ApiResponse.Success("Wishlist updated"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }

    // GET MY WISHLIST
    [HttpGet]
    public async Task<IActionResult> MyWishlist()
    {
        try
        {
            var wishlistItems = await _service.GetAsync(UserId);
            return Ok(ApiResponse.Success("Wishlist retrieved", wishlistItems));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }

    // REMOVE FROM WISHLIST
    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(int productId)
    {
        try
        {
            await _service.RemoveAsync(UserId, productId);
            return Ok(ApiResponse.Success("Removed from wishlist"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
    }
}


