namespace OrderFlow.Application.DTOs;

public sealed record OrderListDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    string Status,
    decimal TotalAmount,
    string Currency,
    int ItemCount,
    DateTime OrderDate);