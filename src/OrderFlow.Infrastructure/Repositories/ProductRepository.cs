using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly OrderFlowDbContext _context;

    public ProductRepository(OrderFlowDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default)
    => await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync(ct);

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken ct)
        => await _context.Products.Where(p => p.IsActive).ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct)
        => await _context.Products.AddAsync(product, ct);

    public void Update(Product product) => _context.Products.Update(product);

    public async Task<int> SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
}