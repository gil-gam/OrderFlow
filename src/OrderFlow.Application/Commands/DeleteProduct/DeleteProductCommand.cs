using MediatR;

namespace OrderFlow.Application.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<bool>;
