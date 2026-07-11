using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto?>
{
    private readonly IApplicationDbContext _context;

    public UpdateCategoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<CategoryDto?> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (category is null) return null;

        category.Update(request.Name, request.Description);
        await _context.SaveChangesAsync(ct);

        return new CategoryDto(category.Id, category.Name, category.Description, category.IsActive, category.CreatedAt);
    }
}