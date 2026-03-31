using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Events;

public class OrderItemRemovedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid OrderItemId { get; }
    public DateTime OccurredOn { get; }

    public OrderItemRemovedEvent(Guid orderId, Guid orderItemId)
    {
        OrderId = orderId;
        OrderItemId = orderItemId;
        OccurredOn = DateTime.UtcNow;
    }
}
