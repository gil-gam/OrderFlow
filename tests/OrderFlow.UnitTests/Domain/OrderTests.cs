using FluentAssertions;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.Events;
using OrderFlow.Domain.ValueObjects;
using Xunit;

namespace OrderFlow.UnitTests.Domain;

public sealed class OrderTests
{
    private static readonly Address ValidAddress =
        new("Rua A", "São Paulo", "SP", "01001", "Brasil");

    private static readonly Money TenUsd = new(10, "USD");

    [Fact]
    public void Create_ShouldReturnPendingOrderWithDomainEvent()
    {
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId, ValidAddress);

        order.CustomerId.Should().Be(customerId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.ShippingAddress.Should().Be(ValidAddress);
        order.TotalAmount.Should().Be(new Money(0, "USD"));
        order.Items.Should().BeEmpty();
        order.DomainEvents.Should().ContainItemsAssignableTo<OrderCreatedDomainEvent>();
    }

    [Fact]
    public void Create_WithNullAddress_ShouldThrow() =>
        FluentActions.Invoking(() => Order.Create(Guid.NewGuid(), null!))
            .Should().Throw<ArgumentNullException>();

    [Fact]
    public void AddItem_NewProduct_ShouldAddItem()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        var productId = Guid.NewGuid();

        order.AddItem(productId, "Product A", 2, TenUsd);

        order.Items.Should().HaveCount(1);
        order.Items.First().ProductId.Should().Be(productId);
        order.Items.First().Quantity.Should().Be(2);
        order.TotalAmount.Should().Be(new Money(20, "USD"));
    }

    [Fact]
    public void AddItem_ExistingProduct_ShouldAccumulateQuantity()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        var productId = Guid.NewGuid();

        order.AddItem(productId, "Product A", 2, TenUsd);
        order.AddItem(productId, "Product A", 3, TenUsd);

        order.Items.Should().HaveCount(1);
        order.Items.First().Quantity.Should().Be(5);
        order.TotalAmount.Should().Be(new Money(50, "USD"));
    }

    [Fact]
    public void AddItem_OnConfirmedOrder_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        order.AddItem(Guid.NewGuid(), "Product", 1, TenUsd);
        order.Confirm();

        FluentActions.Invoking(() => order.AddItem(Guid.NewGuid(), "Another", 1, TenUsd))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*non-pending*");
    }

    [Fact]
    public void RemoveItem_ShouldRemoveAndRecalculate()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        var productId = Guid.NewGuid();
        order.AddItem(productId, "Product", 2, new Money(50, "USD"));
        order.AddItem(Guid.NewGuid(), "Another", 1, new Money(100, "USD"));

        order.RemoveItem(productId);

        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(new Money(100, "USD"));
    }

    [Fact]
    public void RemoveItem_NonExistentProduct_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);

        FluentActions.Invoking(() => order.RemoveItem(Guid.NewGuid()))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public void RemoveItem_OnNonPendingOrder_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        order.AddItem(Guid.NewGuid(), "Product", 1, TenUsd);
        order.Confirm();

        FluentActions.Invoking(() => order.RemoveItem(Guid.NewGuid()))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*non-pending*");
    }

    [Fact]
    public void ApplyDiscount_WithValidPercentage_ShouldReduceTotal()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        order.AddItem(Guid.NewGuid(), "Product", 10, TenUsd);

        order.ApplyDiscount(10); // 10% de 100 = 10

        order.TotalAmount.Should().Be(new Money(90, "USD"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(31)]
    public void ApplyDiscount_WithInvalidPercentage_ShouldThrow(decimal percentage)
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        order.AddItem(Guid.NewGuid(), "Product", 1, TenUsd);

        FluentActions.Invoking(() => order.ApplyDiscount(percentage))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Confirm_ShouldChangeStatusAndRaiseEvent()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        order.AddItem(Guid.NewGuid(), "Product", 1, TenUsd);
        order.ClearDomainEvents();

        order.Confirm();

        order.Status.Should().Be(OrderStatus.Confirmed);
        order.DomainEvents.Should().ContainItemsAssignableTo<OrderConfirmedDomainEvent>();
    }

    [Fact]
    public void Confirm_OnNonPendingOrder_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        order.AddItem(Guid.NewGuid(), "Product", 1, TenUsd);
        order.Confirm();

        FluentActions.Invoking(() => order.Confirm())
            .Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Confirm_OnEmptyOrder_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);

        FluentActions.Invoking(() => order.Confirm())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void Cancel_ShouldChangeStatusAndRaiseEvent()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        order.ClearDomainEvents();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().ContainItemsAssignableTo<OrderCancelledDomainEvent>();
    }

    [Fact]
    public void Cancel_OnShippedOrder_ShouldThrow()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        order.AddItem(Guid.NewGuid(), "Product", 1, TenUsd);
        order.Confirm();

        // Reflectively set to Shipped to test the guard (simulating progression)
        typeof(Order).GetProperty(nameof(Order.Status))!
            .SetValue(order, OrderStatus.Shipped);

        FluentActions.Invoking(() => order.Cancel())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*shipped*");
    }

    [Fact]
    public void ClearDomainEvents_ShouldEmptyList()
    {
        var order = Order.Create(Guid.NewGuid(), ValidAddress);
        order.ClearDomainEvents();
        order.DomainEvents.Should().BeEmpty();
    }
}