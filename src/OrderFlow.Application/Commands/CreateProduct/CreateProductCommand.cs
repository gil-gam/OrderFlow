using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal UnitPrice,
    string Currency,
    Guid CategoryId
) : IRequest<ProductDto>;