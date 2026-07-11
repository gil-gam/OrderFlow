using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;

namespace OrderFlow.Application.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
        if (product is null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}