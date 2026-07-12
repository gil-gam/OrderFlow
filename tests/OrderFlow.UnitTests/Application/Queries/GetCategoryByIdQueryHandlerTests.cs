using FluentAssertions;
using OrderFlow.Application.Queries.GetCategoryById;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Queries;

public sealed class GetCategoryByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCategory_ShouldReturn()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            ctx.Categories.Add(Category.Create("Tech", null));
        });

        var handler = new GetCategoryByIdQueryHandler(context);
        var result = await handler.Handle(
            new GetCategoryByIdQuery(context.Categories.First().Id), default);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_NonExistentCategory_ShouldReturnNull()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetCategoryByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetCategoryByIdQuery(Guid.NewGuid()), default);

        result.Should().BeNull();
    }
}
