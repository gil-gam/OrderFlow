using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.DTOs;
using Xunit;

namespace OrderFlow.IntegrationTests;

[Collection("Integration Tests")]
public sealed class CategoriesControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public CategoriesControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _resetDatabase = async () =>
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Data.OrderFlowDbContext>();
            context.Categories.RemoveRange(context.Categories);
            await context.SaveChangesAsync();
        };
    }

    public Task InitializeAsync() => _resetDatabase();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_ShouldReturn201()
    {
        var request = new CreateCategoryRequestDto("Eletrônicos", "Produtos eletrônicos em geral");
        var response = await _client.PostAsJsonAsync("/api/categories", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
        category!.Name.Should().Be("Eletrônicos");
        category.Description.Should().Be("Produtos eletrônicos em geral");
    }

    [Fact]
    public async Task GetById_ShouldReturn200()
    {
        var create = await _client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequestDto("Roupas", null));
        var created = await create.Content.ReadFromJsonAsync<CategoryDto>();
        var response = await _client.GetAsync($"/api/categories/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
        category!.Name.Should().Be("Roupas");
    }

    [Fact]
    public async Task GetList_ShouldReturnAll()
    {
        await _client.PostAsJsonAsync("/api/categories", new CreateCategoryRequestDto("Cat A", null));
        await _client.PostAsJsonAsync("/api/categories", new CreateCategoryRequestDto("Cat B", null));
        var response = await _client.GetAsync("/api/categories");
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        categories.Should().HaveCount(2);
    }

    [Fact]
    public async Task Update_ShouldReturn200()
    {
        var create = await _client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequestDto("Old", "Old description"));
        var created = await create.Content.ReadFromJsonAsync<CategoryDto>();
        var response = await _client.PutAsJsonAsync($"/api/categories/{created!.Id}",
            new { Id = created!.Id, Name = "New", Description = "New description" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
        category!.Name.Should().Be("New");
    }

    [Fact]
    public async Task Delete_ShouldReturn204()
    {
        var create = await _client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequestDto("Temp", null));
        var created = await create.Content.ReadFromJsonAsync<CategoryDto>();
        var response = await _client.DeleteAsync($"/api/categories/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}