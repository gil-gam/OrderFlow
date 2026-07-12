using FluentAssertions;
using OrderFlow.Application.Events;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Events;

public sealed class CustomerCreatedEventHandlerTests
{
    [Fact]
    public async Task Handle_NewUser_ShouldCreateCustomer()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new CustomerCreatedEventHandler(context);

        var userId = Guid.NewGuid();
        await handler.Handle(new UserCreatedEvent(userId, "John", "john@email.com"), default);

        var customer = await Task.FromResult(context.Customers.FirstOrDefault(c => c.UserExternalId == userId));
        customer.Should().NotBeNull();
        customer!.Name.Should().Be("John");
    }

    [Fact]
    public async Task Handle_ExistingUser_ShouldSkip()
    {
        var userId = Guid.NewGuid();
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            ctx.Customers.Add(new Customer(userId, "Existing", "existing@email.com"));
        });

        var handler = new CustomerCreatedEventHandler(context);
        await handler.Handle(new UserCreatedEvent(userId, "John", "john@email.com"), default);

        var customers = context.Customers.Where(c => c.UserExternalId == userId).ToList();
        customers.Should().HaveCount(1);
        customers[0].Name.Should().Be("Existing");
    }
}