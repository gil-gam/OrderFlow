using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description
) : IRequest<CategoryDto?>;
