namespace Sportex.Application.DTOs.Orders;

public class OrderPreviewItemDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
}
