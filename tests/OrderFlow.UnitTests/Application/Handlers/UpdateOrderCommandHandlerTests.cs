using FluentAssertions;
using OrderFlow.Application.Commands.UpdateOrder;
using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class UpdateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingOrder_ShouldUpdateItems()
    {
        using var context = TestDbContextFactory.Create();
        var customer = new Customer(Guid.NewGuid(), "Test", "test@email.com");
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var order = Order.Create(customer.Id, new Address("St", "City", "ST", "00000", "BR"));
        var productId = Guid.NewGuid();
        order.AddItem(productId, "Old Item", 1, new Money(10, "USD"));
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var handler = new UpdateOrderCommandHandler(context);
        var result = await handler.Handle(
            new UpdateOrderCommand(
                order.Id, "New St", "New City", "NS", "11111", "BR",
                new List<UpdateOrderItemDto>
                {
                    new(productId, "Updated Item", 5, 25m, "USD")
                }), default);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ShouldReturnFalse()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new UpdateOrderCommandHandler(context);

        var result = await handler.Handle(
            new UpdateOrderCommand(Guid.NewGuid(), "St", "City", "ST", "00000", "BR",
                new List<UpdateOrderItemDto>()), default);

        result.Should().BeFalse();
    }
}