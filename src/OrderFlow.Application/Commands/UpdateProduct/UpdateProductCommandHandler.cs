using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto?>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
        if (product is null) return null;

        product.Update(
            request.Name,
            request.Description,
            new Money(request.UnitPrice, request.Currency),
            request.CategoryId);

        await _context.SaveChangesAsync(ct);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.UnitPrice.Amount,
            product.UnitPrice.Currency,
            product.CategoryId,
            product.IsActive,
            product.CreatedAt);
    }
}