namespace OrderService.Models;

public record CreateOrderRequest
{
    public string CustomerName { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
