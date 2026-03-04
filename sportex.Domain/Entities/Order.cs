//using Sportex.Domain.Common;
//using Sportex.Domain.Enums;

//namespace Sportex.Domain.Entities;



//public class Order : BaseEntity
//{
//    public int UserId { get; set; }

//    public User User { get; set; } = null!;

//    public decimal TotalAmount { get; set; }

//    // 🔥 Keep this as a SNAPSHOT of selected address at order time
//    public string ShippingAddress { get; set; } = "";

//    // 🔥 NEW — VERY IMPORTANT (COD vs Razorpay)
//    public string PaymentMode { get; set; } = "ONLINE";
//    // Possible values: "ONLINE" or "COD"

//    public OrderStatus Status { get; set; } = OrderStatus.Pending;

//    public bool IsPaid { get; set; }

//    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

//    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
//}



using Sportex.Domain.Common;
using Sportex.Domain.Entities;
using Sportex.Domain.Enums;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    // Snapshot for invoice/history
    public string ShippingSnapshot { get; set; } = "";

    // FK for admin/logistics
    //public int ShippingAddressId { get; set; }
    //public ShippingAddress ShippingAddress { get; set; } = null!;

    public string PaymentMode { get; set; } = "ONLINE";
    public int? ShippingAddressId { get; set; }
    public ShippingAddress? ShippingAddress { get; set; }


    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public bool IsPaid { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
