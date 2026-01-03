using System.ComponentModel.DataAnnotations;

namespace Sportex.Application.DTOs.Orders;

public class CreateCartOrderDto
{
    [Required]
    [MaxLength(300)]
    public string ShippingAddress { get; set; } = "";
}
