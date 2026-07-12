using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Commands.UpdateCategory;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class UpdateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCategory_ShouldUpdateAndReturn()
    {
        using var context = TestDbContextFactory.Create();
        var category = Category.Create("Old", "Old desc");
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var handler = new UpdateCategoryCommandHandler(context);
        var result = await handler.Handle(
            new UpdateCategoryCommand(category.Id, "New", "New desc"), default);

        result.Should().NotBeNull();
        result!.Name.Should().Be("New");
        result.Description.Should().Be("New desc");
    }

    [Fact]
    public async Task Handle_NonExistentCategory_ShouldReturnNull()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new UpdateCategoryCommandHandler(context);

        var result = await handler.Handle(
            new UpdateCategoryCommand(Guid.NewGuid(), "Name", null), default);

        result.Should().BeNull();
    }
}