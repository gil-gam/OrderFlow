namespace OrderFlow.Api.Requests;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    List<OrderItemRequest> Items);

public sealed record OrderItemRequest(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    string? Currency);