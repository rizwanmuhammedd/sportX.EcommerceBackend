//using Sportex.Domain.Enums;

//namespace Sportex.Application.DTOs.Orders;

//public class OrderDto
//{
//    public int Id { get; set; }
//    public decimal TotalAmount { get; set; }
//    public string ShippingAddress { get; set; } = "";
//    public OrderStatus Status { get; set; }
//    public bool IsPaid { get; set; }
//    public DateTime OrderDate { get; set; }
//    public List<OrderItemDto> Items { get; set; } = new();
//}






namespace Sportex.Application.DTOs.Orders
{

    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = "";
        public string Status { get; set; } = "";   // ⬅️ change here
        public bool IsPaid { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}