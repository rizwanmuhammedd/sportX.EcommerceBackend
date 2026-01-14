using Microsoft.EntityFrameworkCore;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;

namespace Sportex.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly SportexDbContext _context;

    public CartRepository(SportexDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CartItem>> GetByUserAsync(int userId)
    {
        return await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CartItem?> GetAsync(int userId, int productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
    }

    public async Task AddAsync(CartItem item)
    {
        _context.CartItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task ClearAsync(int userId)
    {
        var items = _context.CartItems.Where(x => x.UserId == userId);
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
