using FluentAssertions;
using OrderFlow.Application.Commands.CreateProduct;
using OrderFlow.Domain.Entities;
using OrderFlow.UnitTests.Application.Helpers;
using Xunit;

namespace OrderFlow.UnitTests.Application.Handlers;

public sealed class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCategory_ShouldCreateAndReturnProductDto()
    {
        using var context = TestDbContextFactory.Create();
        var category = Category.Create("Tech", null);
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(context);
        var result = await handler.Handle(
            new CreateProductCommand("Notebook", "Laptop", 5000, "BRL", category.Id), default);

        result.Should().NotBeNull();
        result.Name.Should().Be("Notebook");
        result.UnitPrice.Should().Be(5000);
        result.Currency.Should().Be("BRL");
        result.CategoryId.Should().Be(category.Id);
    }

    [Fact]
    public async Task Handle_NonExistentCategory_ShouldThrowValidationException()
    {
        using var context = TestDbContextFactory.Create();

        var handler = new CreateProductCommandHandler(context);

        await FluentActions.Invoking(() =>
            handler.Handle(
                new CreateProductCommand("P", "D", 10, "USD", Guid.NewGuid()), default))
            .Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
