using MediatR;
using OrderFlow.Application.Common.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateOrderCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<Guid> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var shippingAddress = new Address(
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country);

        var order = Order.Create(request.CustomerId, shippingAddress);

        decimal subtotal = 0;

        foreach (var item in request.Items)
        {
            order.AddItem(
                item.ProductId,
                item.ProductName,
                item.Quantity,
                new Money(item.UnitPrice, item.Currency ?? "USD"));

            subtotal += item.UnitPrice * item.Quantity;
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}