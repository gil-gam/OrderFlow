namespace OrderFlow.Application.DTOs;

public sealed record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt);

public sealed record CreateCategoryRequestDto(
    string Name,
    string? Description);

public sealed record UpdateCategoryRequestDto(
    string Name,
    string? Description);