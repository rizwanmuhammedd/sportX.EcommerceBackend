using Microsoft.EntityFrameworkCore;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Domain.Enums;
using Sportex.Infrastructure.Data;

namespace Sportex.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SportexDbContext _context;
        public ProductRepository(SportexDbContext context) => _context = context;

        // ✅ ADMIN ONLY: Gets ALL products (including inactive)
        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products.ToListAsync();

        // ✅ USERS: Gets only ACTIVE products
        public async Task<IEnumerable<Product>> GetActiveAsync() =>
            await _context.Products.Where(x => x.IsActive).ToListAsync();

        // ✅ USERS: Gets active product by ID (for product details page)
        public async Task<Product?> GetByIdAsync(int id) =>
            await _context.Products.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

        // ✅ ADMIN: Gets product by ID (including inactive)
        public async Task<Product?> GetByIdAdminAsync(int id) =>
            await _context.Products.FindAsync(id);

        // ✅ USERS: Gets active products by category
        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            if (!Enum.TryParse<ProductCategory>(category, true, out var cat))
                return Enumerable.Empty<Product>();

            return await _context.Products
                .Where(x => x.Category == cat && x.IsActive) // ✅ Only active products
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // ✅ SOFT DELETE: Mark as inactive instead of removing
        public async Task SoftDeleteAsync(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p != null)
            {
                p.IsActive = false; // ✅ Soft delete
                await _context.SaveChangesAsync();
            }
        }

        // ✅ RESTORE: Mark as active again
        public async Task RestoreAsync(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p != null)
            {
                p.IsActive = true; // ✅ Restore
                await _context.SaveChangesAsync();
            }
        }

        // ⚠️ Remove the old DeleteAsync method or mark it as obsolete
        public async Task DeleteAsync(int id)
        {
            // Keep this for backward compatibility, but make it soft delete
            await SoftDeleteAsync(id);
        }

        public async Task<IEnumerable<string>> GetAllCategoriesAsync()
        {
            return await _context.Products
                .Where(x => x.IsActive) // ✅ Only active products
                .Select(x => x.Category.ToString())
                .Distinct()
                .ToListAsync();
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredProductsAsync(
            string? category = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = null,
            string? search = null,
            int page = 1,
            int pageSize = 9)
        {
            var query = _context.Products.Where(x => x.IsActive).AsQueryable(); // ✅ Only active

            // Category filter
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                if (Enum.TryParse<ProductCategory>(category, true, out var categoryEnum))
                {
                    query = query.Where(x => x.Category == categoryEnum);
                }
            }

            // Price range filter
            if (minPrice.HasValue)
                query = query.Where(x => x.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(x => x.Price <= maxPrice.Value);

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.Category.ToString().Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "price-low" => query.OrderBy(x => x.Price),
                "price-high" => query.OrderByDescending(x => x.Price),
                "newest" => query.OrderByDescending(x => x.Id),
                "popular" => query.OrderByDescending(x => x.Id),
                "trending" => query.OrderByDescending(x => x.Id),
                _ => query.OrderByDescending(x => x.Id)
            };

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<object> GetProductStatsAsync()
        {
            var products = await _context.Products.Where(x => x.IsActive).ToListAsync();

            return new
            {
                TotalProducts = products.Count,
                AveragePrice = products.Any() ? Math.Round(products.Average(x => x.Price), 2) : 0,
                MinPrice = products.Any() ? products.Min(x => x.Price) : 0,
                MaxPrice = products.Any() ? products.Max(x => x.Price) : 0,
                LowStockCount = products.Count(x => x.StockQuantity <= 10),
                TotalStock = products.Sum(x => x.StockQuantity)
            };
        }

        // NEW METHODS FOR PRODUCT DETAILS PAGE
        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count = 4)
        {
            var currentProduct = await _context.Products.FindAsync(productId);
            if (currentProduct == null)
                return new List<Product>();

            return await _context.Products
                .Where(p => p.IsActive &&
                           p.Category == currentProduct.Category &&
                           p.Id != productId)
                .OrderBy(p => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(int productId)
        {
            // This is a placeholder - in a real app, you'd track views in a separate table
            // For now, we'll just log it
            Console.WriteLine($"Product {productId} was viewed");
            await Task.CompletedTask;
        }

        public async Task<int> GetProductViewsAsync(int productId)
        {
            // Placeholder - generate random views for demo
            var random = new Random();
            return await Task.FromResult(random.Next(500, 2000));
        }

        public async Task<int> GetPurchaseCountAsync(int productId)
        {
            // Placeholder - in a real app, you'd query orders table
            var random = new Random();
            return await Task.FromResult(random.Next(20, 200));
        }

        public async Task<int> GetWishlistCountAsync(int productId)
        {
            return await _context.WishlistItems
                .Where(w => w.ProductId == productId)
                .CountAsync();
        }
    }
}