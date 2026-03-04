////using Sportex.Application.DTOs.Orders;
////using Sportex.Domain.Enums;







//using Sportex.Application.DTOs.Orders;
//using Sportex.Application.DTOs.Shipping;
//using Sportex.Domain.Enums;

//namespace Sportex.Application.DTOs.Orders
//{
//    public class OrderDto
//    {
//        public int Id { get; set; }

//        public int UserId { get; set; }
//        public string? UserName { get; set; }
//        public string? UserEmail { get; set; }

//        public decimal TotalAmount { get; set; }
//        public string Status { get; set; } = "";

//        public DateTime OrderDate { get; set; }   // ✅ REQUIRED (your code uses this)
//        public bool IsPaid { get; set; }         // ✅ REQUIRED (your code uses this)

//        // String snapshot for backward compatibility
//        public string ShippingAddress { get; set; } = "";

//        // Full address object for admin (nullable)
//        public ShippingAddressDto? ShippingAddressDetails { get; set; }

//        public string PaymentMode { get; set; } = "ONLINE";

//        public List<OrderItemDto> Items { get; set; } = new();
//    }
//}




using Sportex.Application.DTOs.Orders;
using Sportex.Application.DTOs.Shipping; // Add this using statement
using Sportex.Domain.Enums;

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

        public DateTime OrderDate { get; set; }
        public bool IsPaid { get; set; }

        // String snapshot for backward compatibility
        public string ShippingAddress { get; set; } = "";

        // Full address object for admin (nullable)
        public ShippingAddressDto? ShippingAddressDetails { get; set; }

        public string PaymentMode { get; set; } = "ONLINE";

        public List<OrderItemDto> Items { get; set; } = new();
    }
}