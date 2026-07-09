using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Domain.Entities;

public sealed class OrderItem
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }

    private OrderItem() { } // EF Core

    public OrderItem(Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        ProductId = productId;
        ProductName = productName
            ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Money Subtotal()
        => Money.Create(Quantity * UnitPrice.Amount, UnitPrice.Currency);
}