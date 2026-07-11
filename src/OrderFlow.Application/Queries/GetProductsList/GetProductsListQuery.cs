using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetProductsList;

public sealed record GetProductsListQuery(Guid? CategoryId = null, bool? ActiveOnly = null) : IRequest<List<ProductDto>>;
