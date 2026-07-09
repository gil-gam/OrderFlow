using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetOrderByIdQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<OrderDetailDto?> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null) return null;

        return new OrderDetailDto(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            order.Items.Select(i => new OrderItemDetailDto(
                i.ProductId, i.ProductName, i.Quantity, i.Subtotal().Amount
            )).ToList());
    }
}