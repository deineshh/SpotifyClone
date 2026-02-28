using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SpotifyClone.Shared.BuildingBlocks.Application.Abstractions.Primitives;

namespace SpotifyClone.Shared.BuildingBlocks.Infrastructure.Auth;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            Claim? subClaim =
                (_httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier))
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            return Guid.Parse(subClaim.Value);
        }
    }
}
