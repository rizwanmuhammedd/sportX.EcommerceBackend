using Microsoft.EntityFrameworkCore;
using Sportex.Application.DTOs.Wishlist;
using Sportex.Application.Interfaces;
using Sportex.Infrastructure.Data;
using Sportex.Domain.Entities;

namespace Sportex.Infrastructure.Services;

public class WishlistService : IWishlistService
{
    private readonly SportexDbContext _context;
    public WishlistService(SportexDbContext context) => _context = context;

    public async Task AddAsync(AddToWishlistDto dto)
    {
        if (!await _context.WishlistItems.AnyAsync(x => x.UserId == dto.UserId && x.ProductId == dto.ProductId))
        {
            _context.WishlistItems.Add(new WishlistItem
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<WishlistItemDto>> GetAsync(int userId)
    {
        return await _context.WishlistItems
            .Where(x => x.UserId == userId)
            .Join(_context.Products, w => w.ProductId, p => p.Id, (w, p) => new WishlistItemDto
            {
                Id = w.Id,
                ProductId = p.Id,
                ProductName = p.Name,
                ImageUrl = p.ImageUrl
            }).ToListAsync();
    }

    public async Task RemoveAsync(int id)
    {
        var item = await _context.WishlistItems.FindAsync(id);
        if (item != null)
        {
            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
