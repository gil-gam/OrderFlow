using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetCategoriesList;

public sealed class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesListQuery, List<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesListQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CategoryDto>> Handle(GetCategoriesListQuery request, CancellationToken ct)
    {
        var query = _context.Categories.AsQueryable();

        if (request.ActiveOnly == true)
            query = query.Where(c => c.IsActive);

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IsActive, c.CreatedAt))
            .ToListAsync(ct);
    }
}