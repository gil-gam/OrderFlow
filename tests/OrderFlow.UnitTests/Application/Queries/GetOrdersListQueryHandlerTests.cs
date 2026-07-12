using FluentAssertions;
using OrderFlow.Application.Queries.GetOrdersList;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Queries;

public sealed class GetOrdersListQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnPagedResult()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            var address = new Address("St", "City", "ST", "00000", "BR");
            ctx.Orders.Add(Order.Create(Guid.NewGuid(), address));
            ctx.Orders.Add(Order.Create(Guid.NewGuid(), address));
        });

        var handler = new GetOrdersListQueryHandler(context);
        var result = await handler.Handle(new GetOrdersListQuery(PageIndex: 1, PageSize: 10), default);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithCustomerFilter_ShouldFilter()
    {
        var customerId = Guid.NewGuid();
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            var address = new Address("St", "City", "ST", "00000", "BR");
            ctx.Orders.Add(Order.Create(customerId, address));
            ctx.Orders.Add(Order.Create(Guid.NewGuid(), address));
        });

        var handler = new GetOrdersListQueryHandler(context);
        var result = await handler.Handle(
            new GetOrdersListQuery(PageIndex: 1, PageSize: 10, CustomerId: customerId), default);

        result.Items.Should().HaveCount(1);
    }
}
