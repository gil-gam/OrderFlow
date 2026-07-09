using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDetailDto?>;