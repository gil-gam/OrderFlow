namespace OrderFlow.Application.DTOs.Auth;

/// <summary>
/// Payload for user login.
/// </summary>
public sealed record LoginRequestDto(
    string Email,
    string Password);

/// <summary>
/// Response returned after successful authentication.
/// </summary>
public sealed record LoginResponseDto(
    string Token,
    DateTime ExpiresAt,
    string UserId,
    string Email);

/// <summary>
/// Payload for user registration.
/// </summary>
public sealed record RegisterUserRequestDto(
    string Email,
    string Password,
    string Name);

/// <summary>
/// Response returned after successful registration.
/// </summary>
public sealed record RegisterUserResponseDto(
    string UserId,
    string Email,
    string Name);