using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;

[Authorize(Roles = "user")]
[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _service;
    public WishlistController(IWishlistService service) => _service = service;

    private int UserId => int.Parse(User.FindFirst("uid")!.Value);

    // ❤️ TOGGLE WISHLIST (Add / Remove)
    [HttpPost]
    public async Task<IActionResult> Toggle(AddToWishlistDto dto)
    {
        var added = await _service.ToggleAsync(UserId, dto.ProductId);

        return Ok(ApiResponse.Success(
            added ? "Added to wishlist" : "Removed from wishlist",
            new
            {
                productId = dto.ProductId,
                isWishlisted = added
            }
        ));
    }


    // GET MY WISHLIST
    [HttpGet]
    public async Task<IActionResult> MyWishlist()
    {
        var data = await _service.GetMyWishlistAsync(UserId);
        return Ok(ApiResponse.Success("Wishlist fetched", data));
    }

    // REMOVE BY WISHLIST ITEM ID (optional manual remove)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var removed = await _service.RemoveAsync(UserId, id);

        return Ok(ApiResponse.Success(
            removed ? "Removed successfully" : "Wishlist item not found"
        ));
    }
}
