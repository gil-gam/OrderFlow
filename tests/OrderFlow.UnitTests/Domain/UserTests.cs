using FluentAssertions;
using OrderFlow.Domain.Entities;
using Xunit;

namespace OrderFlow.UnitTests.Domain;

public sealed class UserTests
{
    [Fact]
    public void Constructor_ShouldCreateUser()
    {
        var user = new User("John", "john@email.com", "hash123");

        user.Id.Should().NotBe(Guid.Empty);
        user.Name.Should().Be("John");
        user.Email.Should().Be("john@email.com");
        user.PasswordHash.Should().Be("hash123");
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var user = new User("John", "john@email.com", "hash");
        user.Deactivate();
        user.IsActive.Should().BeFalse();
    }
}
