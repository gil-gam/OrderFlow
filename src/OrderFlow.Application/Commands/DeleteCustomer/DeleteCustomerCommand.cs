using MediatR;

namespace OrderFlow.Application.Commands.DeleteCustomer;

public sealed record DeleteCustomerCommand(Guid Id) : IRequest<bool>;