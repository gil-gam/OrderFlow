using FluentAssertions;
using OrderFlow.Domain.Entities;
using Xunit;

namespace OrderFlow.UnitTests.Domain;

public sealed class CategoryTests
{
	[Fact]
	public void Create_WithValidData_ShouldReturnCategory()
	{
		var category = Category.Create("Eletrônicos", "Produtos eletrônicos em geral");

		category.Name.Should().Be("Eletrônicos");
		category.Description.Should().Be("Produtos eletrônicos em geral");
		category.IsActive.Should().BeTrue();
		category.Id.Should().NotBe(Guid.Empty);
	}

	[Fact]
	public void Create_WithNullName_ShouldThrow() =>
		FluentActions.Invoking(() => Category.Create(null!, "desc"))
			.Should().Throw<ArgumentException>().WithParameterName("name");

	[Fact]
	public void Create_WithEmptyName_ShouldThrow() =>
		FluentActions.Invoking(() => Category.Create("", "desc"))
			.Should().Throw<ArgumentException>().WithParameterName("name");

	[Fact]
	public void Create_WithNameOver100_ShouldThrow() =>
		FluentActions.Invoking(() => Category.Create(new string('A', 101), "desc"))
			.Should().Throw<ArgumentException>();

	[Fact]
	public void Create_WithDescriptionOver500_ShouldThrow() =>
		FluentActions.Invoking(() => Category.Create("Valid", new string('A', 501)))
			.Should().Throw<ArgumentException>();

	[Fact]
	public void Update_ShouldChangeProperties()
	{
		var category = Category.Create("Old", "Old desc");

		category.Update("New", "New desc");

		category.Name.Should().Be("New");
		category.Description.Should().Be("New desc");
	}

	[Fact]
	public void Update_WithInvalidName_ShouldThrow()
	{
		var category = Category.Create("Valid", null);

		FluentActions.Invoking(() => category.Update("", null))
			.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void Deactivate_ShouldSetIsActiveFalse()
	{
		var category = Category.Create("Test", null);
		category.Deactivate();
		category.IsActive.Should().BeFalse();
	}

	[Fact]
	public void Activate_ShouldSetIsActiveTrue()
	{
		var category = Category.Create("Test", null);
		category.Deactivate();
		category.Activate();
		category.IsActive.Should().BeTrue();
	}
}
