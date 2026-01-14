//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Sportex.Application.Common;
//using Sportex.Application.DTOs.Orders;
//using Sportex.Application.Interfaces;
//using System.Security.Claims;

//namespace Sportex.WebApi.Controllers;

//[ApiController]
//[Route("api/order")]
//public class OrderController : ControllerBase
//{
//    private readonly IOrderService _service;
//    public OrderController(IOrderService service) => _service = service;

//    private int UserId => int.Parse(User.FindFirst("uid")!.Value);

//    // ⚡ BUY NOW
//    [Authorize(Roles = "user")]
//    [HttpPost("Direct")]
//    public async Task<IActionResult> Direct(CreateDirectOrderDto dto)
//    {
//        var id = await _service.PlaceDirectOrderAsync(UserId, dto);
//        return Ok(ApiResponse.Success("Direct order placed", new { orderId = id }));
//    }

//    // 🛒 CART CHECKOUT
//    [Authorize(Roles = "user")]
//    [HttpPost("Add")]
//    public async Task<IActionResult> Add(CreateCartOrderDto dto)
//    {
//        var id = await _service.PlaceOrderAsync(UserId, dto);
//        return Ok(ApiResponse.Success("Order placed successfully", new { orderId = id }));
//    }

//    // USER GET MY ORDERS
//    [Authorize(Roles = "user")]
//    [HttpGet("MyOrders")]
//    public async Task<IActionResult> MyOrders()
//    {
//        var data = await _service.GetMyOrdersAsync(UserId);
//        return Ok(ApiResponse.Success("My orders", data));
//    }

//    // USER GET ORDER BY ID
//    [Authorize(Roles = "user")]
//    [HttpGet("GetBy_{id}")]
//    public async Task<IActionResult> GetBy(int id)
//    {
//        var order = await _service.GetOrderByIdAsync(UserId, id);
//        if (order == null) return NotFound(ApiResponse.Fail(404, "Order not found"));
//        return Ok(ApiResponse.Success("Order details", order));
//    }

//    // USER PAY
//    [Authorize(Roles = "user")]
//    [HttpPost("PayBy_{orderId}")]
//    public async Task<IActionResult> Pay(int orderId)
//    {
//        await _service.PayAsync(UserId, orderId);
//        return Ok(ApiResponse.Success("Payment successful"));
//    }

//    // ADMIN TOGGLE STATUS
//    [Authorize(Roles = "Admin")]
//    [HttpPatch("Admin/toggle/OrderStatus")]
//    public async Task<IActionResult> Toggle(int orderId)
//    {
//        await _service.ToggleStatusAsync(orderId);
//        return Ok(ApiResponse.Success("Order status updated"));
//    }

//    // ADMIN VIEW ALL
//    [Authorize(Roles = "Admin")]
//    [HttpGet("Admin/GetAll_Orders")]
//    public async Task<IActionResult> GetAll()
//    {
//        var data = await _service.GetAllOrdersAsync();
//        return Ok(ApiResponse.Success("All orders", data));
//    }
//}










using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
using Sportex.Application.DTOs.Orders;
using Sportex.Application.Interfaces;
using Sportex.Domain.Enums;
using System.Security.Claims;

namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;
    public OrderController(IOrderService service) => _service = service;

    private int UserId => int.Parse(User.FindFirst("uid")!.Value);

    // ⚡ BUY NOW
    [Authorize(Roles = "user")]
    [HttpPost("direct")]
    public async Task<IActionResult> Direct(CreateDirectOrderDto dto)
    {
        var id = await _service.PlaceDirectOrderAsync(UserId, dto);
        return Ok(ApiResponse.Success("Direct order placed", new { orderId = id }));
    }

    // 🛒 CART CHECKOUT
    [Authorize(Roles = "user")]
    [HttpPost("add")]
    public async Task<IActionResult> Add(CreateCartOrderDto dto)
    {
        var id = await _service.PlaceOrderAsync(UserId, dto);
        return Ok(ApiResponse.Success("Order placed successfully", new { orderId = id }));
    }

    // USER GET MY ORDERS
    [Authorize(Roles = "user")]
    [HttpGet("myorders")]
    public async Task<IActionResult> MyOrders()
    {
        var data = await _service.GetMyOrdersAsync(UserId);
        return Ok(ApiResponse.Success("My orders", data));
    }

    // USER GET ORDER BY ID
    [Authorize(Roles = "user")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBy(int id)
    {
        var order = await _service.GetOrderByIdAsync(UserId, id);
        if (order == null) return NotFound(ApiResponse.Fail(404, "Order not found"));
        return Ok(ApiResponse.Success("Order details", order));
    }

    // USER PAY
    [Authorize(Roles = "user")]
    [HttpPost("pay/{orderId}")]
    public async Task<IActionResult> Pay(int orderId)
    {
        await _service.PayAsync(UserId, orderId);
        return Ok(ApiResponse.Success("Payment successful"));
    }

    
    // ADMIN VIEW ALL
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllOrdersAsync();
        return Ok(ApiResponse.Success("All orders", data));
    }



    [Authorize(Roles = "user")]
    [HttpPatch("Cancel/{orderId}")]
    public async Task<IActionResult> Cancel(int orderId)
    {
        var result = await _service.CancelOrderAsync(UserId, orderId);
        return Ok(ApiResponse.Success("Order cancelled successfully", result));
    }


    // ---------------- ADMIN STATUS CONTROL ----------------

    // UPDATE ORDER STATUS
    [Authorize(Roles = "Admin")]
    [HttpPatch("admin/status/{orderId}")]
    public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
    {
        await _service.UpdateOrderStatusAsync(orderId, status);
        return Ok(ApiResponse.Success("Order status updated"));
    }

    // GET ORDERS BY STATUS
    [Authorize(Roles = "Admin")]
    [HttpGet("admin/status/{status}")]
    public async Task<IActionResult> GetByStatus(OrderStatus status)
    {
        var data = await _service.GetOrdersByStatusAsync(status);
        return Ok(ApiResponse.Success("Orders fetched", data));
    }


}
