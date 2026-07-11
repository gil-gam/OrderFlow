using MediatR;
using OrderFlow.Application.DTOs;

namespace OrderFlow.Application.Commands.UpdateOrder;

public sealed record UpdateOrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    string Currency
);

public sealed record UpdateOrderCommand(
    Guid OrderId,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    List<UpdateOrderItemDto> Items
) : IRequest<bool>;

