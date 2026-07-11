using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct);

        Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken ct);

        Task AddAsync(Product product, CancellationToken ct);

        void Update(Product order);

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}