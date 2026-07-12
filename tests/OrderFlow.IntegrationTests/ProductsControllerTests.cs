using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Commands.CreateProduct;
using OrderFlow.Application.DTOs;
using Xunit;

namespace OrderFlow.IntegrationTests;

[Collection("Integration Tests")]
public sealed class ProductsControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public ProductsControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _resetDatabase = async () =>
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Data.OrderFlowDbContext>();
            context.Products.RemoveRange(context.Products);
            await context.SaveChangesAsync();
            context.Categories.RemoveRange(context.Categories);
            await context.SaveChangesAsync();
        };
    }

    public Task InitializeAsync() => _resetDatabase();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_ShouldReturn201()
    {
        TestAuthHandler.CurrentUserId = "products-test-user";

        var catResponse = await _client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequestDto("Test Category", "For product tests"));
        catResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryDto>();
        category!.Id.Should().NotBe(Guid.Empty);

        var command = new CreateProductCommand("TV", "Smart TV 50 polegadas", 2999.99m, "BRL", category.Id);
        var response = await _client.PostAsJsonAsync("/api/products", command);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product!.Name.Should().Be("TV");
        product.CategoryId.Should().Be(category.Id);
    }

    [Fact]
    public async Task GetList_WithCategoryFilter_ShouldFilter()
    {
        TestAuthHandler.CurrentUserId = "products-test-user";

        var catResponse = await _client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequestDto("Test Category", "For product tests"));
        catResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryDto>();

        var prodResponse = await _client.PostAsJsonAsync("/api/products",
            new CreateProductCommand("Notebook", "Laptop", 5000, "BRL", category!.Id));
        prodResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await _client.GetAsync($"/api/products?categoryId={category.Id}");
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().Contain(p => p.Name == "Notebook");
    }

    [Fact]
    public async Task Update_ShouldReturn200()
    {
        TestAuthHandler.CurrentUserId = "products-test-user";

        var catResponse = await _client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequestDto("Test Category", "For product tests"));
        catResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryDto>();

        var prodResponse = await _client.PostAsJsonAsync("/api/products",
            new CreateProductCommand("Old Product", "Old desc", 100, "USD", category!.Id));
        prodResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await prodResponse.Content.ReadFromJsonAsync<ProductDto>();

        var response = await _client.PutAsJsonAsync($"/api/products/{product!.Id}",
            new { Id = product.Id, Name = "New Product", Description = "New desc", UnitPrice = 200m, Currency = "USD", CategoryId = category.Id });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturn204()
    {
        TestAuthHandler.CurrentUserId = "products-test-user";

        var catResponse = await _client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequestDto("Test Category", "For product tests"));
        catResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryDto>();

        var prodResponse = await _client.PostAsJsonAsync("/api/products",
            new CreateProductCommand("Temp", "Temp", 50, "USD", category!.Id));
        prodResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await prodResponse.Content.ReadFromJsonAsync<ProductDto>();

        var response = await _client.DeleteAsync($"/api/products/{product!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}