using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Data.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customer");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        //Unique index for UserExternalId(no formal FK)
        builder.HasIndex(c => c.UserExternalId)
            .IsUnique()
            .HasDatabaseName("IX_Customer_UserExternalId");
    }
}