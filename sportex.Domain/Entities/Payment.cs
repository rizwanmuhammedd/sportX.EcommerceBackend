using Sportex.Domain.Common;

public class Payment : BaseEntity
{
    public int UserId { get; set; }
    public string RazorpayOrderId { get; set; } = "";
    public string RazorpayPaymentId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";   // Pending | Paid | Failed
}
