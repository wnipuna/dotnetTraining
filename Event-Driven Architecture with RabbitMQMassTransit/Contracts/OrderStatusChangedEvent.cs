namespace Contracts;

public record OrderStatusChangedEvent
{
    public Guid OrderId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
}
