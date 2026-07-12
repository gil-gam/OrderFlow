using FluentAssertions;
using OrderFlow.Application.Commands.DeleteProduct;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingProduct_ShouldRemoveAndReturnTrue()
    {
        using var context = TestDbContextFactory.Create();
        var category = Category.Create("Cat", null);
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var product = Product.Create("Test", "Desc", new Money(10, "USD"), category.Id);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new DeleteProductCommandHandler(context);
        var result = await handler.Handle(
            new DeleteProductCommand(product.Id), default);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ShouldReturnFalse()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new DeleteProductCommandHandler(context);

        var result = await handler.Handle(
            new DeleteProductCommand(Guid.NewGuid()), default);

        result.Should().BeFalse();
    }
}