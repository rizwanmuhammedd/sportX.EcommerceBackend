using System.ComponentModel.DataAnnotations;
using Sportex.Application.DTOs.Shipping;

namespace Sportex.Application.DTOs.Orders
{
    public class CreateDirectOrderDto
    {
        [Required]
        public ShippingAddressDto ShippingAddress { get; set; } = new();

        public string? PaymentMode { get; set; }

        [Required]
        public List<OrderRequestItemDto> Items { get; set; } = new();
    }
}
