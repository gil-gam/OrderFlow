using MediatR;
using OrderFlow.Application.DTOs.Auth;

namespace OrderFlow.Application.Commands.Login;

/// <summary>
/// Command to authenticate a user and return a JWT token.
/// </summary>
public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<LoginResponseDto>;