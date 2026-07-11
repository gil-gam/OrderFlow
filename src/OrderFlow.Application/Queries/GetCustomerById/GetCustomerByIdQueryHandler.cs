using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Queries.GetCustomerById;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (customer is null) return null;

        return new CustomerDto(
            customer.Id,
            customer.UserExternalId,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.CreatedAt);
    }
}
