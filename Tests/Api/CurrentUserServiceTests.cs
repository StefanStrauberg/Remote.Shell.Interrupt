using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Remote.Shell.Interrupt.Storehouse.API.Services;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

namespace Tests.Api;

public class CurrentUserServiceTests
{
    static CurrentUserService CreateService(ClaimsPrincipal? principal)
    {
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(principal is null ? null : new DefaultHttpContext { User = principal });
        return new CurrentUserService(accessor);
    }

    static ClaimsPrincipal AuthenticatedPrincipal(params Claim[] claims)
        => new(new ClaimsIdentity(claims, authenticationType: "TestAuth"));

    [Fact]
    public void UserId_JwtSubClaim_IsParsed()
    {
        var userId = Guid.NewGuid();
        var service = CreateService(AuthenticatedPrincipal(new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())));

        service.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserId_CookieNameIdentifierClaim_IsParsed()
    {
        var userId = Guid.NewGuid();
        var service = CreateService(AuthenticatedPrincipal(new Claim(ClaimTypes.NameIdentifier, userId.ToString())));

        service.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserId_NoClaims_ReturnsNull()
    {
        var service = CreateService(AuthenticatedPrincipal());

        service.UserId.Should().BeNull();
    }

    [Fact]
    public void UserId_MalformedGuidClaim_ReturnsNull()
    {
        var service = CreateService(AuthenticatedPrincipal(new Claim(JwtRegisteredClaimNames.Sub, "not-a-guid")));

        service.UserId.Should().BeNull();
    }

    [Fact]
    public void UserId_NoHttpContext_ReturnsNull()
    {
        var service = CreateService(null);

        service.UserId.Should().BeNull();
    }

    [Fact]
    public void Email_JwtEmailClaim_IsReturned()
    {
        var service = CreateService(AuthenticatedPrincipal(new Claim(JwtRegisteredClaimNames.Email, "a@test.com")));

        service.Email.Should().Be("a@test.com");
    }

    [Fact]
    public void Email_CookieEmailClaim_IsReturned()
    {
        var service = CreateService(AuthenticatedPrincipal(new Claim(ClaimTypes.Email, "a@test.com")));

        service.Email.Should().Be("a@test.com");
    }

    [Fact]
    public void IsAuthenticated_AuthenticatedIdentity_ReturnsTrue()
    {
        var service = CreateService(AuthenticatedPrincipal());

        service.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_NoHttpContext_ReturnsFalse()
    {
        var service = CreateService(null);

        service.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void IsAuthenticated_AnonymousIdentity_ReturnsFalse()
    {
        var service = CreateService(new ClaimsPrincipal(new ClaimsIdentity()));

        service.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void Roles_CombinesJwtAndCookieRoleClaimsWithoutDuplicates()
    {
        var service = CreateService(AuthenticatedPrincipal(
            new Claim(JwtSettings.RoleClaimType, "Admin"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "User")));

        service.Roles.Should().BeEquivalentTo(["Admin", "User"]);
    }

    [Fact]
    public void Roles_NoHttpContext_ReturnsEmptyList()
    {
        var service = CreateService(null);

        service.Roles.Should().BeEmpty();
    }
}
