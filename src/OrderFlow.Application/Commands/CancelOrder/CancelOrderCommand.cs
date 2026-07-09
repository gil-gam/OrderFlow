using MediatR;

namespace OrderFlow.Application.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId) : IRequest;