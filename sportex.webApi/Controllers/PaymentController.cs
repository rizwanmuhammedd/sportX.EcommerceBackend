//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Razorpay.Api;
//using Sportex.Application.DTOs.Payment;
//using Sportex.WebApi.Helpers;


//namespace Sportex.WebApi.Controllers;

//[Authorize]
//[ApiController]
//[Route("api/payment")]
//public class PaymentController : ControllerBase
//{
//    private readonly IConfiguration _config;

//    public PaymentController(IConfiguration config)
//    {
//        _config = config;
//    }

//    // CREATE ORDER
//    [HttpPost("create")]
//    public IActionResult CreateOrder([FromQuery] decimal amount)
//    {
//        var key = _config["Razorpay:Key"];
//        var secret = _config["Razorpay:Secret"];

//        var client = new RazorpayClient(key, secret);

//        var options = new Dictionary<string, object>
//        {
//            { "amount", amount * 100 },
//            { "currency", "INR" },
//            { "receipt", Guid.NewGuid().ToString() }
//        };

//        Razorpay.Api.Order order = client.Order.Create(options);

//        return Ok(new
//        {
//            orderId = order["id"].ToString(),
//            amount = amount,
//            key = key
//        });
//    }

//    // VERIFY PAYMENT



//    [HttpPost("verify")]
//    public IActionResult VerifyPayment([FromBody] RazorpayVerifyDto dto)
//    {
//        var secret = _config["Razorpay:Secret"];

//        string payload = dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;
//        string generatedSignature = RazorpaySignatureHelper.Generate(payload, secret);

//        if (generatedSignature != dto.RazorpaySignature)
//            return BadRequest(new { status = "Signature verification failed" });

//        return Ok(new { status = "Payment verified successfully" });
//    }



//}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using Sportex.Application.DTOs.Payment;
using Sportex.Application.Interfaces;
using Sportex.Domain.Enums;
using Sportex.Infrastructure.Data;
using Sportex.WebApi.Helpers;

namespace Sportex.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly SportexDbContext _context;
    private readonly IConfiguration _config;
    private readonly IOrderService _orderService;

    public PaymentController(
        SportexDbContext context,
        IConfiguration config,
        IOrderService orderService)
    {
        _context = context;
        _config = config;
        _orderService = orderService;
    }


    // CREATE ORDER
    //[HttpPost("create/{orderId}")]
    //public async Task<IActionResult> Create(int orderId)
    //{
    //    int userId = int.Parse(User.FindFirst("uid")!.Value);

    //    var order = await _context.Orders
    //        .Where(x => x.Id == orderId && x.UserId == userId)
    //        .Select(x => new { x.Id, x.TotalAmount })
    //        .FirstOrDefaultAsync();

    //    if (order == null) return NotFound("Order not found");

    //    var client = new RazorpayClient(
    //        _config["Razorpay:Key"],
    //        _config["Razorpay:Secret"]
    //    );

    //    var options = new Dictionary<string, object>
    //    {
    //        { "amount", (int)(order.TotalAmount * 100) },
    //        { "currency", "INR" },
    //        { "receipt", $"order_{order.Id}" }
    //    };

    //    var razorpayOrder = client.Order.Create(options);

    //    return Ok(new
    //    {
    //        razorpayOrderId = razorpayOrder["id"].ToString(),
    //        orderId = order.Id,
    //        amount = order.TotalAmount,
    //        key = _config["Razorpay:Key"]
    //    });
    //}



    [HttpPost("create/{orderId}")]
    public IActionResult Create(int orderId)
    {
        var order = _context.Orders.Find(orderId);
        if (order == null) return NotFound("Order not found");

        var client = new RazorpayClient(
            _config["Razorpay:Key"],
            _config["Razorpay:Secret"]
        );

        var options = new Dictionary<string, object>
    {
        { "amount", (int)(order.TotalAmount * 100) }, // paise
        { "currency", "INR" },
        { "receipt", $"order_{order.Id}" },
        { "payment_capture", 1 }
    };

        var razorpayOrder = client.Order.Create(options);

        return Ok(new
        {
            razorpayOrderId = razorpayOrder["id"].ToString(),
            amount = order.TotalAmount,
            key = _config["Razorpay:Key"]
        });
    }

    // VERIFY
    //[AllowAnonymous]
    //[HttpPost("verify")]
    //public async Task<IActionResult> Verify(RazorpayVerifyDto dto)
    //{
    //    string payload = dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

    //    bool isValid = RazorpaySignatureHelper.Verify(
    //        payload,
    //        dto.RazorpaySignature,
    //        _config["Razorpay:Secret"]
    //    );

    //    if (!isValid)
    //        return BadRequest("Invalid payment signature");

    //    // 🔥 CONFIRM PAYMENT (THIS DEDUCTS STOCK + MARKS ORDER PAID)
    //    await _orderService.ConfirmPaymentAsync(dto.OrderId);

    //    return Ok("Payment successful");
    //}

    [AllowAnonymous]
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] RazorpayVerifyDto dto)
    {
        // 1️⃣ Validate required fields
        if (string.IsNullOrEmpty(dto.RazorpayOrderId) ||
            string.IsNullOrEmpty(dto.RazorpayPaymentId) ||
            string.IsNullOrEmpty(dto.RazorpaySignature))
        {
            return BadRequest("Missing payment details");
        }

        // 2️⃣ Create payload exactly as Razorpay expects
        string payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";

        // 3️⃣ Verify signature
        bool isValid = RazorpaySignatureHelper.Verify(
            payload,
            dto.RazorpaySignature,
            _config["Razorpay:Secret"]
        );

        if (!isValid)
            return Unauthorized("Invalid Razorpay signature");

        // 4️⃣ Confirm payment (deduct stock + mark paid)
        await _orderService.ConfirmPaymentAsync(dto.OrderId);

        return Ok(new
        {
            message = "Payment verified successfully",
            orderId = dto.OrderId
        });
    }


}
