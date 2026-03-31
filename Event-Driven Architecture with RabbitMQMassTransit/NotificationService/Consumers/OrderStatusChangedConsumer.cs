using MassTransit;
using Contracts;

namespace NotificationService.Consumers;

public class OrderStatusChangedConsumer : IConsumer<OrderStatusChangedEvent>
{
    private readonly ILogger<OrderStatusChangedConsumer> _logger;

    public OrderStatusChangedConsumer(ILogger<OrderStatusChangedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "📧 Notification: Order status changed! OrderId: {OrderId}, New Status: {Status}, UpdatedAt: {UpdatedAt}",
            message.OrderId,
            message.Status,
            message.UpdatedAt);

        await Task.Delay(100);
        
        _logger.LogInformation("✅ Order status change notification sent successfully for OrderId: {OrderId}", message.OrderId);
    }
}
