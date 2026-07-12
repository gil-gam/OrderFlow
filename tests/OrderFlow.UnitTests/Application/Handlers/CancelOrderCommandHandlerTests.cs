using FluentAssertions;
using Moq;
using OrderFlow.Application.Commands.CancelOrder;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class CancelOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingOrder_ShouldCancel()
    {
        var address = new Address("St", "City", "ST", "00000", "BR");
        var order = Order.Create(Guid.NewGuid(), address);
        order.ClearDomainEvents();

        var repo = new Mock<IOrderRepository>();
        repo.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new CancelOrderCommandHandler(repo.Object);
        await handler.Handle(new CancelOrderCommand(order.Id), default);

        repo.Verify(r => r.Update(order), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        order.Status.Should().Be(OrderFlow.Domain.Enums.OrderStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ShouldThrowKeyNotFound()
    {
        var repo = new Mock<IOrderRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler = new CancelOrderCommandHandler(repo.Object);

        await FluentActions.Invoking(() =>
            handler.Handle(new CancelOrderCommand(Guid.NewGuid()), default))
            .Should().ThrowAsync<KeyNotFoundException>();
    }
}
