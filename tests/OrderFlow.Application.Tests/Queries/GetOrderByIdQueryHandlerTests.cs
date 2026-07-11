using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.Queries.GetOrderById;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Tests.Queries;

public sealed class GetOrderByIdQueryHandlerTests
{
    private sealed class TestDbContext : DbContext, IApplicationDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

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
                entity.Ignore(o => o.CustomerId);
            });

            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedNever();
                entity.Property(p => p.CategoryId).IsRequired();
                entity.OwnsOne(p => p.UnitPrice);
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

    private static Order CreateTestOrder()
    {
        var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
        var order = Order.Create(Guid.NewGuid(), address);
        order.AddItem(Guid.NewGuid(), "Product X", 2, new Money(75m, "USD"));
        return order;
    }

    [Fact]
    public async Task Handle_ExistingOrder_ShouldReturnMappedDto()
    {
        using var context = CreateContext();
        var order = CreateTestOrder();
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var handler = new GetOrderByIdQueryHandler(context);
        var result = await handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.CustomerId.Should().Be(order.CustomerId);
        result.Status.Should().Be(OrderStatus.Pending.ToString());
        result.TotalAmount.Should().Be(150m);
        result.Currency.Should().Be("USD");
        result.Items.Should().HaveCount(1);
        result.Items.First().ProductName.Should().Be("Product X");
        result.Items.First().Quantity.Should().Be(2);
        result.Items.First().Subtotal.Should().Be(150m);
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ShouldReturnNull()
    {
        using var context = CreateContext();
        var handler = new GetOrderByIdQueryHandler(context);
        var result = await handler.Handle(new GetOrderByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }
}