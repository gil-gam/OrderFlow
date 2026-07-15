using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.DTOs;
using Xunit;

namespace OrderFlow.IntegrationTests;

[Collection("Integration Tests")]
public sealed class CustomersControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public CustomersControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _resetDatabase = async () =>
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Data.OrderFlowDbContext>();
            context.Customers.RemoveRange(context.Customers);
            await context.SaveChangesAsync();
        };
    }

    public Task InitializeAsync() => _resetDatabase();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_ShouldReturn201_WithCustomer()
    {
        var request = new CreateCustomerRequestDto("John Doe", "john@email.com", null);
        var response = await _client.PostAsJsonAsync("/api/1.0/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Name.Should().Be("John Doe");
        customer.Email.Should().Be("john@email.com");
    }

    [Fact]
    public async Task GetById_ExistingCustomer_ShouldReturn200()
    {
        var create = await _client.PostAsJsonAsync("/api/1.0/customers",
            new CreateCustomerRequestDto("Jane Doe", "jane@email.com", null));
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await create.Content.ReadFromJsonAsync<CustomerDto>();
        var response = await _client.GetAsync($"/api/1.0/customers/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer!.Name.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task GetById_NonExistent_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/api/1.0/customers/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetList_ShouldReturnAllCustomers()
    {
        TestAuthHandler.CurrentUserId = Guid.NewGuid().ToString();
        var post1 = await _client.PostAsJsonAsync("/api/1.0/customers", new CreateCustomerRequestDto("A", "a@email.com", null));
        post1.StatusCode.Should().Be(HttpStatusCode.Created);

        TestAuthHandler.CurrentUserId = Guid.NewGuid().ToString();
        var post2 = await _client.PostAsJsonAsync("/api/1.0/customers", new CreateCustomerRequestDto("B", "b@email.com", null));
        post2.StatusCode.Should().Be(HttpStatusCode.Created);

        TestAuthHandler.CurrentUserId = Guid.NewGuid().ToString();
        var response = await _client.GetAsync("/api/1.0/customers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
        customers.Should().HaveCount(2);
    }

    [Fact]
    public async Task Update_ShouldReturn200_WithUpdatedCustomer()
    {
        var create = await _client.PostAsJsonAsync("/api/1.0/customers",
            new CreateCustomerRequestDto("Old Name", "old@email.com", null));
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await create.Content.ReadFromJsonAsync<CustomerDto>();
        var response = await _client.PutAsJsonAsync($"/api/1.0/customers/{created!.Id}",
            new { Id = created!.Id, Name = "New Name", Email = "new@email.com", Phone = (string?)null });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer!.Name.Should().Be("New Name");
        customer.Email.Should().Be("new@email.com");
    }

    [Fact]
    public async Task Delete_ShouldReturn204()
    {
        var create = await _client.PostAsJsonAsync("/api/1.0/customers",
            new CreateCustomerRequestDto("Delete Me", "delete@email.com", null));
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await create.Content.ReadFromJsonAsync<CustomerDto>();
        var response = await _client.DeleteAsync($"/api/1.0/customers/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await _client.GetAsync($"/api/1.0/customers/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}