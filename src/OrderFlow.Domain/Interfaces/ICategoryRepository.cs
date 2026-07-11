using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Category>> GetActiveAsync(CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    void Update(Category category);
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
}
