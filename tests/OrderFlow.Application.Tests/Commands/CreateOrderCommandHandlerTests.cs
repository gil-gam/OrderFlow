using Moq;
using FluentAssertions;
using OrderFlow.Application.Commands.CreateOrder;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Tests.Commands;

public sealed class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new CreateOrderCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrderAndReturnId()
    {
        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Street: "123 Main St", City: "New York",
            State: "NY", ZipCode: "10001", Country: "USA",
            Items: new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "Product A", 2, 50m, "USD")
            });

        Order? capturedOrder = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((o, _) => capturedOrder = o)
            .Returns(Task.CompletedTask);
        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        capturedOrder.Should().NotBeNull();
        capturedOrder!.CustomerId.Should().Be(command.CustomerId);
        capturedOrder.Items.Should().HaveCount(1);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldAddAllItems()
    {
        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Street: "456 Oak Ave", City: "Los Angeles",
            State: "CA", ZipCode: "90001", Country: "USA",
            Items: new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "Item 1", 1, 100m, "USD"),
                new(Guid.NewGuid(), "Item 2", 3, 25.50m, "USD")
            });

        Order? capturedOrder = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((o, _) => capturedOrder = o)
            .Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        capturedOrder!.Items.Should().HaveCount(2);
        var subtotal = capturedOrder.Items.Sum(i => i.Subtotal().Amount);
        subtotal.Should().Be(100m + (3 * 25.50m));
    }
}