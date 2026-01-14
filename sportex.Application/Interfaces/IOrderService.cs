using Sportex.Application.DTOs.Orders;
using Sportex.Domain.Enums;

namespace Sportex.Application.Interfaces;

public interface IOrderService
{
    Task<int> PlaceOrderAsync(int userId, CreateCartOrderDto dto);
    Task<int> PlaceDirectOrderAsync(int userId, CreateDirectOrderDto dto);

    Task<List<OrderDto>> GetMyOrdersAsync(int userId);
    Task<OrderDto?> GetOrderByIdAsync(int userId, int orderId);

    Task PayAsync(int userId, int orderId);
    Task ToggleStatusAsync(int orderId);

    Task<List<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto> CancelOrderAsync(int userId, int orderId);
    Task ConfirmPaymentAsync(int orderId);


    Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
    Task<List<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
}
