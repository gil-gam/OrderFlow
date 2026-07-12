using FluentAssertions;
using OrderFlow.Application.Commands.DeleteCustomer;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class DeleteCustomerCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCustomer_ShouldRemoveAndReturnTrue()
    {
        using var context = TestDbContextFactory.Create();
        var customer = new Customer(Guid.NewGuid(), "Test", "test@email.com");
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var handler = new DeleteCustomerCommandHandler(context);
        var result = await handler.Handle(
            new DeleteCustomerCommand(customer.Id), default);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NonExistentCustomer_ShouldReturnFalse()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new DeleteCustomerCommandHandler(context);

        var result = await handler.Handle(
            new DeleteCustomerCommand(Guid.NewGuid()), default);

        result.Should().BeFalse();
    }
}