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

    public async Task<IEnumerable<Product>> GetActiveAsync() =>
        await _context.Products.Where(x => x.IsActive).ToListAsync();

    public async Task<Product?> GetByIdAsync(int id) =>
        await _context.Products.FindAsync(id);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        if (!Enum.TryParse<ProductCategory>(category, true, out var cat))
            return Enumerable.Empty<Product>();

        return await _context.Products.Where(x => x.Category == cat).ToListAsync();
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

    public async Task DeleteAsync(int id)
    {
        var p = await _context.Products.FindAsync(id);
        if (p != null)
        {
            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
        }
    }
}
