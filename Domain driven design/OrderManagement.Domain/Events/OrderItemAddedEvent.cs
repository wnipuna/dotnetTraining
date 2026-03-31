using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Events;

public class OrderItemAddedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid ProductId { get; }
    public int Quantity { get; }
    public DateTime OccurredOn { get; }

    public OrderItemAddedEvent(Guid orderId, Guid productId, int quantity)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        OccurredOn = DateTime.UtcNow;
    }
}
