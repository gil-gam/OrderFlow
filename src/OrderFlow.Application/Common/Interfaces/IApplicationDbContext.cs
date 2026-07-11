using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Order> Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}