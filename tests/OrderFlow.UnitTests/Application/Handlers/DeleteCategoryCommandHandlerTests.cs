using FluentAssertions;
using OrderFlow.Application.Commands.DeleteCategory;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class DeleteCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCategory_ShouldRemoveAndReturnTrue()
    {
        using var context = TestDbContextFactory.Create();
        var category = Category.Create("Test", null);
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var handler = new DeleteCategoryCommandHandler(context);
        var result = await handler.Handle(
            new DeleteCategoryCommand(category.Id), default);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NonExistentCategory_ShouldReturnFalse()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new DeleteCategoryCommandHandler(context);

        var result = await handler.Handle(
            new DeleteCategoryCommand(Guid.NewGuid()), default);

        result.Should().BeFalse();
    }
}
