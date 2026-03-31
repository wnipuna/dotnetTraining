using MassTransit;
using Contracts;
using OrderService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var orders = new List<Order>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async (CreateOrderRequest request, IPublishEndpoint publishEndpoint) =>
{
    var order = new Order
    {
        Id = Guid.NewGuid(),
        CustomerName = request.CustomerName,
        ProductName = request.ProductName,
        Amount = request.Amount,
        Status = "Pending",
        CreatedAt = DateTime.UtcNow
    };

    orders.Add(order);

    var orderCreatedEvent = new OrderCreatedEvent
    {
        OrderId = order.Id,
        CustomerName = order.CustomerName,
        ProductName = order.ProductName,
        Amount = order.Amount,
        CreatedAt = order.CreatedAt
    };

    await publishEndpoint.Publish(orderCreatedEvent);

    return Results.Created($"/orders/{order.Id}", order);
})
.WithName("CreateOrder");

app.MapGet("/orders", () =>
{
    return Results.Ok(orders);
})
.WithName("GetOrders");

app.MapGet("/orders/{id:guid}", (Guid id) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("GetOrderById");

app.MapPut("/orders/{id:guid}/status", async (Guid id, string status, IPublishEndpoint publishEndpoint) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    if (order is null)
        return Results.NotFound();

    order.Status = status;

    var statusChangedEvent = new OrderStatusChangedEvent
    {
        OrderId = order.Id,
        Status = status,
        UpdatedAt = DateTime.UtcNow
    };

    await publishEndpoint.Publish(statusChangedEvent);

    return Results.Ok(order);
})
.WithName("UpdateOrderStatus");

app.Run();
