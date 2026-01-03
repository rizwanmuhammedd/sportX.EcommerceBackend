using Microsoft.EntityFrameworkCore;

public class WishlistItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ImageUrl { get; set; }
}
