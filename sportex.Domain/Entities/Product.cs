using Sportex.Domain.Common;
using Sportex.Domain.Enums;

namespace Sportex.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ProductCategory Category { get; set; }

    public string? ImageUrl { get; set; }   // 🔥 ADD THIS LINE
}
