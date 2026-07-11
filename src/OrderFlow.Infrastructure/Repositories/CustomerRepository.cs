using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly OrderFlowDbContext _context;

    public CustomerRepository(OrderFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Customer?> GetByUserExternalIdAsync(Guid userExternalId, CancellationToken ct)
        => await _context.Customers.FirstOrDefaultAsync(c => c.UserExternalId == userExternalId, ct);

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct)
        => await _context.Customers.ToListAsync(ct);

    public async Task AddAsync(Customer customer, CancellationToken ct)
        => await _context.Customers.AddAsync(customer, ct);

    public void Update(Customer customer)
        => _context.Customers.Update(customer);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
}