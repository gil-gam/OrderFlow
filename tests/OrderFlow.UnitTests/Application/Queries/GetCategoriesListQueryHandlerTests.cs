using FluentAssertions;
using OrderFlow.Application.Queries.GetCategoriesList;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Queries;

public sealed class GetCategoriesListQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAll()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            ctx.Categories.Add(Category.Create("A", null));
            ctx.Categories.Add(Category.Create("B", null));
        });

        var handler = new GetCategoriesListQueryHandler(context);
        var result = await handler.Handle(new GetCategoriesListQuery(), default);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithActiveOnly_ShouldFilter()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            var active = Category.Create("Active", null);
            var inactive = Category.Create("Inactive", null);
            inactive.Deactivate();
            ctx.Categories.AddRange(active, inactive);
        });

        var handler = new GetCategoriesListQueryHandler(context);
        var result = await handler.Handle(new GetCategoriesListQuery(ActiveOnly: true), default);

        result.Should().ContainSingle(c => c.Name == "Active");
    }
}
