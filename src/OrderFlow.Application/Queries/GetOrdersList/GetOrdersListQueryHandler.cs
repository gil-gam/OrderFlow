using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetOrdersList;

public sealed class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, PaginatedList<OrderListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrdersListQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<OrderListDto>> Handle(GetOrdersListQuery request, CancellationToken ct)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .AsQueryable();

        if (request.CustomerId.HasValue)
            query = query.Where(o => o.CustomerId == request.CustomerId.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OrderListDto(
                o.Id,
                o.CustomerId,
                string.Empty, // CustomerName from join or view
                o.Status.ToString(),
                o.TotalAmount.Amount,
                o.TotalAmount.Currency,
                o.Items.Count,
                o.OrderDate))
            .ToListAsync(ct);

        return new PaginatedList<OrderListDto>(items, request.PageIndex, request.PageSize, totalCount);
    }
}