using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Domain.Entities;

public sealed class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;

    private OrderItem() { } // EF Core

    public OrderItem(Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName
            ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
    }

    internal void SetOrderId(Guid orderId)
    {
        OrderId = orderId;
    }

    internal void Update(string productName, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
    }

    public Money Subtotal()
        => new Money(Quantity * UnitPrice.Amount, UnitPrice.Currency);
}