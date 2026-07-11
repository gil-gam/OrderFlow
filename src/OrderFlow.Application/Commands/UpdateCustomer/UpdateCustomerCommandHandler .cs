using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto?>
{
    private readonly IApplicationDbContext _context;

    public UpdateCustomerCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<CustomerDto?> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (customer is null) return null;

        customer.Update(request.Name, request.Email, request.Phone ?? string.Empty);
        await _context.SaveChangesAsync(ct);

        return new CustomerDto(
            customer.Id,
            customer.UserExternalId,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.CreatedAt);
    }
}