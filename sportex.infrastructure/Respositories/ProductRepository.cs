//using Microsoft.EntityFrameworkCore;
//using Sportex.Application.Interfaces;
//using Sportex.Domain.Entities;
//using Sportex.Infrastructure.Data;

//namespace Sportex.Infrastructure.Repositories;

//public class ProductRepository : IProductRepository
//{
//    private readonly SportexDbContext _context;
//    public ProductRepository(SportexDbContext context) => _context = context;

//    public async Task<IEnumerable<Product>> GetAllAsync() =>
//        await _context.Products.ToListAsync();

//    public async Task<Product?> GetByIdAsync(int id) =>
//        await _context.Products.FindAsync(id);

//    public async Task AddAsync(Product product)
//    {
//        _context.Products.Add(product);
//        await _context.SaveChangesAsync();
//    }

//    public async Task UpdateAsync(Product product)
//    {
//        _context.Products.Update(product);
//        await _context.SaveChangesAsync();
//    }
//}







using Microsoft.EntityFrameworkCore;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Domain.Enums;
using Sportex.Infrastructure.Data;

namespace Sportex.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly SportexDbContext _context;
    public ProductRepository(SportexDbContext context) => _context = context;

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _context.Products.ToListAsync();

    public async Task<Product?> GetByIdAsync(int id) =>
        await _context.Products.FindAsync(id);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        if (!Enum.TryParse<ProductCategory>(category, true, out var cat))
            return new List<Product>();

        return await _context.Products
            .Where(x => x.Category == cat)
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
}
