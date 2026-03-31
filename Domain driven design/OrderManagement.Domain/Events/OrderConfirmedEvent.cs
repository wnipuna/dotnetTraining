using OrderManagement.Domain.Common;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Events;

public class OrderConfirmedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public Money TotalAmount { get; }
    public DateTime OccurredOn { get; }

    public OrderConfirmedEvent(Guid orderId, string orderNumber, Money totalAmount)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        TotalAmount = totalAmount;
        OccurredOn = DateTime.UtcNow;
    }
}
