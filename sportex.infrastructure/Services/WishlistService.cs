using Microsoft.EntityFrameworkCore;
using Sportex.Application.DTOs.Wishlist;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;

namespace Sportex.Infrastructure.Services;

public class WishlistService : IWishlistService
{
    private readonly SportexDbContext _context;

    public WishlistService(SportexDbContext context)
    {
        _context = context;
    }

    public async Task ToggleAsync(int userId, int productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

        if (product == null)
            throw new Exception("Product not found");

        var item = await _context.WishlistItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

        if (item == null)
        {
            _context.WishlistItems.Add(new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            });
        }
        else
        {
            _context.WishlistItems.Remove(new WishlistItem { Id = item.Id });
        }

        await _context.SaveChangesAsync();
    }


    public async Task AddAsync(int userId, int productId)
    {
        await ToggleAsync(userId, productId);
    }

    public async Task RemoveAsync(int userId, int productId)
    {
        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

        if (item != null)
        {
            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<WishlistItemDto>> GetAsync(int userId)
    {
        var wishlistItems = await _context.WishlistItems
            .Where(w => w.UserId == userId)
            .Join(_context.Products.Where(p => p.IsActive),
                w => w.ProductId,
                p => p.Id,
                (w, p) => new WishlistItemDto
                {
                    Id = w.Id,
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ImageUrl = p.ImageUrl ?? "/images/default-product.jpg",
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Category = p.Category.ToString()
                })
            .ToListAsync();

        return wishlistItems;
    }
}