using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetOrdersList;

public sealed record GetOrdersListQuery(
    int PageIndex = 1,
    int PageSize = 10,
    Guid? CustomerId = null
) : IRequest<PaginatedList<OrderListDto>>;
