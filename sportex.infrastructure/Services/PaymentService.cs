using Microsoft.Extensions.Configuration;
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

    private string GetRazorpayKey()
    {
        var key = _config["Razorpay:Key"];
        if (string.IsNullOrEmpty(key))
            throw new Exception("Razorpay Key is missing in appsettings.json");

        return key;
    }

    private string GetRazorpaySecret()
    {
        var secret = _config["Razorpay:Secret"];
        if (string.IsNullOrEmpty(secret))
            throw new Exception("Razorpay Secret is missing in appsettings.json");

        return secret;
    }

    public async Task<object> CreateOrder(int userId, decimal amount)
    {
        var client = new RazorpayClient(
            GetRazorpayKey(),
            GetRazorpaySecret()
        );

        var options = new Dictionary<string, object>
        {
            { "amount", (int)(amount * 100) }, // convert to paise
            { "currency", "INR" },
            { "receipt", $"rcpt_{Guid.NewGuid()}" },
            { "payment_capture", 1 }
        };

        var order = client.Order.Create(options);

        _context.Payments.Add(new Payment
        {
            UserId = userId,
            RazorpayOrderId = order["id"].ToString(),
            Amount = amount,
            Status = "Created"
        });

        await _context.SaveChangesAsync();

        return new
        {
            orderId = order["id"].ToString(),
            amount = amount,
            key = GetRazorpayKey()
        };
    }

    public async Task VerifyPayment(int userId, RazorpayVerifyDto dto)
    {
        if (string.IsNullOrEmpty(dto.RazorpayOrderId) ||
            string.IsNullOrEmpty(dto.RazorpayPaymentId) ||
            string.IsNullOrEmpty(dto.RazorpaySignature))
        {
            throw new Exception("Missing payment details");
        }

        string secret = GetRazorpaySecret();

        string payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";

        string expectedSignature = Convert.ToHexString(
            new HMACSHA256(Encoding.UTF8.GetBytes(secret))
            .ComputeHash(Encoding.UTF8.GetBytes(payload))
        ).ToLower();

        if (expectedSignature != dto.RazorpaySignature)
            throw new Exception("Invalid Razorpay signature");

        var payment = await _context.Payments
            .FirstOrDefaultAsync(x =>
                x.RazorpayOrderId == dto.RazorpayOrderId &&
                x.UserId == userId);

        if (payment == null)
            throw new Exception("Payment record not found");

        payment.Status = "Paid";
        payment.RazorpayPaymentId = dto.RazorpayPaymentId;

        await _context.SaveChangesAsync();
    }
}
