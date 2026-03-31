using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Aggregates;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(createOrderDto.CustomerEmail);
        var address = Address.Create(
            createOrderDto.ShippingAddress.Street,
            createOrderDto.ShippingAddress.City,
            createOrderDto.ShippingAddress.State,
            createOrderDto.ShippingAddress.Country,
            createOrderDto.ShippingAddress.ZipCode
        );

        var order = Order.Create(
            createOrderDto.CustomerId,
            createOrderDto.CustomerName,
            email,
            address
        );

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found");

        return MapToDto(order);
    }

    public async Task<OrderDto> GetOrderByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(orderNumber, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with number {orderNumber} not found");

        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> AddOrderItemAsync(Guid orderId, AddOrderItemDto addOrderItemDto, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found");

        var unitPrice = Money.Create(addOrderItemDto.UnitPrice, addOrderItemDto.Currency);
        
        order.AddOrderItem(
            addOrderItemDto.ProductId,
            addOrderItemDto.ProductName,
            addOrderItemDto.Quantity,
            unitPrice
        );

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> RemoveOrderItemAsync(Guid orderId, Guid orderItemId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found");

        order.RemoveOrderItem(orderItemId);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateOrderItemQuantityAsync(Guid orderId, Guid orderItemId, int newQuantity, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found");

        order.UpdateOrderItemQuantity(orderItemId, newQuantity);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> ConfirmOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found");

        order.ConfirmOrder();

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> ShipOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found");

        order.ShipOrder();

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> DeliverOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found");

        order.DeliverOrder();

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found");

        order.CancelOrder();

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateShippingAddressAsync(Guid orderId, AddressDto addressDto, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found");

        var newAddress = Address.Create(
            addressDto.Street,
            addressDto.City,
            addressDto.State,
            addressDto.Country,
            addressDto.ZipCode
        );

        order.UpdateShippingAddress(newAddress);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail.Value,
            ShippingAddress = new AddressDto
            {
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                Country = order.ShippingAddress.Country,
                ZipCode = order.ShippingAddress.ZipCode
            },
            Status = order.Status.ToString(),
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount.Amount,
            Currency = order.TotalAmount.Currency,
            OrderItems = order.OrderItems.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.Amount,
                TotalPrice = item.TotalPrice.Amount,
                Currency = item.UnitPrice.Currency
            }).ToList()
        };
    }
}
