using FluentAssertions;
using OrderFlow.Application.Queries.GetProductById;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Queries;

public sealed class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingProduct_ShouldReturn()
    {
        using var context = TestDbContextFactory.CreateWithSeed(ctx =>
        {
            var cat = Category.Create("Cat", null);
            ctx.Categories.Add(cat);
            ctx.SaveChanges();
            ctx.Products.Add(Product.Create("Notebook", "Laptop", new Money(5000, "BRL"), cat.Id));
        });

        var handler = new GetProductByIdQueryHandler(context);
        var result = await handler.Handle(
            new GetProductByIdQuery(context.Products.First().Id), default);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Notebook");
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ShouldReturnNull()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetProductByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetProductByIdQuery(Guid.NewGuid()), default);

        result.Should().BeNull();
    }
}
