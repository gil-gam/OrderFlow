using FluentAssertions;
using OrderFlow.Application.Commands.DeleteOrder;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class DeleteOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingOrder_ShouldRemoveAndReturnTrue()
    {
        using var context = TestDbContextFactory.Create();
        var order = Order.Create(Guid.NewGuid(), new Address("St", "City", "ST", "00000", "BR"));
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var handler = new DeleteOrderCommandHandler(context);
        var result = await handler.Handle(
            new DeleteOrderCommand(order.Id), default);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ShouldReturnFalse()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new DeleteOrderCommandHandler(context);

        var result = await handler.Handle(
            new DeleteOrderCommand(Guid.NewGuid()), default);

        result.Should().BeFalse();
    }
}
