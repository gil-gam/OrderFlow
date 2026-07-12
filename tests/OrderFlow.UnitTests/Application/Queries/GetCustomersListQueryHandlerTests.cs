using FluentAssertions;
using OrderFlow.Application.Queries.GetCustomersList;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Queries;

public sealed class GetCustomersListQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAll()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            ctx.Customers.Add(new Customer(Guid.NewGuid(), "A", "a@email.com"));
            ctx.Customers.Add(new Customer(Guid.NewGuid(), "B", "b@email.com"));
        });

        var handler = new GetCustomersListQueryHandler(context);
        var result = await handler.Handle(new GetCustomersListQuery(), default);

        result.Should().HaveCount(2);
    }
}
