using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Data;

public sealed class OrderFlowDbContext : DbContext, IApplicationDbContext
{
    private readonly IPublisher _publisher;

    public OrderFlowDbContext(
        DbContextOptions<OrderFlowDbContext> options,
        IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OrderFlowDbContext).Assembly);

        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Customer>().ToTable("Customer");
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedNever();
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).IsRequired().HasMaxLength(2000);
            entity.Property(p => p.IsActive).HasDefaultValue(true);
            entity.Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
            entity.Property(p => p.CategoryId).IsRequired();

            entity.HasOne<Product>()
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.OwnsOne(p => p.UnitPrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("UnitPrice_Amount");
                money.Property(m => m.Currency)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("UnitPrice_Currency");
            });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Property(c => c.IsActive).HasDefaultValue(true);
            entity.Property(c => c.CreatedAt).HasColumnType("timestamp with time zone");
        });

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEntities = ChangeTracker.Entries<Order>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        domainEntities.ForEach(e => e.ClearDomainEvents());

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var @event in domainEvents)
            await _publisher.Publish(@event, cancellationToken);

        return result;
    }
}