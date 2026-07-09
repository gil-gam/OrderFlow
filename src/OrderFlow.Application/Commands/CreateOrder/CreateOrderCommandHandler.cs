using MediatR;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;

    public CreateOrderCommandHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var address = Address.Create(
            request.Street, request.City, request.State,
            request.ZipCode, request.Country);

        var order = Order.Create(request.CustomerId, address);

        foreach (var item in request.Items)
            order.AddItem(item.ProductId, item.ProductName, item.Quantity,
                Money.Create(item.UnitPrice, item.Currency));

        await _repository.AddAsync(order, ct);
        await _repository.SaveChangesAsync(ct);

        return order.Id;
    }
}