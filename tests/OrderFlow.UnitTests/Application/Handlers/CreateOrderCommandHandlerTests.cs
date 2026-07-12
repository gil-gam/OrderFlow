using FluentAssertions;
using Moq;
using OrderFlow.Application.Commands.CreateOrder;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class CreateOrderCommandHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public async Task Handle_WithValidCustomer_ShouldCreateOrder()
    {
        using var context = TestDbContextFactory.Create();
        var customer = new Customer(UserId, "John", "john@email.com");
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(UserId);

        var handler = new CreateOrderCommandHandler(context, currentUser.Object);
        var result = await handler.Handle(
            new CreateOrderCommand(
                "Rua A", "São Paulo", "SP", "01001", "Brasil",
                new List<CreateOrderItemDto>
                {
                    new(Guid.NewGuid(), "Item X", 2, 50m, "BRL")
                }), default);

        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WithoutCustomerProfile_ShouldThrowUnauthorized()
    {
        using var context = TestDbContextFactory.Create();
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var handler = new CreateOrderCommandHandler(context, currentUser.Object);

        await FluentActions.Invoking(() =>
            handler.Handle(
                new CreateOrderCommand("Rua", "Cidade", "ST", "00000", "BR",
                    new List<CreateOrderItemDto>()), default))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
