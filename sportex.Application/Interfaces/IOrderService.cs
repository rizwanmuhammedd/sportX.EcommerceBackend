using Sportex.Application.DTOs.Orders;

namespace Sportex.Application.Interfaces;

public interface IOrderService
{
    Task PlaceOrderAsync(CreateOrderDto dto);
}
