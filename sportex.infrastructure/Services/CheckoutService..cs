using Microsoft.EntityFrameworkCore;
using Sportex.Application.DTOs.Checkout;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;

namespace Sportex.Infrastructure.Services;

public class CheckoutService : ICheckoutService
{
    private readonly SportexDbContext _context;
    public CheckoutService(SportexDbContext context) => _context = context;

    public async Task<IEnumerable<CheckoutPreviewDto>> PreviewAsync(int userId)
    {
        return await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .Select(c => new CheckoutPreviewDto
            {
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                Quantity = c.Quantity,
                Price = c.Product.Price
            }).ToListAsync();
    }

    public async Task ConfirmAsync(ConfirmCheckoutDto dto)
    {
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == dto.UserId)
            .ToListAsync();

        if (!cartItems.Any())
            throw new Exception("Cart is empty");

        var total = cartItems.Sum(x => x.Product.Price * x.Quantity);

        var order = new Order
        {
            UserId = dto.UserId,
            ShippingAddress = dto.ShippingAddress,
            TotalAmount = total
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in cartItems)
        {
            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Product.Price
            });

            item.Product.StockQuantity -= item.Quantity;
        }

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }
}
