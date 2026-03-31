using OrderManagement.Domain.Common;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Events;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Aggregates;

public class Order : AggregateRoot
{
    private readonly List<OrderItem> _orderItems = new();

    public string OrderNumber { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; }
    public Email CustomerEmail { get; private set; }
    public Address ShippingAddress { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime OrderDate { get; private set; }
    public Money TotalAmount { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order(
        Guid customerId,
        string customerName,
        Email customerEmail,
        Address shippingAddress)
        : base()
    {
        CustomerId = customerId;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        OrderDate = DateTime.UtcNow;
        OrderNumber = GenerateOrderNumber();
        TotalAmount = Money.Create(0, "USD");

        AddDomainEvent(new OrderCreatedEvent(Id, OrderNumber, CustomerId, OrderDate));
    }

    public static Order Create(
        Guid customerId,
        string customerName,
        Email customerEmail,
        Address shippingAddress)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(customerId));

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("CustomerName cannot be empty", nameof(customerName));

        if (customerEmail == null)
            throw new ArgumentNullException(nameof(customerEmail));

        if (shippingAddress == null)
            throw new ArgumentNullException(nameof(shippingAddress));

        return new Order(customerId, customerName, customerEmail, shippingAddress);
    }

    public void AddOrderItem(Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot add items to a non-pending order");

        var existingItem = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = OrderItem.Create(productId, productName, quantity, unitPrice);
            _orderItems.Add(orderItem);
        }

        RecalculateTotalAmount();
        AddDomainEvent(new OrderItemAddedEvent(Id, productId, quantity));
    }

    public void RemoveOrderItem(Guid orderItemId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot remove items from a non-pending order");

        var item = _orderItems.FirstOrDefault(x => x.Id == orderItemId);
        if (item == null)
            throw new InvalidOperationException("Order item not found");

        _orderItems.Remove(item);
        RecalculateTotalAmount();
        AddDomainEvent(new OrderItemRemovedEvent(Id, orderItemId));
    }

    public void UpdateOrderItemQuantity(Guid orderItemId, int newQuantity)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot update items in a non-pending order");

        var item = _orderItems.FirstOrDefault(x => x.Id == orderItemId);
        if (item == null)
            throw new InvalidOperationException("Order item not found");

        item.UpdateQuantity(newQuantity);
        RecalculateTotalAmount();
    }

    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        if (!_orderItems.Any())
            throw new InvalidOperationException("Cannot confirm an order without items");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(Id, OrderNumber, TotalAmount));
    }

    public void ShipOrder()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be shipped");

        Status = OrderStatus.Shipped;
        AddDomainEvent(new OrderShippedEvent(Id, OrderNumber, DateTime.UtcNow));
    }

    public void DeliverOrder()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered");

        Status = OrderStatus.Delivered;
        AddDomainEvent(new OrderDeliveredEvent(Id, OrderNumber, DateTime.UtcNow));
    }

    public void CancelOrder()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel a delivered order");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Order is already cancelled");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id, OrderNumber, DateTime.UtcNow));
    }

    public void UpdateShippingAddress(Address newAddress)
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Cannot update shipping address for shipped or delivered orders");

        if (newAddress == null)
            throw new ArgumentNullException(nameof(newAddress));

        ShippingAddress = newAddress;
    }

    private void RecalculateTotalAmount()
    {
        if (!_orderItems.Any())
        {
            TotalAmount = Money.Create(0, "USD");
            return;
        }

        var total = _orderItems.First().TotalPrice;
        foreach (var item in _orderItems.Skip(1))
        {
            total = total.Add(item.TotalPrice);
        }

        TotalAmount = total;
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }
}
