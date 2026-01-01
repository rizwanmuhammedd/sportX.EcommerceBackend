using System.ComponentModel.DataAnnotations;
using Sportex.Domain.Enums;

namespace Sportex.Application.DTOs.Products
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100000)]
        public decimal Price { get; set; }

        [Range(0, 10000)]
        public int StockQuantity { get; set; }

        public ProductCategory Category { get; set; }

        public string? ImageUrl { get; set; }
    }
}
