using MediatR;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Commands.CreateProduct;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            new Money(request.UnitPrice, request.Currency),
            request.CategoryId);

        _context.Products.Add(product);
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