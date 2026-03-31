# Test Script for Event-Driven Architecture with RabbitMQ

Write-Host "🧪 Testing Event-Driven Architecture with RabbitMQ and MassTransit" -ForegroundColor Cyan
Write-Host ""

# Test 1: Create Order
Write-Host "📝 Test 1: Creating a new order..." -ForegroundColor Yellow
$orderBody = @{
    customerName = "John Doe"
    productName = "Laptop"
    amount = 1299.99
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5001/orders" -Method Post -Body $orderBody -ContentType "application/json"
    $orderId = $response.id
    Write-Host "✅ Order created successfully!" -ForegroundColor Green
    Write-Host "   Order ID: $orderId" -ForegroundColor Gray
    Write-Host "   Customer: $($response.customerName)" -ForegroundColor Gray
    Write-Host "   Product: $($response.productName)" -ForegroundColor Gray
    Write-Host "   Amount: $($response.amount)" -ForegroundColor Gray
    Write-Host "   Status: $($response.status)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ Failed to create order: $_" -ForegroundColor Red
    exit 1
}

# Wait for event processing
Write-Host "⏳ Waiting for event processing..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

# Test 2: Get All Orders
Write-Host "📋 Test 2: Retrieving all orders..." -ForegroundColor Yellow
try {
    $orders = Invoke-RestMethod -Uri "http://localhost:5001/orders" -Method Get
    Write-Host "✅ Retrieved $($orders.Count) order(s)" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "❌ Failed to retrieve orders: $_" -ForegroundColor Red
}

# Test 3: Get Order by ID
Write-Host "🔍 Test 3: Retrieving order by ID..." -ForegroundColor Yellow
try {
    $order = Invoke-RestMethod -Uri "http://localhost:5001/orders/$orderId" -Method Get
    Write-Host "✅ Order retrieved successfully!" -ForegroundColor Green
    Write-Host "   Status: $($order.status)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ Failed to retrieve order: $_" -ForegroundColor Red
}

# Test 4: Update Order Status
Write-Host "🔄 Test 4: Updating order status to 'Processing'..." -ForegroundColor Yellow
try {
    $updatedOrder = Invoke-RestMethod -Uri "http://localhost:5001/orders/$orderId/status?status=Processing" -Method Put
    Write-Host "✅ Order status updated successfully!" -ForegroundColor Green
    Write-Host "   New Status: $($updatedOrder.status)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ Failed to update order status: $_" -ForegroundColor Red
}

# Wait for event processing
Write-Host "⏳ Waiting for event processing..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

# Test 5: Update Order Status Again
Write-Host "🔄 Test 5: Updating order status to 'Shipped'..." -ForegroundColor Yellow
try {
    $updatedOrder = Invoke-RestMethod -Uri "http://localhost:5001/orders/$orderId/status?status=Shipped" -Method Put
    Write-Host "✅ Order status updated successfully!" -ForegroundColor Green
    Write-Host "   New Status: $($updatedOrder.status)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ Failed to update order status: $_" -ForegroundColor Red
}

# Test 6: Check NotificationService Health
Write-Host "🏥 Test 6: Checking NotificationService health..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "http://localhost:5002/health" -Method Get
    Write-Host "✅ NotificationService is healthy!" -ForegroundColor Green
    Write-Host "   Service: $($health.service)" -ForegroundColor Gray
    Write-Host "   Status: $($health.status)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ NotificationService health check failed: $_" -ForegroundColor Red
}

Write-Host "🎉 All tests completed!" -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 Check the following:" -ForegroundColor Yellow
Write-Host "   1. NotificationService terminal for event notifications" -ForegroundColor Gray
Write-Host "   2. RabbitMQ Management UI (http://localhost:15672) for queue statistics" -ForegroundColor Gray
Write-Host "   3. OrderService terminal for API requests" -ForegroundColor Gray
