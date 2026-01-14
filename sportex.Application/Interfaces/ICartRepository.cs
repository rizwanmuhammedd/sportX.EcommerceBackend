using Sportex.Domain.Entities;

namespace Sportex.Application.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetByUserAsync(int userId);
    Task<CartItem?> GetAsync(int userId, int productId);
    Task AddAsync(CartItem item);
    Task RemoveAsync(CartItem item);
    Task ClearAsync(int userId);
}
