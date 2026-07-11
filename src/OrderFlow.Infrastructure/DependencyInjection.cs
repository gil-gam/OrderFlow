using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.Common.Interfaces.Auth;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Repositories;
using OrderFlow.Infrastructure.Services;

namespace OrderFlow.Infrastructure;

/// <summary>
/// Registers infrastructure-layer services: DbContext, repositories, and token service.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<OrderFlowDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<OrderFlowDbContext>());

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}