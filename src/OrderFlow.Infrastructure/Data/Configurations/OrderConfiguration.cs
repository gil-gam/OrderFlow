using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Infrastructure.Data.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.CustomerId).IsRequired();
        builder.Property(o => o.OrderDate).IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Configure ShippingAddress owned entity
        builder.OwnsOne(typeof(Address), "ShippingAddress", b =>
        {
            b.Property("Street").HasColumnName("Address_Street").IsRequired().HasMaxLength(200);
            b.Property("City").HasColumnName("Address_City").IsRequired().HasMaxLength(100);
            b.Property("State").HasColumnName("Address_State").IsRequired().HasMaxLength(50);
            b.Property("ZipCode").HasColumnName("Address_ZipCode").IsRequired().HasMaxLength(20);
            b.Property("Country").HasColumnName("Address_Country").IsRequired().HasMaxLength(50);
        });

        // Configure TotalAmount owned entity
        builder.OwnsOne(typeof(Money), "TotalAmount", b =>
        {
            b.Property("Amount").HasColumnName("Total_Amount").HasPrecision(18, 2);
            b.Property("Currency").HasColumnName("Total_Currency").HasMaxLength(3);
        });

        // Configure DiscountApplied owned entity
        builder.OwnsOne(typeof(Money), "DiscountApplied", b =>
        {
            b.Property("Amount").HasColumnName("Discount_Amount").HasPrecision(18, 2);
            b.Property("Currency").HasColumnName("Discount_Currency").HasMaxLength(3);
        });

        // Configure Items collection
        builder.OwnsMany(typeof(OrderItem), "Items", b =>
        {
            b.ToTable("OrderItems");
            b.HasKey("Id");
            b.Property("Id").ValueGeneratedOnAdd();
            b.WithOwner().HasForeignKey("OrderId");

            b.Property("ProductId").IsRequired();
            b.Property("ProductName").IsRequired().HasMaxLength(200);
            b.Property("Quantity").IsRequired();

            // Configure UnitPrice within OrderItem
            b.OwnsOne(typeof(Money), "UnitPrice", ub =>
            {
                ub.Property("Amount").HasColumnName("UnitPrice_Amount").HasPrecision(18, 2);
                ub.Property("Currency").HasColumnName("UnitPrice_Currency").HasMaxLength(3);
            });
        });

        builder.Navigation("Items").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(o => o.DomainEvents);

        builder.HasIndex(o => o.CustomerId);
    }
}