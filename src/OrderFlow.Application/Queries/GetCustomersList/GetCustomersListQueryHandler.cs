using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetCustomersList;

public sealed class GetCustomersListQueryHandler : IRequestHandler<GetCustomersListQuery, List<CustomerDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCustomersListQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CustomerDto>> Handle(GetCustomersListQuery request, CancellationToken ct)
    {
        return await _context.Customers
            .Select(c => new CustomerDto(c.Id, c.UserExternalId, c.Name, c.Email, c.Phone, c.CreatedAt))
            .ToListAsync(ct);
    }
}