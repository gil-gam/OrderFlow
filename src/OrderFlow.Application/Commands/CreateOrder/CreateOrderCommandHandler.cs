using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateOrderCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        // ── Finds Customer by UserId from JWT token ──
        var customer = await _context.Customers
            .FirstOrDefaultAsync(
                c => c.UserExternalId == _currentUser.UserId,
                cancellationToken)
            ?? throw new UnauthorizedAccessException("Customer profile not found.");

        var shippingAddress = new Address(
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country);

        var order = Order.Create(customer.Id, shippingAddress);

        foreach (var item in request.Items)
        {
            order.AddItem(
                item.ProductId,
                item.ProductName,
                item.Quantity,
                new Money(item.UnitPrice, item.Currency ?? "USD"));
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}