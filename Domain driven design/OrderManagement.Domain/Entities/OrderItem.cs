using OrderManagement.Domain.Common;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public class OrderItem : Entity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice { get; private set; }

    private OrderItem() : base()
    {
    }

    private OrderItem(Guid productId, string productName, int quantity, Money unitPrice)
        : base()
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = unitPrice.Multiply(quantity);
    }

    public static OrderItem Create(Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("ProductName cannot be empty", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice == null)
            throw new ArgumentNullException(nameof(unitPrice));

        return new OrderItem(productId, productName, quantity, unitPrice);
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
        TotalPrice = UnitPrice.Multiply(newQuantity);
    }
}
