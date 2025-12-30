using Sportex.Domain.Common;

namespace Sportex.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public string ShippingAddress { get; set; } = "";
    public decimal TotalAmount { get; set; }
}
