using MassTransit;
using Contracts;

namespace NotificationService.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "📧 Notification: New order created! OrderId: {OrderId}, Customer: {CustomerName}, Product: {ProductName}, Amount: ${Amount}, CreatedAt: {CreatedAt}",
            message.OrderId,
            message.CustomerName,
            message.ProductName,
            message.Amount,
            message.CreatedAt);

        await Task.Delay(100);
        
        _logger.LogInformation("✅ Order creation notification sent successfully for OrderId: {OrderId}", message.OrderId);
    }
}
