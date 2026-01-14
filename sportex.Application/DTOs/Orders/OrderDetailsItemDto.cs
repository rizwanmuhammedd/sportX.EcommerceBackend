namespace Sportex.Application.DTOs.Orders
{

    public class OrderDetailsItemDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}