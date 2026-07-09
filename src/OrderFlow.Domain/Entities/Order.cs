using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Events;
using OrderFlow.Domain.ValueObjects;
using MediatR;

namespace OrderFlow.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items = new();
    private readonly List<INotification> _domainEvents = new();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public List<OrderItem> Items => _items;
    public Address ShippingAddress { get; private set; } = null!;
    public Money TotalAmount { get; private set; }
    public Money DiscountApplied { get; private set; }
    public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

    private Order() { } // EF Core

    public static Order Create(Guid customerId, Address shippingAddress)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddress = shippingAddress
                ?? throw new ArgumentNullException(nameof(shippingAddress)),
            TotalAmount = Money.Create(0),
            DiscountApplied = Money.Create(0)
        };

        order._domainEvents.Add(
            new OrderCreatedDomainEvent(order.Id, customerId, order.OrderDate));

        return order;
    }

    public void AddItem(Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot add items to a non-pending order.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            _items.Remove(existing);
            _items.Add(new OrderItem(
                productId, productName, existing.Quantity + quantity, unitPrice));
        }
        else
        {
            _items.Add(new OrderItem(productId, productName, quantity, unitPrice));
        }

        RecalculateTotal();
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot remove items from a non-pending order.");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            throw new InvalidOperationException("Item not found in order.");

        _items.Remove(item);
        RecalculateTotal();
    }

    public void ApplyDiscount(decimal percentage)
    {
        if (percentage < 0 || percentage > 30)
            throw new ArgumentException("Discount must be between 0 and 30%.");

        var subtotal = _items.Sum(i => i.Subtotal().Amount);
        var discountAmount = subtotal * (percentage / 100m);
        DiscountApplied = Money.Create(discountAmount, TotalAmount.Currency);
        RecalculateTotal();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");
        if (!_items.Any())
            throw new InvalidOperationException("Cannot confirm an empty order.");

        Status = OrderStatus.Confirmed;
        _domainEvents.Add(new OrderConfirmedDomainEvent(Id));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new InvalidOperationException(
                "Cannot cancel shipped or delivered orders.");

        Status = OrderStatus.Cancelled;
        _domainEvents.Add(new OrderCancelledDomainEvent(Id));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void RecalculateTotal()
    {
        var subtotal = _items.Sum(i => i.Subtotal().Amount);
        TotalAmount = Money.Create(
            Math.Max(0, subtotal - DiscountApplied.Amount), TotalAmount.Currency);
    }
}