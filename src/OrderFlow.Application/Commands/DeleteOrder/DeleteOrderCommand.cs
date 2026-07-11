using MediatR;

namespace OrderFlow.Application.Commands.DeleteOrder;

public sealed record DeleteOrderCommand(Guid OrderId) : IRequest<bool>;