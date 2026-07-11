using MediatR;
using OrderFlow.Application.DTOs;


namespace OrderFlow.Application.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    List<CreateOrderItemDto> Items
) : IRequest<Guid>;