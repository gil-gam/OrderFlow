using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;

namespace OrderFlow.Application.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (category is null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
