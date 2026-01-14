namespace Sportex.Application.DTOs.Payment;

public class RazorpayVerifyDto
{
    public string RazorpayOrderId { get; set; } = "";
    public string RazorpayPaymentId { get; set; } = "";
    public string RazorpaySignature { get; set; } = "";
    public int OrderId { get; set; }
}
