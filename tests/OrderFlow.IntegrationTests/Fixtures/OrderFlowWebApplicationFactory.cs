using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.IntegrationTests.Fixtures;

public class OrderFlowWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "ThisIsAReallyLongTestSecretKeyForJwtSigning1234567890",
                ["Jwt:Issuer"] = "OrderFlow.IntegrationTests",
                ["Jwt:Audience"] = "OrderFlow.IntegrationTests"
            });
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderFlowDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderFlowDbContext>();
        await db.Database.EnsureDeletedAsync();
    }
}