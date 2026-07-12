using FluentAssertions;
using OrderFlow.Domain.ValueObjects;
using Xunit;

namespace OrderFlow.UnitTests.Domain;

public sealed class AddressTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreate()
    {
        var address = new Address("Rua A", "São Paulo", "SP", "01001", "Brasil");

        address.Street.Should().Be("Rua A");
        address.City.Should().Be("São Paulo");
        address.State.Should().Be("SP");
        address.ZipCode.Should().Be("01001");
        address.Country.Should().Be("Brasil");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithInvalidStreet_ShouldThrow(string? street) =>
        FluentActions.Invoking(() => new Address(street!, "City", "ST", "00000", "Country"))
            .Should().Throw<ArgumentException>().WithParameterName("street");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithInvalidCity_ShouldThrow(string? city) =>
        FluentActions.Invoking(() => new Address("Street", city!, "ST", "00000", "Country"))
            .Should().Throw<ArgumentException>().WithParameterName("city");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithInvalidState_ShouldThrow(string? state) =>
        FluentActions.Invoking(() => new Address("Street", "City", state!, "00000", "Country"))
            .Should().Throw<ArgumentException>().WithParameterName("state");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithInvalidZipCode_ShouldThrow(string? zip) =>
        FluentActions.Invoking(() => new Address("Street", "City", "ST", zip!, "Country"))
            .Should().Throw<ArgumentException>().WithParameterName("zipCode");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithInvalidCountry_ShouldThrow(string? country) =>
        FluentActions.Invoking(() => new Address("Street", "City", "ST", "00000", country!))
            .Should().Throw<ArgumentException>().WithParameterName("country");
}
