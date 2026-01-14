using Sportex.Domain.Enums;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace Sportex.Application.DTOs.Products
{
    public class CreateProductDto
    {
        [DefaultValue("product")]

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(100)]
        [RegularExpression(
            @"^[A-Za-z]+( [A-Za-z]+)*$",
            ErrorMessage = "Name can contain only letters and single spaces between words"
        )]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100000)]
        public decimal Price { get; set; }

        [Range(0, 10000)]
        public int StockQuantity { get; set; }

        public ProductCategory Category { get; set; }

        public IFormFile Image { get; set; } = null!;
    }
}
