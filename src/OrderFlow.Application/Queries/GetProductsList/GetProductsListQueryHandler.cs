using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetProductsList;

public sealed class GetProductsListQueryHandler : IRequestHandler<GetProductsListQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsListQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ProductDto>> Handle(GetProductsListQuery request, CancellationToken ct)
    {
        var query = _context.Products.AsQueryable();

        if (request.ActiveOnly == true)
            query = query.Where(p => p.IsActive);

        if (request.CategoryId.HasValue && request.CategoryId != Guid.Empty)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        return await query
            .Select(p => new ProductDto(
                p.Id, p.Name, p.Description,
                p.UnitPrice.Amount, p.UnitPrice.Currency,
                p.CategoryId, p.IsActive, p.CreatedAt))
            .ToListAsync(ct);
    }
}