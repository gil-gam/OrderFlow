using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;

namespace OrderFlow.Application.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCustomerCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (customer is null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}