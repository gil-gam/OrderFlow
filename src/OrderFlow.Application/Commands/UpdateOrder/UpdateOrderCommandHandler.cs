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

        // Get current item IDs
        var currentProductIds = order.Items.Select(i => i.ProductId).ToList();
        var requestedProductIds = request.Items.Select(i => i.ProductId).ToList();

        // Remove items not in the new list
        foreach (var productId in currentProductIds)
        {
            if (!requestedProductIds.Contains(productId))
                order.RemoveItem(productId);
        }

        // Add or update items
        foreach (var itemDto in request.Items)
        {
            if (currentProductIds.Contains(itemDto.ProductId))
            {
                // Remove and re-add to update quantity/price
                order.RemoveItem(itemDto.ProductId);
            }

            order.AddItem(
                itemDto.ProductId,
                itemDto.ProductName,
                itemDto.Quantity,
                new Money(itemDto.UnitPrice, itemDto.Currency));
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

