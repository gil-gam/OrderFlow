using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OrderFlow.IntegrationTests.Fixtures;

namespace OrderFlow.IntegrationTests.Controllers;

public sealed class OrdersControllerTests : TestBase
{
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly Guid ProductId = Guid.NewGuid();

    public OrdersControllerTests(OrderFlowWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateOrder_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = CreateValidOrderRequest();

        // Act
        var response = await Client.PostAsJsonAsync("/api/orders", request, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOrder_WithValidAuth_ShouldReturnCreated()
    {
        // Arrange
        var authClient = await GetAuthenticatedClientAsync();
        var request = CreateValidOrderRequest();

        // Act
        var response = await authClient.PostAsJsonAsync("/api/orders", request, JsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var orderId = await response.Content.ReadFromJsonAsync<Guid>(JsonOptions);
        orderId.Should().NotBeEmpty();

        // Store for the GetById test
        _createdOrderId = orderId;
    }

    [Fact]
    public async Task GetOrderById_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync($"/api/orders/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOrderById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var authClient = await GetAuthenticatedClientAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await authClient.GetAsync($"/api/orders/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAndGetOrder_ShouldReturnSameOrder()
    {
        // Arrange — create an order first
        var authClient = await GetAuthenticatedClientAsync();
        var createRequest = CreateValidOrderRequest();

        var createResponse = await authClient.PostAsJsonAsync(
            "/api/orders", createRequest, JsonOptions);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var orderId = await createResponse.Content.ReadFromJsonAsync<Guid>(JsonOptions);

        // Act — retrieve the created order
        var getResponse = await authClient.GetAsync($"/api/orders/{orderId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await getResponse.Content
            .ReadFromJsonAsync<Dictionary<string, object>>(JsonOptions);

        order.Should().NotBeNull();
        order!["customerId"]?.ToString().Should().Be(CustomerId.ToString());
    }

    // ── Shared State ───────────────────────────────────────
    private static Guid _createdOrderId;

    private static object CreateValidOrderRequest() => new
    {
        customerId = CustomerId,
        street = "123 Main St",
        city = "New York",
        state = "NY",
        zipCode = "10001",
        country = "USA",
        items = new[]
        {
            new
            {
                productId = ProductId,
                productName = "Integration Test Product",
                quantity = 1,
                unitPrice = 29.99m,
                currency = "USD"
            }
        }
    };
}