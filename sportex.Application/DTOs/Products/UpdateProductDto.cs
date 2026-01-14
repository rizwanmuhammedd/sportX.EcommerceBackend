using Sportex.Domain.Enums;

namespace Sportex.Application.DTOs.Products;

public class UpdateProductDto
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ProductCategory Category { get; set; }
}
