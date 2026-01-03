//public interface IWishlistService
//{
//    Task<bool> AddAsync(int userId, int productId);
//    Task<List<WishlistItemDto>> GetMyWishlistAsync(int userId);
//    Task<bool> RemoveAsync(int userId, int wishlistId);
//}




public interface IWishlistService
{
    Task<bool> ToggleAsync(int userId, int productId);     // 🔥 ADD / REMOVE TOGGLE
    Task<List<WishlistItemDto>> GetMyWishlistAsync(int userId);
    Task<bool> RemoveAsync(int userId, int wishlistId);
}
