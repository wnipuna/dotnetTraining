namespace OrderManagement.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
