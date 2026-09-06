using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

namespace Remote.Shell.Interrupt.Storehouse.API.Services;

/// <summary>
/// Resolves the caller's identity from the authenticated claims principal.
/// Claim lookups cover both authentication schemes: JWT tokens carry raw
/// "sub"/"email"/"role" claims (MapInboundClaims = false), while Identity
/// cookies use ClaimTypes.NameIdentifier/Email/Role.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = FindValue(JwtRegisteredClaimNames.Sub)
                        ?? FindValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public string? Email
        => FindValue(JwtRegisteredClaimNames.Email) ?? FindValue(ClaimTypes.Email);

    public bool IsAuthenticated
        => Principal?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyList<string> Roles
    {
        get
        {
            var principal = Principal;

            if (principal is null)
                return [];

            return [.. principal.FindAll(JwtSettings.RoleClaimType)
                                .Concat(principal.FindAll(ClaimTypes.Role))
                                .Select(claim => claim.Value)
                                .Distinct()];
        }
    }

    string? FindValue(string claimType)
        => Principal?.FindFirstValue(claimType);
}
