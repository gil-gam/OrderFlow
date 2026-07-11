using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Application.DTOs.Auth;
using OrderFlow.Application.Events;
using OrderFlow.Domain.Entities;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace OrderFlow.Application.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, RegisterUserResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPublisher _publisher;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IPublisher publisher,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<RegisterUserResponseDto> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser is not null)
            throw new InvalidOperationException("Email already registered.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User(request.Name, request.Email, passwordHash);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(
            new UserCreatedEvent(user.Id, user.Name, user.Email),
            cancellationToken);

        return new RegisterUserResponseDto(
            user.Id.ToString(),
            user.Email,
            user.Name);
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