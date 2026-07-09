using System.Security.Claims;

namespace OrderFlow.Application.Common.Interfaces.Auth;

/// <summary>
/// Defines the contract for JWT token generation and validation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the given user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="roles">The roles assigned to the user.</param>
    /// <returns>A signed JWT token string.</returns>
    string GenerateToken(string userId, string email, IEnumerable<string> roles);

    /// <summary>
    /// Validates a JWT token and returns the claims principal.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The ClaimsPrincipal if valid, null otherwise.</returns>
    ClaimsPrincipal? ValidateToken(string token);
}