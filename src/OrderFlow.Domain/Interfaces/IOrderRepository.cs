using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByCustomerAsync(
        Guid customerId, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    void Update(Order order);
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
}