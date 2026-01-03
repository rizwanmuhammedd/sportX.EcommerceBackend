using Sportex.Domain.Common;
using Sportex.Domain.Enums;

namespace Sportex.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = "";
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public bool IsPaid { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
