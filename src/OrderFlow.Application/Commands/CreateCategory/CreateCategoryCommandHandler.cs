using MediatR;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var category = Category.Create(request.Name, request.Description);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(ct);

        return new CategoryDto(category.Id, category.Name, category.Description, category.IsActive, category.CreatedAt);
    }
}