namespace OrderFlow.Application.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal UnitPrice,
    string Currency,
    Guid CategoryId,
    bool IsActive,
    DateTime CreatedAt);

public sealed record CreateProductRequestDto(
    string Name,
    string Description,
    decimal UnitPrice,
    string Currency,
    Guid CategoryId);

public sealed record UpdateProductRequestDto(
    string Name,
    string Description,
    decimal UnitPrice,
    string Currency,
    Guid CategoryId);