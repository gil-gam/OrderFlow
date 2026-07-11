using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description
) : IRequest<CategoryDto>;