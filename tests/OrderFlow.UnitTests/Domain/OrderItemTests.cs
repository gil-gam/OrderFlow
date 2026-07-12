using FluentAssertions;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using Xunit;

namespace OrderFlow.UnitTests.Domain;

public sealed class OrderItemTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreate()
    {
        var productId = Guid.NewGuid();

        var item = new OrderItem(productId, "Product X", 3, new Money(10, "USD"));

        item.ProductId.Should().Be(productId);
        item.ProductName.Should().Be("Product X");
        item.Quantity.Should().Be(3);
        item.UnitPrice.Should().Be(new Money(10, "USD"));
        item.Subtotal().Should().Be(new Money(30, "USD"));
    }

    [Fact]
    public void Constructor_WithZeroQuantity_ShouldThrow() =>
        FluentActions.Invoking(() =>
            new OrderItem(Guid.NewGuid(), "Product", 0, new Money(10, "USD")))
            .Should().Throw<ArgumentException>().WithParameterName("quantity");

    [Fact]
    public void Constructor_WithNegativeQuantity_ShouldThrow() =>
        FluentActions.Invoking(() =>
            new OrderItem(Guid.NewGuid(), "Product", -1, new Money(10, "USD")))
            .Should().Throw<ArgumentException>().WithParameterName("quantity");

    [Fact]
    public void Constructor_WithNullProductName_ShouldThrow() =>
        FluentActions.Invoking(() =>
            new OrderItem(Guid.NewGuid(), null!, 1, new Money(10, "USD")))
            .Should().Throw<ArgumentNullException>().WithParameterName("productName");

    [Fact]
    public void Constructor_WithNullUnitPrice_ShouldThrow() =>
        FluentActions.Invoking(() =>
            new OrderItem(Guid.NewGuid(), "Product", 1, null!))
            .Should().Throw<ArgumentNullException>().WithParameterName("unitPrice");

    [Fact]
    public void Subtotal_ShouldCalculateCorrectly()
    {
        var item = new OrderItem(Guid.NewGuid(), "Product", 5, new Money(15.50m, "BRL"));
        item.Subtotal().Should().Be(new Money(77.50m, "BRL"));
    }

    [Fact]
    public void SetOrderId_ShouldAssignOrderId()
    {
        var item = new OrderItem(Guid.NewGuid(), "Product", 1, new Money(10, "USD"));
        var orderId = Guid.NewGuid();
        item.SetOrderId(orderId);
        item.OrderId.Should().Be(orderId);
    }
}
