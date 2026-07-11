using MediatR;

namespace OrderFlow.Application.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest<bool>;