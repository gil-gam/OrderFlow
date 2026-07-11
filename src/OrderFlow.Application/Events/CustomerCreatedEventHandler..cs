using MediatR;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Events;

public sealed class CustomerCreatedEventHandler
    : INotificationHandler<UserCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CustomerCreatedEventHandler> _logger;

    public CustomerCreatedEventHandler(
        IApplicationDbContext context,
        ILogger<CustomerCreatedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var exists = await _context.Customers
            .AnyAsync(c => c.UserExternalId == notification.UserId, cancellationToken);

        if (exists)
        {
            _logger.LogInformation("Customer for UserExternalId {UserId} already exists, skipping insert.", notification.UserId);
            return;
        }

        var customer = new Customer(
            notification.UserId,
            notification.Name,
            notification.Email);

        _context.Customers.Add(customer);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            // If another concurrent process inserted the same customer, ignore unique-violation
            if (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
            {
                _logger.LogWarning(ex, "Duplicate customer detected for UserExternalId {UserId}, ignoring.", notification.UserId);
                return;
            }

            _logger.LogError(ex, "Failed to insert customer for UserExternalId {UserId}.", notification.UserId);
            throw;
        }
    }
}