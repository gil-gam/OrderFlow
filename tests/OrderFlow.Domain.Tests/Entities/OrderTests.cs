using FluentAssertions;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.Domain.Events;

namespace OrderFlow.Domain.Tests.Entities;

public sealed class OrderTests
{
    private static readonly Address DefaultAddress = Address.Create(
        "123 Main St", "New York", "NY", "10001", "USA");
    private static readonly Money DefaultPrice = Money.Create(100);
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid ProductId2 = Guid.NewGuid();

    [Fact]
    public void Create_ShouldSetInitialProperties()
    {
        var order = Order.Create(CustomerId, DefaultAddress);

        order.CustomerId.Should().Be(CustomerId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().BeEmpty();
        order.TotalAmount.Amount.Should().Be(0);
        order.OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldRaiseOrderCreatedEvent()
    {
        var order = Order.Create(CustomerId, DefaultAddress);

        order.DomainEvents.Should().ContainSingle(e
            => e is OrderCreatedDomainEvent);
    }

    [Fact]
    public void AddItem_ShouldIncludeInItemsAndRecalculateTotal()
    {
        var order = Order.Create(CustomerId, DefaultAddress);
        order.AddItem(ProductId, "Test Product", 2, DefaultPrice);

        order.Items.Should().HaveCount(1);
        order.Items[0].Quantity.Should().Be(2);
        order.TotalAmount.Amount.Should().Be(200);
    }

    [Fact]
    public void AddItem_DuplicateProduct_ShouldMergeQuantities()
    {
        var order = Order.Create(CustomerId, DefaultAddress);
        order.AddItem(ProductId, "Test Product", 2, DefaultPrice);
        order.AddItem(ProductId, "Test Product", 3, DefaultPrice);

        order.Items.Should().HaveCount(1);
        order.Items[0].Quantity.Should().Be(5);
        order.TotalAmount.Amount.Should().Be(500);
    }

    [Fact]
    public void RemoveItem_ShouldExcludeFromItems()
    {
        var order = Order.Create(CustomerId, DefaultAddress);
        order.AddItem(ProductId, "Test Product", 1, DefaultPrice);
        order.RemoveItem(ProductId);

        order.Items.Should().BeEmpty();
        order.TotalAmount.Amount.Should().Be(0);
    }

    [Fact]
    public void ApplyDiscount_ShouldReduceTotal()
    {
        var order = Order.Create(CustomerId, DefaultAddress);
        order.AddItem(ProductId, "Test", 10, DefaultPrice);
        order.ApplyDiscount(10);

        order.TotalAmount.Amount.Should().Be(900);
    }

    [Fact]
    public void ApplyDiscount_Over30Percent_ShouldThrow()
    {
        var order = Order.Create(CustomerId, DefaultAddress);

        order.Invoking(o => o.ApplyDiscount(50))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Confirm_WithoutItems_ShouldThrow()
    {
        var order = Order.Create(CustomerId, DefaultAddress);

        order.Invoking(o => o.Confirm())
            .Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Confirm_WithItems_ShouldSetStatusToConfirmed()
    {
        var order = Order.Create(CustomerId, DefaultAddress);
        order.AddItem(ProductId, "Test", 1, DefaultPrice);
        order.Confirm();

        order.Status.Should().Be(OrderStatus.Confirmed);
        order.DomainEvents.Should().Contain(e => e is OrderConfirmedDomainEvent);
    }

    [Fact]
    public void Cancel_OnPendingOrder_ShouldSetCancelled()
    {
        var order = Order.Create(CustomerId, DefaultAddress);
        order.AddItem(ProductId, "Test", 1, DefaultPrice);
        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().Contain(e => e is OrderCancelledDomainEvent);
    }

    [Fact]
    public void Cancel_OnShippedOrder_ShouldThrow()
    {
        var order = Order.Create(CustomerId, DefaultAddress);
        order.AddItem(ProductId, "Test", 1, DefaultPrice);

        // Reflection para setar status como Shipped e testar a guarda
        var statusProperty = typeof(Order).GetProperty("Status");
        statusProperty!.SetValue(order, OrderStatus.Shipped);

        order.Invoking(o => o.Cancel())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*shipped*");
    }

    [Fact]
    public void Cancel_OnDeliveredOrder_ShouldThrow()
    {
        var order = Order.Create(CustomerId, DefaultAddress);
        order.AddItem(ProductId, "Test", 1, DefaultPrice);

        var statusProperty = typeof(Order).GetProperty("Status");
        statusProperty!.SetValue(order, OrderStatus.Delivered);

        order.Invoking(o => o.Cancel())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*shipped*");
    }
}