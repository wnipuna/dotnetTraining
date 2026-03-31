using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(createOrderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the order" });
        }
    }

    [HttpGet("order-number/{orderNumber}")]
    public async Task<ActionResult<OrderDto>> GetOrderByOrderNumber(string orderNumber)
    {
        try
        {
            var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderNumber}", orderNumber);
            return StatusCode(500, new { error = "An error occurred while retrieving the order" });
        }
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomerId(Guid customerId)
    {
        try
        {
            var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders for customer {CustomerId}", customerId);
            return StatusCode(500, new { error = "An error occurred while retrieving orders" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all orders");
            return StatusCode(500, new { error = "An error occurred while retrieving orders" });
        }
    }

    [HttpPost("{id}/items")]
    public async Task<ActionResult<OrderDto>> AddOrderItem(Guid id, [FromBody] AddOrderItemDto addOrderItemDto)
    {
        try
        {
            var order = await _orderService.AddOrderItemAsync(id, addOrderItemDto);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to order {OrderId}", id);
            return StatusCode(500, new { error = "An error occurred while adding the item" });
        }
    }

    [HttpDelete("{id}/items/{itemId}")]
    public async Task<ActionResult<OrderDto>> RemoveOrderItem(Guid id, Guid itemId)
    {
        try
        {
            var order = await _orderService.RemoveOrderItemAsync(id, itemId);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item {ItemId} from order {OrderId}", itemId, id);
            return StatusCode(500, new { error = "An error occurred while removing the item" });
        }
    }

    [HttpPut("{id}/items/{itemId}/quantity")]
    public async Task<ActionResult<OrderDto>> UpdateOrderItemQuantity(
        Guid id, 
        Guid itemId, 
        [FromBody] UpdateQuantityRequest request)
    {
        try
        {
            var order = await _orderService.UpdateOrderItemQuantityAsync(id, itemId, request.Quantity);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quantity for item {ItemId} in order {OrderId}", itemId, id);
            return StatusCode(500, new { error = "An error occurred while updating the quantity" });
        }
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult<OrderDto>> ConfirmOrder(Guid id)
    {
        try
        {
            var order = await _orderService.ConfirmOrderAsync(id);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming order {OrderId}", id);
            return StatusCode(500, new { error = "An error occurred while confirming the order" });
        }
    }

    [HttpPost("{id}/ship")]
    public async Task<ActionResult<OrderDto>> ShipOrder(Guid id)
    {
        try
        {
            var order = await _orderService.ShipOrderAsync(id);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shipping order {OrderId}", id);
            return StatusCode(500, new { error = "An error occurred while shipping the order" });
        }
    }

    [HttpPost("{id}/deliver")]
    public async Task<ActionResult<OrderDto>> DeliverOrder(Guid id)
    {
        try
        {
            var order = await _orderService.DeliverOrderAsync(id);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delivering order {OrderId}", id);
            return StatusCode(500, new { error = "An error occurred while delivering the order" });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<OrderDto>> CancelOrder(Guid id)
    {
        try
        {
            var order = await _orderService.CancelOrderAsync(id);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", id);
            return StatusCode(500, new { error = "An error occurred while cancelling the order" });
        }
    }

    [HttpPut("{id}/shipping-address")]
    public async Task<ActionResult<OrderDto>> UpdateShippingAddress(Guid id, [FromBody] AddressDto addressDto)
    {
        try
        {
            var order = await _orderService.UpdateShippingAddressAsync(id, addressDto);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating shipping address for order {OrderId}", id);
            return StatusCode(500, new { error = "An error occurred while updating the shipping address" });
        }
    }
}

public record UpdateQuantityRequest(int Quantity);
