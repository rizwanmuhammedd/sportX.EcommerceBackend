using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class WishlistService : IWishlistService
{
    private readonly SportexDbContext _context;
    public WishlistService(SportexDbContext context) => _context = context;

    public async Task<bool> ToggleAsync(int userId, int productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
        if (product == null)
            throw new Exception("Product not found");

        var existing = await _context.WishlistItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

        if (existing != null)
        {
            _context.WishlistItems.Remove(existing);
            await _context.SaveChangesAsync();
            return false; // REMOVED
        }

        _context.WishlistItems.Add(new WishlistItem
        {
            UserId = userId,
            ProductId = productId
        });

        await _context.SaveChangesAsync();
        return true; // ADDED
    }

    public async Task<List<WishlistItemDto>> GetMyWishlistAsync(int userId)
    {
        return await _context.WishlistItems
            .Where(x => x.UserId == userId)
            .Join(_context.Products, w => w.ProductId, p => p.Id,
            (w, p) => new WishlistItemDto
            {
                Id = w.Id,
                ProductId = p.Id,
                ProductName = p.Name,
                ImageUrl = p.ImageUrl
            }).ToListAsync();
    }

    public async Task<bool> RemoveAsync(int userId, int wishlistId)
    {
        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(x => x.Id == wishlistId && x.UserId == userId);

        if (item == null) return false;

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}
