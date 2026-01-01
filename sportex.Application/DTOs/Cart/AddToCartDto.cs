using System.ComponentModel.DataAnnotations;

namespace Sportex.Application.DTOs.Cart;

public class AddToCartDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }
}
