using Microsoft.EntityFrameworkCore;
using Sportex.Application.DTOs.Cart;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;

namespace Sportex.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly SportexDbContext _context;
    public CartService(SportexDbContext context) => _context = context;

    public async Task AddToCartAsync(AddToCartDto dto)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.ProductId == dto.ProductId);

        if (item == null)
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }
        else
        {
            item.Quantity += dto.Quantity;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CartItemDto>> GetCartAsync(int userId)
    {
        return await _context.CartItems
            .Where(x => x.UserId == userId)
            .Select(x => new CartItemDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                Quantity = x.Quantity
            }).ToListAsync();
    }

    public async Task RemoveItemAsync(int id)
    {
        var item = await _context.CartItems.FindAsync(id);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(int userId)
    {
        var items = _context.CartItems.Where(x => x.UserId == userId);
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
