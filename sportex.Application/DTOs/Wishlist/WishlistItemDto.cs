// Sportex.Application/DTOs/Wishlist/WishlistItemDto.cs
namespace Sportex.Application.DTOs.Wishlist;

public class WishlistItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Category { get; set; }
}