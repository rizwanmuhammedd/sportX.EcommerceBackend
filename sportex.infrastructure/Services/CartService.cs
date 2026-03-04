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

    // ADD TO CART - Updated to handle negative quantities properly
    public async Task AddToCartAsync(AddToCartDto dto, int userId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.ProductId);
        if (product == null)
            throw new Exception("Product not found");

        // REMOVED: if (dto.Quantity < 1) throw new Exception("Minimum quantity is 1");
        // Now allowing negative quantities for decreasing

        var item = await _context.CartItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == dto.ProductId);

        if (item == null)
        {
            // For adding NEW item, quantity should be >= 1
            if (dto.Quantity < 1)
                throw new Exception("Cannot add new item with quantity less than 1");

            _context.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }
        else
        {
            int newQty = item.Quantity + dto.Quantity;

            // If quantity becomes 0 or negative, remove the item
            if (newQty <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else if (newQty > product.StockQuantity)
            {
                throw new Exception($"You already have {item.Quantity}. Only {product.StockQuantity} in stock");
            }
            else
            {
                item.Quantity = newQty;
            }
        }

        await _context.SaveChangesAsync();
    }

    // NEW METHOD: Update cart quantity with positive/negative change
    public async Task UpdateCartQuantityAsync(UpdateCartQuantityDto dto, int userId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.ProductId);
        if (product == null)
            throw new Exception("Product not found");

        var item = await _context.CartItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == dto.ProductId);

        if (item == null)
        {
            throw new Exception("Item not found in cart");
        }

        int newQty = item.Quantity + dto.QuantityChange;

        // If quantity becomes 0 or negative, remove the item
        if (newQty <= 0)
        {
            _context.CartItems.Remove(item);
        }
        else if (newQty > product.StockQuantity)
        {
            throw new Exception($"Cannot increase quantity beyond available stock. Available: {product.StockQuantity}");
        }
        else
        {
            item.Quantity = newQty;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CartItemDto>> GetCartAsync(int userId)
    {
        return await _context.CartItems
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedOn) // ✅ LATEST FIRST
            .Join(_context.Products,
                c => c.ProductId,
                p => p.Id,
                (c, p) => new CartItemDto
                {
                    Id = c.Id,
                    ProductId = p.Id,
                    ProductName = p.Name!,
                    ImageUrl = p.ImageUrl!,
                    Price = p.Price,
                    Quantity = c.Quantity,
                    Stock = p.StockQuantity
                })
            .ToListAsync();
    }


    // DELETE SINGLE ITEM (Secure)
    public async Task RemoveItemAsync(int cartItemId, int userId)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(x => x.Id == cartItemId && x.UserId == userId);

        if (item == null)
            throw new Exception("Unauthorized delete attempt");

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    // CLEAR CART
    public async Task ClearCartAsync(int userId)
    {
        var items = _context.CartItems.Where(x => x.UserId == userId);
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}