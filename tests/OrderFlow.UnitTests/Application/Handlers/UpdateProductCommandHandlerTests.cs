using FluentAssertions;
using OrderFlow.Application.Commands.UpdateProduct;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingProduct_ShouldUpdateAndReturn()
    {
        using var context = TestDbContextFactory.Create();
        var category = Category.Create("Cat", null);
        context.Categories.Add(category);
        var product = Product.Create("Old", "Old desc", new Money(100, "USD"), category.Id);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new UpdateProductCommandHandler(context);
        var result = await handler.Handle(
            new UpdateProductCommand(product.Id, "New", "New desc", 200, "BRL", category.Id), default);

        result.Should().NotBeNull();
        result!.Name.Should().Be("New");
        result.UnitPrice.Should().Be(200);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ShouldReturnNull()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new UpdateProductCommandHandler(context);

        var result = await handler.Handle(
            new UpdateProductCommand(Guid.NewGuid(), "N", "D", 10, "USD", Guid.NewGuid()), default);

        result.Should().BeNull();
    }
}