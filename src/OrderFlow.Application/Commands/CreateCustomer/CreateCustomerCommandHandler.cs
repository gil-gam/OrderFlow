using MediatR;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCustomerCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var customer = new Customer(_currentUser.UserId, request.Name, request.Email);

        if (!string.IsNullOrWhiteSpace(request.Phone))
            customer.Update(request.Name, request.Email, request.Phone);

        _context.Customers.Add(customer);
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