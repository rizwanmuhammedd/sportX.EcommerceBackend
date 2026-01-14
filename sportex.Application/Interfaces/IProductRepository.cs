using Sportex.Domain.Entities;

namespace Sportex.Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
}
