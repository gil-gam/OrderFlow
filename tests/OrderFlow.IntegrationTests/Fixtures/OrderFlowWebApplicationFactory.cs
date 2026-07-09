using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.IntegrationTests.Fixtures;

/// <summary>
/// Custom WebApplicationFactory that replaces the real database
/// with an in-memory database for integration testing.
/// </summary>
public sealed class OrderFlowWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string DatabaseName = "OrderFlow_IntegrationTests_Db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove the real DbContext registration
            services.RemoveAll<DbContextOptions<OrderFlowDbContext>>();

            // Replace with in-memory database
            services.AddDbContext<OrderFlowDbContext>(options =>
                options.UseInMemoryDatabase(DatabaseName));
        });
    }

    public async Task InitializeAsync()
    {
        // Ensure the in-memory database is created
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderFlowDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderFlowDbContext>();
        await db.Database.EnsureDeletedAsync();
    }
}