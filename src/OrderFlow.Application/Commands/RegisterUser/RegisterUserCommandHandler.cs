using MediatR;
using OrderFlow.Application.DTOs.Auth;

namespace OrderFlow.Application.Commands.RegisterUser;

/// <summary>
/// Handles new user registration.
/// </summary>
public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, RegisterUserResponseDto>
{
    public async Task<RegisterUserResponseDto> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // 🔒 Production: create the user via Identity / database
        // Example:
        //   var user = new IdentityUser { Email = request.Email, UserName = request.Email };
        //   var result = await _userManager.CreateAsync(user, request.Password);

        return await Task.FromResult(new RegisterUserResponseDto(
            Guid.NewGuid().ToString(),
            request.Email,
            request.Name));
    }
}