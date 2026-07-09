using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using OrderFlow.Application.DTOs.Auth;

namespace OrderFlow.IntegrationTests.Fixtures;

/// <summary>
/// Base class for integration tests providing shared HttpClient,
/// JSON options, and helper methods.
/// </summary>
public abstract class TestBase : IClassFixture<OrderFlowWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly OrderFlowWebApplicationFactory Factory;
    private string? _authToken;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected TestBase(OrderFlowWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Authenticates with mock credentials and returns the JWT token.
    /// </summary>
    protected async Task<string> GetAuthTokenAsync()
    {
        if (_authToken is not null)
            return _authToken;

        var response = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@orderflow.com",
            password = "OrderFlow@2026"
        }, JsonOptions);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<LoginResponseDto>(JsonOptions);

        _authToken = result!.Token;
        return _authToken;
    }

    /// <summary>
    /// Creates an HttpClient with the Authorization header set.
    /// </summary>
    protected async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var token = await GetAuthTokenAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}