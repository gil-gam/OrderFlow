using Moq;
using FluentAssertions;
using OrderFlow.Application.Commands.CancelOrder;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Tests.Commands;

public sealed class CancelOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new CancelOrderCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCancelExistingOrder()
    {
        var address = Address.Create("123 St", "City", "ST", "12345", "US");
        var order = Order.Create(Guid.NewGuid(), address);
        order.AddItem(Guid.NewGuid(), "Test", 1, Money.Create(100));

        _repositoryMock
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        await _handler.Handle(new CancelOrderCommand(order.Id), CancellationToken.None);

        order.Status.Should().Be(OrderFlow.Domain.Enums.OrderStatus.Cancelled);
        _repositoryMock.Verify(r => r.Update(order), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ShouldThrowKeyNotFoundException()
    {
        var orderId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var act = () => _handler.Handle(new CancelOrderCommand(orderId), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{orderId}*");
    }
}