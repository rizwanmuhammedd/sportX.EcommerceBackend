using Microsoft.Extensions.Configuration;
using Razorpay;
using Razorpay.Api;
using Sportex.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Sportex.Application.DTOs.Payment;





public class PaymentService : IPaymentService
{
    private readonly SportexDbContext _context;
    private readonly IConfiguration _config;

    public PaymentService(SportexDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<object> CreateOrder(int userId, decimal amount)
    {
        var client = new RazorpayClient(
            _config["Razorpay:Key"],
            _config["Razorpay:Secret"]
        );

        var options = new Dictionary<string, object>
        {
            { "amount", amount * 100 },
            { "currency", "INR" },
            { "receipt", Guid.NewGuid().ToString() }
        };

        var order = client.Order.Create(options);

        _context.Payments.Add(new Payment
        {
            UserId = userId,
            RazorpayOrderId = order["id"].ToString(),
            Amount = amount
        });

        await _context.SaveChangesAsync();

        return new { orderId = order["id"], amount };
    }

    public async Task VerifyPayment(int userId, RazorpayVerifyDto dto)
    {
        var secret = _config["Razorpay:Secret"];
        var payload = dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

        var expectedSignature = Convert.ToHexString(
            new HMACSHA256(Encoding.UTF8.GetBytes(secret))
            .ComputeHash(Encoding.UTF8.GetBytes(payload))
        ).ToLower();

        if (expectedSignature != dto.RazorpaySignature)
            throw new Exception("Invalid Payment");

        var payment = await _context.Payments
            .FirstAsync(x => x.RazorpayOrderId == dto.RazorpayOrderId && x.UserId == userId);

        payment.Status = "Paid";
        payment.RazorpayPaymentId = dto.RazorpayPaymentId;

        await _context.SaveChangesAsync();
    }
}
