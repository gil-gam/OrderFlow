using FluentAssertions;
using OrderFlow.Application.Commands.UpdateCustomer;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class UpdateCustomerCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCustomer_ShouldUpdateAndReturn()
    {
        using var context = TestDbContextFactory.Create();
        var customer = new Customer(Guid.NewGuid(), "Old", "old@email.com");
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var handler = new UpdateCustomerCommandHandler(context);
        var result = await handler.Handle(
            new UpdateCustomerCommand(customer.Id, "New", "new@email.com", "11988888888"), default);

        result.Should().NotBeNull();
        result!.Name.Should().Be("New");
        result.Email.Should().Be("new@email.com");
        result.Phone.Should().Be("11988888888");
    }

    [Fact]
    public async Task Handle_NonExistentCustomer_ShouldReturnNull()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new UpdateCustomerCommandHandler(context);

        var result = await handler.Handle(
            new UpdateCustomerCommand(Guid.NewGuid(), "Name", "e@m.com", null), default);

        result.Should().BeNull();
    }
}
