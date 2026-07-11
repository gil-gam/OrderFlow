using Microsoft.AspNetCore.Http;
using OrderFlow.Application.Common.Interfaces;
using System.Security.Claims;

namespace OrderFlow.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var userIdClaim = httpContextAccessor.HttpContext?
            .User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        UserId = Guid.TryParse(userIdClaim, out var uid) ? uid : Guid.Empty;
    }

    public Guid UserId { get; }
}