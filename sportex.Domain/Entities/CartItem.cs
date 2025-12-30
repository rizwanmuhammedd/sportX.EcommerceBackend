using Sportex.Domain.Common;

namespace Sportex.Domain.Entities;

public class CartItem : BaseEntity
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public Product Product { get; set; } = null!;
}
