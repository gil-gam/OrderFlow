using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;