using System.ComponentModel.DataAnnotations;

namespace Sportex.Application.DTOs.Shipping
{
    public class ShippingAddressDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        [Required]
        public string Phone { get; set; } = "";

        public string? AltPhone { get; set; }

        [Required]
        [MaxLength(200)]
        public string AddressLine { get; set; } = "";

        public string? Landmark { get; set; }

        [Required]
        public string City { get; set; } = "";

        [Required]
        public string State { get; set; } = "";

        [Required]
        public string Pincode { get; set; } = "";

        public bool IsDefault { get; set; }
    }
}
