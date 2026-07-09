using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OrderFlow.IntegrationTests.Fixtures;

namespace OrderFlow.IntegrationTests.Controllers;

public sealed class HealthControllerTests : TestBase
{
    public HealthControllerTests(OrderFlowWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetHealth_ShouldReturnOk()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content
            .ReadFromJsonAsync<Dictionary<string, object>>(JsonOptions);

        content.Should().NotBeNull();
        content!["status"].ToString().Should().Be("Healthy");
    }
}