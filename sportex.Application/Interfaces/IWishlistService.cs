using Sportex.Application.DTOs.Wishlist;

namespace Sportex.Application.Interfaces;

public interface IWishlistService
{
    Task AddAsync(int userId, int productId);
    Task RemoveAsync(int userId, int productId);
    Task<List<WishlistItemDto>> GetAsync(int userId);

    // ❤️ Toggle (Add if not exists, Remove if exists)
    Task ToggleAsync(int userId, int productId);
}
