using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
using Sportex.Application.DTOs.Cart;
using Sportex.Application.Interfaces;
using System.Security.Claims;

namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "user")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;
    public CartController(ICartService service) => _service = service;

    // ADD TO CART - Now supports negative quantities for decreasing
    [HttpPost("add")]
    public async Task<IActionResult> Add(AddToCartDto dto)
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);

        var before = await _service.GetCartAsync(userId);
        var existingBefore = before.FirstOrDefault(x => x.ProductId == dto.ProductId);

        await _service.AddToCartAsync(dto, userId);

        var cart = await _service.GetCartAsync(userId);
        var existingAfter = cart.FirstOrDefault(x => x.ProductId == dto.ProductId);

        string message = existingBefore == null
            ? "Item added to cart"
            : $"Cart updated: quantity set to {existingAfter?.Quantity ?? 0} 🛒";

        return Ok(ApiResponse.Success(message, cart));
    }

    // NEW ENDPOINT: Update cart quantity (for +1/-1 operations)
    [HttpPost("update-quantity")]
    public async Task<IActionResult> UpdateQuantity(UpdateCartQuantityDto dto)
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);

        await _service.UpdateCartQuantityAsync(dto, userId);

        var cart = await _service.GetCartAsync(userId);

        string message = dto.QuantityChange > 0
            ? $"Increased quantity by {dto.QuantityChange}"
            : $"Decreased quantity by {Math.Abs(dto.QuantityChange)}";

        return Ok(ApiResponse.Success(message, cart));
    }

    // GET MY CART
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);
        var data = await _service.GetCartAsync(userId);
        return Ok(ApiResponse.Success("Cart fetched", data));
    }

    // REMOVE ITEM
    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);
        await _service.RemoveItemAsync(cartItemId, userId);
        var cart = await _service.GetCartAsync(userId);

        return Ok(ApiResponse.Success("Item removed", cart));
    }

    // CLEAR CART
    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);
        await _service.ClearCartAsync(userId);
        return Ok(ApiResponse.Success("Cart cleared", new List<CartItemDto>()));
    }
}