using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Events;

public class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public Guid CustomerId { get; }
    public DateTime OccurredOn { get; }

    public OrderCreatedEvent(Guid orderId, string orderNumber, Guid customerId, DateTime occurredOn)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        CustomerId = customerId;
        OccurredOn = occurredOn;
    }
}
