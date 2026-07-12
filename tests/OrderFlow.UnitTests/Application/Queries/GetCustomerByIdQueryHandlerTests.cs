using FluentAssertions;
using OrderFlow.Application.Queries.GetCustomerById;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Queries;

public sealed class GetCustomerByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCustomer_ShouldReturn()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            ctx.Customers.Add(new Customer(Guid.NewGuid(), "John", "john@email.com"));
        });

        var handler = new GetCustomerByIdQueryHandler(context);
        var result = await handler.Handle(
            new GetCustomerByIdQuery(context.Customers.First().Id), default);

        result.Should().NotBeNull();
        result!.Name.Should().Be("John");
    }

    [Fact]
    public async Task Handle_NonExistentCustomer_ShouldReturnNull()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetCustomerByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetCustomerByIdQuery(Guid.NewGuid()), default);

        result.Should().BeNull();
    }
}