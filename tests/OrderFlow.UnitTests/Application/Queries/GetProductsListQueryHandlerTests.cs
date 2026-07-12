using FluentAssertions;
using OrderFlow.Application.Queries.GetProductsList;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Queries;

public sealed class GetProductsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAll()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            var cat = Category.Create("Cat", null);
            ctx.Categories.Add(cat);
            ctx.SaveChanges();
            ctx.Products.Add(Product.Create("P1", "D1", new Money(10, "USD"), cat.Id));
            ctx.Products.Add(Product.Create("P2", "D2", new Money(20, "USD"), cat.Id));
        });

        var handler = new GetProductsListQueryHandler(context);
        var result = await handler.Handle(new GetProductsListQuery(), default);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ShouldFilter()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            var cat1 = Category.Create("Cat1", null);
            var cat2 = Category.Create("Cat2", null);
            ctx.Categories.AddRange(cat1, cat2);
            ctx.SaveChanges();
            ctx.Products.Add(Product.Create("P1", "D1", new Money(10, "USD"), cat1.Id));
            ctx.Products.Add(Product.Create("P2", "D2", new Money(20, "USD"), cat2.Id));
        });

        var cat1Id = context.Categories.First(c => c.Name == "Cat1").Id;
        var handler = new GetProductsListQueryHandler(context);
        var result = await handler.Handle(
            new GetProductsListQuery(CategoryId: cat1Id), default);

        result.Should().ContainSingle(p => p.Name == "P1");
    }

    [Fact]
    public async Task Handle_WithActiveOnly_ShouldFilter()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            var cat = Category.Create("Cat", null);
            ctx.Categories.Add(cat);
            ctx.SaveChanges();
            var active = Product.Create("Active", "D", new Money(10, "USD"), cat.Id);
            var inactive = Product.Create("Inactive", "D", new Money(10, "USD"), cat.Id);
            inactive.Deactivate();
            ctx.Products.AddRange(active, inactive);
        });

        var handler = new GetProductsListQueryHandler(context);
        var result = await handler.Handle(
            new GetProductsListQuery(ActiveOnly: true), default);

        result.Should().ContainSingle(p => p.Name == "Active");
    }
}
