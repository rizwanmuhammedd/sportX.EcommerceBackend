namespace Sportex.Application.DTOs.Cart
{
    public class UpdateCartQuantityDto
    {
        public int ProductId { get; set; }
        public int QuantityChange { get; set; } // Can be positive or negative
    }
}