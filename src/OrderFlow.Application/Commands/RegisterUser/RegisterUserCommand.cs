using MediatR;
using OrderFlow.Application.DTOs.Auth;

namespace OrderFlow.Application.Commands.RegisterUser;

/// <summary>
/// Command to register a new user.
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string Name) : IRequest<RegisterUserResponseDto>;