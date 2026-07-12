using FluentAssertions;
using OrderFlow.Application.Queries.GetOrderById;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Queries;

public sealed class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingOrder_ShouldReturn()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            var order = Order.Create(Guid.NewGuid(), new Address("St", "City", "ST", "00000", "BR"));
            order.AddItem(Guid.NewGuid(), "Item", 2, new Money(50, "USD"));
            ctx.Orders.Add(order);
        });

        var handler = new GetOrderByIdQueryHandler(context);
        var result = await handler.Handle(
            new GetOrderByIdQuery(context.Orders.First().Id), default);

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ShouldReturnNull()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetOrderByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetOrderByIdQuery(Guid.NewGuid()), default);

        result.Should().BeNull();
    }
}
