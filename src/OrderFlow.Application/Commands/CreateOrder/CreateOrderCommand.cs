using MediatR;

namespace OrderFlow.Application.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    string Street, string City, string State, string ZipCode, string Country,
    List<CreateOrderItemDto> Items
) : IRequest<Guid>;

public sealed record CreateOrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, string Currency);