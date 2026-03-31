using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Services;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken = default);
    Task<OrderDto> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderDto> GetOrderByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<OrderDto> AddOrderItemAsync(Guid orderId, AddOrderItemDto addOrderItemDto, CancellationToken cancellationToken = default);
    Task<OrderDto> RemoveOrderItemAsync(Guid orderId, Guid orderItemId, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateOrderItemQuantityAsync(Guid orderId, Guid orderItemId, int newQuantity, CancellationToken cancellationToken = default);
    Task<OrderDto> ConfirmOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderDto> ShipOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderDto> DeliverOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderDto> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateShippingAddressAsync(Guid orderId, AddressDto addressDto, CancellationToken cancellationToken = default);
}
