using Sportex.Application.DTOs.Shipping;

namespace Sportex.Application.Interfaces
{
    public interface IShippingAddressService
    {
        Task<IEnumerable<ShippingAddressDto>> GetAllAsync(int userId);
        Task<ShippingAddressDto> AddAsync(ShippingAddressDto dto, int userId);
        Task<ShippingAddressDto> UpdateAsync(int id, ShippingAddressDto dto, int userId);
        Task DeleteAsync(int id, int userId);
        Task SetDefaultAsync(int id, int userId);
    }
}
