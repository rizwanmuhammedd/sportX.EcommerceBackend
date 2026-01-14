namespace Sportex.Application.DTOs.Orders
{

    public class OrderDetailsDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public List<OrderDetailsItemDto> Items { get; set; } = new();
    }
}
