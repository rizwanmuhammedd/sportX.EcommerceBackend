using Sportex.Application.DTOs.Cart;

namespace Sportex.Application.Interfaces;

public interface ICartService
{
    Task AddToCartAsync(AddToCartDto dto);
    Task<IEnumerable<CartItemDto>> GetCartAsync(int userId);
    Task RemoveItemAsync(int id);
    Task ClearCartAsync(int userId);
}
