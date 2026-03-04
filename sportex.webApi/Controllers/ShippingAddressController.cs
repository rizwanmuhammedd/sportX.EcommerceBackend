using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
using Sportex.Application.DTOs.Shipping;
using Sportex.Infrastructure.Data;
using Sportex.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/address")]
[Authorize(Roles = "user")]
public class ShippingAddressController : ControllerBase
{
    private readonly SportexDbContext _context;

    public ShippingAddressController(SportexDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst("uid")!.Value);
    }

    // ✅ ADD ADDRESS
    [HttpPost]
    public async Task<IActionResult> Add(ShippingAddressDto dto)
    {
        int userId = GetUserId();

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
            FullName = dto.FullName,
            Phone = dto.Phone,
            AltPhone = dto.AltPhone,
            AddressLine = dto.AddressLine,
            Landmark = dto.Landmark,
            City = dto.City,
            State = dto.State,
            Pincode = dto.Pincode,
            IsDefault = dto.IsDefault
        };

        _context.ShippingAddresses.Add(address);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success("Address added", address));
    }

    // ✅ GET ALL ADDRESSES OF USER
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        int userId = GetUserId();

        var addresses = await _context.ShippingAddresses
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

        return Ok(ApiResponse.Success("Addresses fetched", addresses));
    }

    // ✅ UPDATE ADDRESS
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ShippingAddressDto dto)
    {
        int userId = GetUserId();

        var address = await _context.ShippingAddresses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (address == null)
            return NotFound(ApiResponse.Fail(404, "Address not found"));

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

        return Ok(ApiResponse.Success("Address updated", address));
    }

    // ✅ DELETE ADDRESS
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        int userId = GetUserId();

        var address = await _context.ShippingAddresses
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (address == null)
            return NotFound(ApiResponse.Fail(404, "Address not found"));

        _context.ShippingAddresses.Remove(address);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success("Address deleted"));
    }

    // ✅ SET DEFAULT ADDRESS
    [HttpPatch("default/{id}")]
    public async Task<IActionResult> SetDefault(int id)
    {
        int userId = GetUserId();

        var addresses = await _context.ShippingAddresses
            .Where(a => a.UserId == userId)
            .ToListAsync();

        foreach (var a in addresses)
            a.IsDefault = false;

        var address = addresses.FirstOrDefault(a => a.Id == id);

        if (address == null)
            return NotFound(ApiResponse.Fail(404, "Address not found"));

        address.IsDefault = true;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Success("Default address set"));
    }
}
