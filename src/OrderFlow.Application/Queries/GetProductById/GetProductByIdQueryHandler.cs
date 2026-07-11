using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
        if (product is null) return null;

        return new ProductDto(
            product.Id, product.Name, product.Description,
            product.UnitPrice.Amount, product.UnitPrice.Currency,
            product.CategoryId, product.IsActive, product.CreatedAt);
    }
}
