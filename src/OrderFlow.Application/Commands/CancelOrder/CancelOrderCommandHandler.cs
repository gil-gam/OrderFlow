using MediatR;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _repository;

    public CancelOrderCommandHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        order.Cancel();
        _repository.Update(order);
        await _repository.SaveChangesAsync(ct);
    }
}