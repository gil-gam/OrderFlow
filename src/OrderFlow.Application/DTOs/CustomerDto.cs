namespace OrderFlow.Application.DTOs;

public sealed record CustomerDto(
    Guid Id,
    Guid UserExternalId,
    string Name,
    string Email,
    string? Phone,
    DateTime CreatedAt);

public sealed record CreateCustomerRequestDto(
    string Name,
    string Email,
    string? Phone);

public sealed record UpdateCustomerRequestDto(
    string Name,
    string Email,
    string? Phone);