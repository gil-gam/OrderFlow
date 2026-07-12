using MediatR;
using OrderFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Events;

public sealed class CustomerCreatedEventHandler
    : INotificationHandler<UserCreatedEvent>
{
    private readonly IApplicationDbContext _context;

    public CustomerCreatedEventHandler(IApplicationDbContext context)
        => _context = context;

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var exists = await _context.Customers
            .AnyAsync(c => c.UserExternalId == notification.UserId, cancellationToken);

        if (exists) return;

        var customer = new Customer(
            notification.UserId,
            notification.Name,
            notification.Email);

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }
}