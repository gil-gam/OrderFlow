using FluentAssertions;
using OrderFlow.Domain.ValueObjects;
using System.Globalization;
using Xunit;

namespace OrderFlow.UnitTests.Domain;

public sealed class MoneyTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreate() =>
        FluentActions.Invoking(() => new Money(100, "USD"))
            .Should().NotThrow();

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrow() =>
        FluentActions.Invoking(() => new Money(-1, "USD"))
            .Should().Throw<ArgumentException>().WithParameterName("amount");

    [Fact]
    public void Constructor_WithNullCurrency_ShouldThrow() =>
        FluentActions.Invoking(() => new Money(100, null!))
            .Should().Throw<ArgumentException>().WithParameterName("currency");

    [Fact]
    public void Constructor_WithEmptyCurrency_ShouldThrow() =>
        FluentActions.Invoking(() => new Money(100, ""))
            .Should().Throw<ArgumentException>().WithParameterName("currency");

    [Fact]
    public void Constructor_ShouldConvertCurrencyToUpper()
    {
        var money = new Money(50, "usd");
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var culture = new CultureInfo("pt-BR");
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        var money = new Money(150.50m, "BRL");
        var result = money.ToString();

        Assert.Equal("150,50 BRL", result);
    }
}