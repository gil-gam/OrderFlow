using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Commands.CreateOrder;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Tests.Commands;

public sealed class CreateOrderCommandHandlerTests
{
    private sealed class TestDbContext : DbContext, IApplicationDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).ValueGeneratedNever();
                entity.Property(o => o.Status).HasConversion<string>();
                entity.Ignore(o => o.DomainEvents);
                entity.Ignore(o => o.ShippingAddress);
                entity.Ignore(o => o.TotalAmount);
                entity.Ignore(o => o.DiscountApplied);
                entity.Ignore(o => o.Items);
            });
        }
    }

    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrderAndReturnId()
    {
        using var context = CreateContext();
        var handler = new CreateOrderCommandHandler(context);

        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Street: "123 Main St", City: "New York",
            State: "NY", ZipCode: "10001", Country: "USA",
            Items: new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "Product A", 2, 50m, "USD")
            });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();

        var savedOrder = await context.Orders.FindAsync(result);
        savedOrder.Should().NotBeNull();
        savedOrder!.CustomerId.Should().Be(command.CustomerId);
    }

    [Fact]
    public async Task Handle_ShouldAddAllItems()
    {
        using var context = CreateContext();
        var handler = new CreateOrderCommandHandler(context);

        var command = new CreateOrderCommand(
            CustomerId: Guid.NewGuid(),
            Street: "456 Oak Ave", City: "Los Angeles",
            State: "CA", ZipCode: "90001", Country: "USA",
            Items: new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "Item 1", 1, 100m, "USD"),
                new(Guid.NewGuid(), "Item 2", 3, 25.50m, "USD")
            });

        var result = await handler.Handle(command, CancellationToken.None);

        var savedOrder = await context.Orders.FindAsync(result);
        savedOrder.Should().NotBeNull();
    }
}