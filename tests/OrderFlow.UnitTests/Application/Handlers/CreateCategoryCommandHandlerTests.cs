using FluentAssertions;
using OrderFlow.Application.Commands.CreateCategory;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateAndReturnCategoryDto()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new CreateCategoryCommandHandler(context);

        var result = await handler.Handle(
            new CreateCategoryCommand("Eletrônicos", "Produtos eletrônicos"), default);

        result.Should().NotBeNull();
        result.Name.Should().Be("Eletrônicos");
        result.Description.Should().Be("Produtos eletrônicos");
        result.IsActive.Should().BeTrue();
    }
}
