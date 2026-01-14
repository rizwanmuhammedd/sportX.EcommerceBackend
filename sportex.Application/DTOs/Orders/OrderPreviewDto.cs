namespace Sportex.Application.DTOs.Orders
{

    public class OrderPreviewDto
    {
        public List<OrderPreviewItemDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}
