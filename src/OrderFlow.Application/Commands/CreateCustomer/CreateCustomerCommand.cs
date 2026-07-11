using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name,
    string Email,
    string? Phone
) : IRequest<CustomerDto>;