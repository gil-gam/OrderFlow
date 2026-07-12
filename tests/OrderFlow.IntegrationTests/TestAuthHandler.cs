using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OrderFlow.IntegrationTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	public const string SchemeName = "TestScheme";

	public static string CurrentUserId { get; set; } = "11111111-1111-1111-1111-111111111111";

	public TestAuthHandler(
		IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder)
		: base(options, logger, encoder)
	{
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, CurrentUserId),
			new Claim(ClaimTypes.Name, "Test User"),
			new Claim(ClaimTypes.Email, "test@orderflow.com"),
			new Claim(ClaimTypes.Role, "Customer"),
			new Claim(ClaimTypes.Role, "Admin")
		};

		var identity = new ClaimsIdentity(claims, SchemeName);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, SchemeName);

		return Task.FromResult(AuthenticateResult.Success(ticket));
	}
}