using System.ComponentModel.DataAnnotations;

namespace Sportex.Application.DTOs.Orders
{

    public class CreateDirectOrderDto
    {
        [Required]
        [MaxLength(300)]
        public string ShippingAddress { get; set; } = "";

        [Required]
        public List<OrderRequestItemDto> Items { get; set; } = new();
    }
}
