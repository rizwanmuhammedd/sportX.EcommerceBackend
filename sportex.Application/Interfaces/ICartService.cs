////using Sportex.Application.DTOs.Cart;

////namespace Sportex.Application.Interfaces;

////public interface ICartService
////{
////    Task AddToCartAsync(AddToCartDto dto);
////    Task<IEnumerable<CartItemDto>> GetCartAsync(int userId);
////    Task RemoveItemAsync(int id);
////    Task ClearCartAsync(int userId);
////}



//using Sportex.Application.DTOs.Cart;

//namespace Sportex.Application.Interfaces;

//public interface ICartService
//{
//    Task AddToCartAsync(AddToCartDto dto);
//    Task<IEnumerable<CartItemDto>> GetCartAsync(int userId);

//    // 🔒 Now removal also needs userId
//    Task RemoveItemAsync(int cartItemId, int userId);

//    Task ClearCartAsync(int userId);
//}



using Sportex.Application.DTOs.Cart;

public interface ICartService
{
    Task AddToCartAsync(AddToCartDto dto, int userId);
    Task<IEnumerable<CartItemDto>> GetCartAsync(int userId);
    Task RemoveItemAsync(int cartItemId, int userId);
    Task ClearCartAsync(int userId);
}
