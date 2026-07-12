using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Commands.UpdateOrder;

public sealed class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateOrderCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken ct)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null) return false;

        var requestedProductIds = request.Items.Select(i => i.ProductId).ToHashSet();

        // Remove items NOT in the request (DELETE only — no re-add for these)
        foreach (var item in order.Items.ToList())
        {
            if (!requestedProductIds.Contains(item.ProductId))
                order.RemoveItem(item.ProductId);
        }

        // Update existing items in-place or add new ones (UPDATE/INSERT — no remove+recreate)
        foreach (var itemDto in request.Items)
        {
            order.UpdateItem(
                itemDto.ProductId,
                itemDto.ProductName,
                itemDto.Quantity,
                new Money(itemDto.UnitPrice, itemDto.Currency));
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}