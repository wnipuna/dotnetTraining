using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Events;

public class OrderCancelledEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public DateTime OccurredOn { get; }

    public OrderCancelledEvent(Guid orderId, string orderNumber, DateTime occurredOn)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        OccurredOn = occurredOn;
    }
}
