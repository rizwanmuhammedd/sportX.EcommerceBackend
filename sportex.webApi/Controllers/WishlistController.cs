using Microsoft.AspNetCore.Mvc;
using Sportex.Application.DTOs.Wishlist;
using Sportex.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/wishlist")]
[Authorize]

public class WishlistController : ControllerBase
{
    private readonly IWishlistService _service;
    public WishlistController(IWishlistService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Add(AddToWishlistDto dto)
    {
        await _service.AddAsync(dto);
        return Ok("Added to wishlist");
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(int userId)
    {
        return Ok(await _service.GetAsync(userId));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        await _service.RemoveAsync(id);
        return Ok("Removed from wishlist");
    }
}
