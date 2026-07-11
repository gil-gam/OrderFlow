namespace OrderFlow.Application.DTOs;

public sealed record OrderDetailDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    decimal TotalAmount,
    string Currency,
    List<OrderItemDetailDto> Items);

public sealed record OrderItemDetailDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Subtotal);

public sealed record CreateOrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    string Currency
);