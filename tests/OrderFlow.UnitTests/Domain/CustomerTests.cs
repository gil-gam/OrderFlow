using FluentAssertions;
using OrderFlow.Domain.Entities;
using Xunit;

namespace OrderFlow.UnitTests.Domain;

public sealed class CustomerTests
{
    [Fact]
    public void Constructor_ShouldCreateCustomer()
    {
        var userExternalId = Guid.NewGuid();
        var customer = new Customer(userExternalId, "John Doe", "john@email.com");

        customer.Id.Should().NotBe(Guid.Empty);
        customer.UserExternalId.Should().Be(userExternalId);
        customer.Name.Should().Be("John Doe");
        customer.Email.Should().Be("john@email.com");
        customer.Phone.Should().BeEmpty();
    }

    [Fact]
    public void Update_ShouldChangeProperties()
    {
        var customer = new Customer(Guid.NewGuid(), "Old", "old@email.com");

        customer.Update("New", "new@email.com", "123456789");

        customer.Name.Should().Be("New");
        customer.Email.Should().Be("new@email.com");
        customer.Phone.Should().Be("123456789");
    }
}
