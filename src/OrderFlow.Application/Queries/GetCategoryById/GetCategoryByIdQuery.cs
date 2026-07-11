using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;