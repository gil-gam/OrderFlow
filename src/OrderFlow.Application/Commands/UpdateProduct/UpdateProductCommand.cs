using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal UnitPrice,
    string Currency,
    Guid CategoryId
) : IRequest<ProductDto?>;