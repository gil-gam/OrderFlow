using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetCustomersList;

public sealed record GetCustomersListQuery : IRequest<List<CustomerDto>>;