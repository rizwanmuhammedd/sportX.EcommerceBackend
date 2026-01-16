using Sportex.Application.DTOs.Orders;
using Sportex.Domain.Enums;






//namespace Sportex.Application.DTOs.Orders
//{

//    public class OrderDto
//    {
//        public int Id { get; set; }
//        public decimal TotalAmount { get; set; }
//        public string ShippingAddress { get; set; } = "";
//        public string Status { get; set; } = "";   // ⬅️ change here
//        public bool IsPaid { get; set; }
//        public DateTime OrderDate { get; set; }
//        public List<OrderItemDto> Items { get; set; } = new();
//    }
//}

namespace Sportex.Application.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }

        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "";

        public DateTime OrderDate { get; set; }   // ✅ REQUIRED (your code uses this)
        public bool IsPaid { get; set; }         // ✅ REQUIRED (your code uses this)

        public string ShippingAddress { get; set; } = ""; // keep as string (matches DB)

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
