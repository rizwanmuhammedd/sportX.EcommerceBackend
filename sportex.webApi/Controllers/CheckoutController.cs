using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.DTOs.Checkout;
using Sportex.Application.Interfaces;

namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/checkout")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _service;
    public CheckoutController(ICheckoutService service) => _service = service;

    [HttpGet("preview/{userId}")]
    public async Task<IActionResult> Preview(int userId)
    {
        return Ok(await _service.PreviewAsync(userId));
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(ConfirmCheckoutDto dto)
    {
        await _service.ConfirmAsync(dto);
        return Ok("Order placed successfully");
    }
}
