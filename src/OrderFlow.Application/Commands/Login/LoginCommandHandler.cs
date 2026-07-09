using MediatR;
using OrderFlow.Application.Common.Interfaces.Auth;
using OrderFlow.Application.DTOs.Auth;

namespace OrderFlow.Application.Commands.Login;

/// <summary>
/// Handles user authentication — validates credentials and issues a JWT.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly ITokenService _tokenService;

    // In production: inject UserManager<IdentityUser> or a user repository
    public LoginCommandHandler(ITokenService tokenService)
        => _tokenService = tokenService;

    public async Task<LoginResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // 🔒 Production: validate credentials against the database
        // Example:
        //   var user = await _userManager.FindByEmailAsync(request.Email);
        //   if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        //       throw new UnauthorizedAccessException("Invalid credentials.");

        // Mock credentials for development — remove in production
        if (request.Email != "admin@orderflow.com" || request.Password != "OrderFlow@2026")
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _tokenService.GenerateToken(
            Guid.NewGuid().ToString(),
            request.Email,
            ["Admin"]);

        return await Task.FromResult(new LoginResponseDto(
            token,
            DateTime.UtcNow.AddHours(8),
            Guid.NewGuid().ToString(),
            request.Email));
    }
}