using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Commands.Login;
using OrderFlow.Application.Commands.RegisterUser;
using OrderFlow.Application.DTOs.Auth;

namespace OrderFlow.Api.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">Registration payload containing email, password, and name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The newly created user information.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterUserResponseDto>> Register(
        [FromBody] RegisterUserRequestDto request,
        CancellationToken ct)
    {
        var command = new RegisterUserCommand(
            request.Email, request.Password, request.Name);

        var result = await _mediator.Send(command, ct);

        return CreatedAtAction(nameof(Login), null, result);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">Login payload containing email and password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>JWT token with expiration and user details.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken ct)
    {
        try
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }
    }
}