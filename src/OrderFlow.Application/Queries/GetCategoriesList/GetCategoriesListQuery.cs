using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetCategoriesList;

public sealed record GetCategoriesListQuery(bool? ActiveOnly = null) : IRequest<List<CategoryDto>>;