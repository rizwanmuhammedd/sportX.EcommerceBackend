using Sportex.Domain.Common;

namespace Sportex.Domain.Entities;

public class WishlistItem : BaseEntity
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
}
