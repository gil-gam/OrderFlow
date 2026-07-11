using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid Id,
    string Name,
    string Email,
    string? Phone
) : IRequest<CustomerDto?>;
