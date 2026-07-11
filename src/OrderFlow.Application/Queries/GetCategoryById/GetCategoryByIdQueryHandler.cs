using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (category is null) return null;

        return new CategoryDto(category.Id, category.Name, category.Description, category.IsActive, category.CreatedAt);
    }
}
