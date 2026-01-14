using Sportex.Application.DTOs.Payment;

public interface IPaymentService
{
    Task<object> CreateOrder(int userId, decimal amount);
    Task VerifyPayment(int userId, RazorpayVerifyDto dto);
}
