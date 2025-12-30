using Microsoft.AspNetCore.Mvc;
using Sportex.Application.DTOs.Orders;
using Sportex.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]

public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    public OrdersController(IOrderService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(CreateOrderDto dto)
    {
        await _service.PlaceOrderAsync(dto);
        return Ok("Order placed successfully");
    }
}
