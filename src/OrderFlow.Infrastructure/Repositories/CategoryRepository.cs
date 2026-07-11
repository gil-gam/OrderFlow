using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly OrderFlowDbContext _context;

    public CategoryRepository(OrderFlowDbContext context) => _context = context;

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
        => await _context.Categories.OrderBy(c => c.Name).ToListAsync(ct);

    public async Task<IEnumerable<Category>> GetActiveAsync(CancellationToken ct = default)
        => await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(ct);

    public async Task AddAsync(Category category, CancellationToken ct = default)
        => await _context.Categories.AddAsync(category, ct);

    public void Update(Category category) => _context.Categories.Update(category);

    public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct) > 0;
}