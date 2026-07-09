using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OrderFlow.Application.DTOs.Auth;
using OrderFlow.IntegrationTests.Fixtures;

namespace OrderFlow.IntegrationTests.Controllers;

public sealed class AuthControllerTests : TestBase
{
    public AuthControllerTests(OrderFlowWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = new
        {
            email = "admin@orderflow.com",
            password = "OrderFlow@2026"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<LoginResponseDto>(JsonOptions);

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.Email.Should().Be("admin@orderflow.com");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new
        {
            email = "wrong@email.com",
            password = "wrong_password"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            email = "newuser@test.com",
            password = "Password@123",
            name = "New User"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content
            .ReadFromJsonAsync<RegisterUserResponseDto>(JsonOptions);

        result.Should().NotBeNull();
        result!.Email.Should().Be("newuser@test.com");
        result.Name.Should().Be("New User");
        result.UserId.Should().NotBeEmpty();
    }
}