using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;

namespace OrderFlow.Application.Commands.DeleteOrder;

public sealed class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteOrderCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken ct)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order is null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}