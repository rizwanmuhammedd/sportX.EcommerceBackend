namespace Sportex.Application.DTOs.Orders
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";   // ✅ REQUIRED
        public string? ImageUrl { get; set; }           // ✅ REQUIRED
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }          // ✅ REQUIRED
    }
}
