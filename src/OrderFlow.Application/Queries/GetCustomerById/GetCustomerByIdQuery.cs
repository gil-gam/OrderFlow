using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto?>;