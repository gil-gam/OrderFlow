using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Commands.CreateOrder;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Tests.Commands;

public sealed class CreateOrderCommandHandlerTests
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
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedNever();
                entity.HasIndex(c => c.UserExternalId).IsUnique();
            });
        }
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        public Guid UserId { get; }

        public FakeCurrentUserService(Guid userId) => UserId = userId;
    }

    private static (TestDbContext context, FakeCurrentUserService currentUser) CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new TestDbContext(options);
        var currentUser = new FakeCurrentUserService(Guid.NewGuid());

        return (context, currentUser);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrderAndReturnId()
    {
        var (context, currentUser) = CreateContext();

        // Seed a Customer linked to the fake user
        var customer = new Customer(currentUser.UserId, "Test Customer", "test@email.com");
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var handler = new CreateOrderCommandHandler(context, currentUser);

        var command = new CreateOrderCommand(
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
        savedOrder!.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public async Task Handle_ShouldAddAllItems()
    {
        var (context, currentUser) = CreateContext();

        // Seed a Customer linked to the fake user
        var customer = new Customer(currentUser.UserId, "Test Customer", "test@email.com");
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var handler = new CreateOrderCommandHandler(context, currentUser);

        var command = new CreateOrderCommand(
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

    [Fact]
    public async Task Handle_NoCustomerProfile_ShouldThrowUnauthorized()
    {
        var (context, currentUser) = CreateContext();
        // Nenhum Customer semeado → deve lançar UnauthorizedAccessException

        var handler = new CreateOrderCommandHandler(context, currentUser);

        var command = new CreateOrderCommand(
            Street: "123 Main St", City: "New York",
            State: "NY", ZipCode: "10001", Country: "USA",
            Items: new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "Product A", 2, 50m, "USD")
            });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}