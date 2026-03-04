using Microsoft.EntityFrameworkCore;
using Sportex.Application.DTOs.Shipping;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;

namespace Sportex.Infrastructure.Services
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly SportexDbContext _context;

        public ShippingAddressService(SportexDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShippingAddressDto>> GetAllAsync(int userId)
        {
            return await _context.ShippingAddresses
                .Where(a => a.UserId == userId)
                .Select(a => new ShippingAddressDto
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    AltPhone = a.AltPhone,
                    AddressLine = a.AddressLine,
                    Landmark = a.Landmark,
                    City = a.City,
                    State = a.State,
                    Pincode = a.Pincode,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();
        }

        public async Task<ShippingAddressDto> AddAsync(ShippingAddressDto dto, int userId)
        {
            if (dto.IsDefault)
            {
                var oldDefaults = await _context.ShippingAddresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ToListAsync();

                foreach (var a in oldDefaults)
                    a.IsDefault = false;
            }
            var address = new ShippingAddress
            {
                UserId = userId,
                FullName = dto.FullName ?? string.Empty,
                Phone = dto.Phone ?? string.Empty,
                AltPhone = dto.AltPhone,
                AddressLine = dto.AddressLine ?? string.Empty,
                Landmark = dto.Landmark,
                City = dto.City ?? string.Empty,
                State = dto.State ?? string.Empty,
                Pincode = dto.Pincode ?? string.Empty,
                IsDefault = dto.IsDefault
            };


            _context.ShippingAddresses.Add(address);
            await _context.SaveChangesAsync();

            dto.Id = address.Id;
            return dto;
        }

        public async Task<ShippingAddressDto> UpdateAsync(int id, ShippingAddressDto dto, int userId)
        {
            var address = await _context.ShippingAddresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
                throw new Exception("Address not found");

            if (dto.IsDefault)
            {
                var oldDefaults = await _context.ShippingAddresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ToListAsync();

                foreach (var a in oldDefaults)
                    a.IsDefault = false;
            }

            address.FullName = dto.FullName;
            address.Phone = dto.Phone;
            address.AltPhone = dto.AltPhone;
            address.AddressLine = dto.AddressLine;
            address.Landmark = dto.Landmark;
            address.City = dto.City;
            address.State = dto.State;
            address.Pincode = dto.Pincode;
            address.IsDefault = dto.IsDefault;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var address = await _context.ShippingAddresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
                throw new Exception("Address not found");

            _context.ShippingAddresses.Remove(address);
            await _context.SaveChangesAsync();
        }

        public async Task SetDefaultAsync(int id, int userId)
        {
            var addresses = await _context.ShippingAddresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            foreach (var a in addresses)
                a.IsDefault = false;

            var address = addresses.FirstOrDefault(a => a.Id == id);

            if (address == null)
                throw new Exception("Address not found");

            address.IsDefault = true;
            await _context.SaveChangesAsync();
        }
    }
}
