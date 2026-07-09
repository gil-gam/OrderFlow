using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(OrderFlowDbContext context)
    {
        if (await context.Orders.AnyAsync())
            return;

        var address = new Address(
            "123 Main St", "New York", "NY", "10001", "USA");

        var order = Order.Create(Guid.NewGuid(), address);
        order.AddItem(Guid.NewGuid(), "Product A", 2, new Money(49.99m, "USD"));
        order.AddItem(Guid.NewGuid(), "Product B", 1, new Money(129.99m, "USD"));

        context.Orders.Add(order);
        await context.SaveChangesAsync();
    }
}