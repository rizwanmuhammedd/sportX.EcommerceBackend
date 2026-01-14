//using Microsoft.EntityFrameworkCore;
//using Sportex.Application.DTOs.Cart;
//using Sportex.Application.Interfaces;
//using Sportex.Domain.Entities;
//using Sportex.Infrastructure.Data;

//namespace Sportex.Infrastructure.Services;

//public class CartService : ICartService
//{
//    private readonly SportexDbContext _context;
//    public CartService(SportexDbContext context) => _context = context;

//    public async Task AddToCartAsync(AddToCartDto dto)
//    {
//        var item = await _context.CartItems
//            .FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.ProductId == dto.ProductId);

//        if (item == null)
//        {
//            _context.CartItems.Add(new CartItem
//            {
//                UserId = dto.UserId,
//                ProductId = dto.ProductId,
//                Quantity = dto.Quantity
//            });
//        }
//        else
//        {
//            item.Quantity += dto.Quantity;
//        }

//        await _context.SaveChangesAsync();
//    }

//    public async Task<IEnumerable<CartItemDto>> GetCartAsync(int userId)
//    {
//        return await _context.CartItems
//            .Where(x => x.UserId == userId)
//            .Select(x => new CartItemDto
//            {
//                Id = x.Id,
//                ProductId = x.ProductId,
//                Quantity = x.Quantity
//            }).ToListAsync();
//    }

//    public async Task RemoveItemAsync(int id)
//    {
//        var item = await _context.CartItems.FindAsync(id);
//        if (item != null)
//        {
//            _context.CartItems.Remove(item);
//            await _context.SaveChangesAsync();
//        }
//    }

//    public async Task ClearCartAsync(int userId)
//    {
//        var items = _context.CartItems.Where(x => x.UserId == userId);
//        _context.CartItems.RemoveRange(items);
//        await _context.SaveChangesAsync();
//    }
//}



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

    // ADD TO CART
    public async Task AddToCartAsync(AddToCartDto dto, int userId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.ProductId);
        if (product == null)
            throw new Exception("Product not found");

        if (dto.Quantity < 1)
            throw new Exception("Minimum quantity is 1");

        if (dto.Quantity > product.StockQuantity)
            throw new Exception($"Only {product.StockQuantity} items left in stock");

        var item = await _context.CartItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == dto.ProductId);

        if (item == null)
        {
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

            if (newQty > product.StockQuantity)
                throw new Exception($"You already have {item.Quantity}. Only {product.StockQuantity} in stock");

            item.Quantity = newQty;
        }

        await _context.SaveChangesAsync();
    }

    // GET USER CART
    //public async Task<IEnumerable<CartItemDto>> GetCartAsync(int userId)
    //{
    //    return await _context.CartItems
    //        .Where(x => x.UserId == userId)
    //        .Select(x => new CartItemDto
    //        {
    //            Id = x.Id,
    //            ProductId = x.ProductId,
    //            Quantity = x.Quantity
    //        }).ToListAsync();
    //}



    public async Task<IEnumerable<CartItemDto>> GetCartAsync(int userId)
    {
        return await _context.CartItems
            .Where(x => x.UserId == userId)
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
                      Quantity = c.Quantity
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
