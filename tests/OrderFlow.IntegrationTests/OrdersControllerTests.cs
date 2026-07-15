using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Commands.CreateOrder;
using OrderFlow.Application.DTOs;
using Xunit;

namespace OrderFlow.IntegrationTests;

[Collection("Integration Tests")]
public sealed class OrdersControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;
    private Guid _customerId;

    public OrdersControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _resetDatabase = async () =>
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Data.OrderFlowDbContext>();
            context.Orders.RemoveRange(context.Orders);
            context.Customers.RemoveRange(context.Customers);
            await context.SaveChangesAsync();
        };
    }

    public async Task InitializeAsync()
    {
        TestAuthHandler.CurrentUserId = "orders-test-user";
        await _resetDatabase();

        var customerResponse = await _client.PostAsJsonAsync("/api/1.0/customers",
            new CreateCustomerRequestDto("Order Tester", "orders@email.com", null));
        customerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();
        _customerId = customer!.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_ShouldReturn201()
    {
        var command = new CreateOrderCommand(
            Street: "Rua A", City: "São Paulo",
            State: "SP", ZipCode: "01001", Country: "Brasil",
            Items: new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "Item 1", 2, 50m, "BRL")
            });

        var response = await _client.PostAsJsonAsync("/api/1.0/orders", command);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetList_ShouldReturnPagedResult()
    {
        var command = new CreateOrderCommand(
            Street: "Rua B", City: "Rio", State: "RJ",
            ZipCode: "20000", Country: "Brasil",
            Items: new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "Item", 1, 100m, "BRL")
            });
        var postResponse = await _client.PostAsJsonAsync("/api/1.0/orders", command);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await _client.GetAsync("/api/1.0/orders?pageIndex=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_NonExistent_ShouldReturn404()
    {
        var response = await _client.DeleteAsync($"/api/1.0/orders/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}