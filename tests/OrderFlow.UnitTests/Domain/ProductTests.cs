using FluentAssertions;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;
using Xunit;

namespace OrderFlow.UnitTests.Domain;

public sealed class ProductTests
{
    private static readonly Money ValidPrice = new(100, "USD");
    private static readonly Guid ValidCategoryId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldReturnProduct()
    {
        var product = Product.Create("Notebook", "Laptop", ValidPrice, ValidCategoryId);

        product.Name.Should().Be("Notebook");
        product.Description.Should().Be("Laptop");
        product.UnitPrice.Should().Be(ValidPrice);
        product.CategoryId.Should().Be(ValidCategoryId);
        product.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidName_ShouldThrow(string? name) =>
        FluentActions.Invoking(() => Product.Create(name!, "desc", ValidPrice, ValidCategoryId))
            .Should().Throw<ArgumentException>().WithParameterName("name");

    [Fact]
    public void Create_WithNameOver200_ShouldThrow() =>
        FluentActions.Invoking(() =>
            Product.Create(new string('A', 201), "desc", ValidPrice, ValidCategoryId))
            .Should().Throw<ArgumentException>();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidDescription_ShouldThrow(string? desc) =>
        FluentActions.Invoking(() => Product.Create("Name", desc!, ValidPrice, ValidCategoryId))
            .Should().Throw<ArgumentException>().WithParameterName("description");

    [Fact]
    public void Create_WithNullUnitPrice_ShouldThrow() =>
        FluentActions.Invoking(() => Product.Create("Name", "desc", null!, ValidCategoryId))
            .Should().Throw<ArgumentException>().WithParameterName("unitPrice");

    [Fact]
    public void Create_WithNegativePrice_ShouldThrow() =>
        FluentActions.Invoking(() =>
            Product.Create("Name", "desc", new Money(-1, "USD"), ValidCategoryId))
            .Should().Throw<ArgumentException>().WithParameterName("amount");

    [Fact]
    public void Create_WithEmptyCategoryId_ShouldThrow() =>
        FluentActions.Invoking(() => Product.Create("Name", "desc", ValidPrice, Guid.Empty))
            .Should().Throw<ArgumentException>().WithParameterName("categoryId");

    [Fact]
    public void Update_ShouldChangeProperties()
    {
        var product = Product.Create("Old", "Old desc", ValidPrice, ValidCategoryId);
        var newPrice = new Money(200, "BRL");
        var newCategoryId = Guid.NewGuid();

        product.Update("New", "New desc", newPrice, newCategoryId);

        product.Name.Should().Be("New");
        product.Description.Should().Be("New desc");
        product.UnitPrice.Should().Be(newPrice);
        product.CategoryId.Should().Be(newCategoryId);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var product = Product.Create("Name", "desc", ValidPrice, ValidCategoryId);
        product.Deactivate();
        product.IsActive.Should().BeFalse();
    }
}