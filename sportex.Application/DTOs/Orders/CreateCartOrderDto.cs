using System.ComponentModel.DataAnnotations;
using Sportex.Application.DTOs.Shipping;

namespace Sportex.Application.DTOs.Orders
{
    public class CreateCartOrderDto
    {
        [Required]
        public ShippingAddressDto ShippingAddress { get; set; } = new();

        public string? PaymentMode { get; set; }
    }
}
