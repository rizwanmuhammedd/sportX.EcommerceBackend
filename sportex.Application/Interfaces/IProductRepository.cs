
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Sportex.Domain.Entities;

//namespace Sportex.Application.Interfaces
//{
//    public interface IProductRepository
//    {
//        Task<IEnumerable<Product>> GetAllAsync();
//        Task<IEnumerable<Product>> GetActiveAsync();
//        Task<Product?> GetByIdAsync(int id);
//        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
//        Task AddAsync(Product product);
//        Task UpdateAsync(Product product);
//        Task DeleteAsync(int id);

//        // MoreProducts page methods
//        Task<IEnumerable<string>> GetAllCategoriesAsync();
//        Task<object> GetProductStatsAsync();
//        Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredProductsAsync(
//            string? category = null,
//            decimal? minPrice = null,
//            decimal? maxPrice = null,
//            string? sortBy = null,
//            string? search = null,
//            int page = 1,
//            int pageSize = 9);

//        // New methods for ProductDetails page
//        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count = 4);
//        Task IncrementViewCountAsync(int productId);
//        Task<int> GetProductViewsAsync(int productId);
//        Task<int> GetPurchaseCountAsync(int productId);
//        Task<int> GetWishlistCountAsync(int productId);
//    }
//}







using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sportex.Domain.Entities;

namespace Sportex.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetActiveAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByIdAdminAsync(int id); // ✅ ADD THIS
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id); // ✅ ADD THIS
        Task RestoreAsync(int id); // ✅ ADD THIS

        // MoreProducts page methods
        Task<IEnumerable<string>> GetAllCategoriesAsync();
        Task<object> GetProductStatsAsync();
        Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredProductsAsync(
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = null,
            string? search = null,
            int page = 1,
            int pageSize = 9);

        // New methods for ProductDetails page
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count = 4);
        Task IncrementViewCountAsync(int productId);
        Task<int> GetProductViewsAsync(int productId);
        Task<int> GetPurchaseCountAsync(int productId);
        Task<int> GetWishlistCountAsync(int productId);
    }
}