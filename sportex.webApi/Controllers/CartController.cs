//using Microsoft.AspNetCore.Mvc;
//using Sportex.Application.DTOs.Cart;
//using Sportex.Application.Interfaces;
//using Microsoft.AspNetCore.Authorization;



//namespace Sportex.WebApi.Controllers;

//[ApiController]
//[Route("api/cart")]
//[Authorize]

//public class CartController : ControllerBase
//{
//    private readonly ICartService _service;
//    public CartController(ICartService service) => _service = service;

//    [HttpPost("add")]
//    public async Task<IActionResult> Add(AddToCartDto dto)
//    {
//        await _service.AddToCartAsync(dto);
//        return Ok("Added to Cart");
//    }

//    [HttpGet("{userId}")]
//    [Authorize(Roles ="user")]
//    public async Task<IActionResult> Get(int userId)
//    {
//        return Ok(await _service.GetCartAsync(userId));
//    }

//    [HttpDelete("{id}")]
//    public async Task<IActionResult> Remove(int id)
//    {
//        await _service.RemoveItemAsync(id);
//        return Ok("Removed");
//    }

//    [HttpDelete("clear/{userId}")]
//    public async Task<IActionResult> Clear(int userId)
//    {
//        await _service.ClearCartAsync(userId);
//        return Ok("Cleared");
//    }
//}
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

    // ADD TO CART
    [HttpPost("add")]
    public async Task<IActionResult> Add(AddToCartDto dto)
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);   // 🔥 FIXED
        await _service.AddToCartAsync(dto, userId);
        var cart = await _service.GetCartAsync(userId);

        return Ok(ApiResponse.Success("Item added to cart", cart));

    }

    // GET MY CART
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);   // 🔥 FIXED
        var data = await _service.GetCartAsync(userId);
        return Ok(ApiResponse.Success("Cart fetched", data));
    }

    // REMOVE ITEM
    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);   // 🔥 FIXED
        await _service.RemoveItemAsync(cartItemId, userId);
        var cart = await _service.GetCartAsync(userId);

        return Ok(ApiResponse.Success("Item removed", cart));

    }

    // CLEAR CART
    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);   // 🔥 FIXED
        await _service.ClearCartAsync(userId);
        return Ok(ApiResponse.Success("Cart cleared", new List<CartItemDto>()));

    }
}
