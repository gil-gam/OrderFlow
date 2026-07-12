using FluentAssertions;
using Moq;
using OrderFlow.Application.Commands.CreateCustomer;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class CreateCustomerCommandHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public async Task Handle_ShouldCreateAndReturnCustomerDto()
    {
        using var context = TestDbContextFactory.Create();
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(UserId);

        var handler = new CreateCustomerCommandHandler(context, currentUser.Object);
        var result = await handler.Handle(
            new CreateCustomerCommand("John Doe", "john@email.com", null), default);

        result.Should().NotBeNull();
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@email.com");
        result.UserExternalId.Should().Be(UserId);
        result.Phone.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithPhone_ShouldIncludePhone()
    {
        using var context = TestDbContextFactory.Create();
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.UserId).Returns(UserId);

        var handler = new CreateCustomerCommandHandler(context, currentUser.Object);
        var result = await handler.Handle(
            new CreateCustomerCommand("Jane", "jane@email.com", "11999999999"), default);

        result.Phone.Should().Be("11999999999");
    }
}
