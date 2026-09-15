using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.QueryFilterParser;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;
using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;

namespace Tests.Persistence;

public class IdentityServiceTests : IDisposable
{
    readonly UserManager<ApplicationUser> _userManager;
    readonly RoleManager<IdentityRole<Guid>> _roleManager;
    readonly SignInManager<ApplicationUser> _signInManager;
    readonly ApplicationDbContext _dbContext = TestDbContextFactory.CreateContext();
    readonly IQueryFilterParser _queryFilterParser = new CommonQueryFilterParser();
    readonly JwtSettings _jwtSettings = new()
    {
        Issuer = "test-issuer",
        Audience = "test-audience",
        Key = "0123456789abcdef0123456789abcdef",
        ExpiryMinutes = 60,
        CookieExpiryDays = 7,
        RefreshTokenExpiryDays = 14
    };
    readonly IdentityService _service;

    public IdentityServiceTests()
    {
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _roleManager = Substitute.For<RoleManager<IdentityRole<Guid>>>(
            Substitute.For<IRoleStore<IdentityRole<Guid>>>(), null, null, null, null);
        _signInManager = Substitute.For<SignInManager<ApplicationUser>>(
            _userManager,
            Substitute.For<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null);

        _service = new IdentityService(_userManager, _roleManager, _signInManager, _dbContext, _queryFilterParser, Options.Create(_jwtSettings));
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsGenericFailure()
    {
        _userManager.FindByEmailAsync("missing@test.com").Returns((ApplicationUser?)null);

        var result = await _service.LoginAsync("missing@test.com", "pw");

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Invalid credentials.");
    }

    [Fact]
    public async Task LoginAsync_InactiveUser_ReturnsDeactivatedError()
    {
        var user = new ApplicationUser { Email = "a@test.com", IsActive = false };
        _userManager.FindByEmailAsync("a@test.com").Returns(user);

        var result = await _service.LoginAsync("a@test.com", "pw");

        result.Success.Should().BeFalse();
        result.Error.Should().Be("This account has been deactivated.");
    }

    [Fact]
    public async Task LoginAsync_LockedOutUser_ReturnsLockedOutError()
    {
        var user = new ApplicationUser { Email = "a@test.com", IsActive = true };
        _userManager.FindByEmailAsync("a@test.com").Returns(user);
        _signInManager.CheckPasswordSignInAsync(user, "pw", true).Returns(SignInResult.LockedOut);

        var result = await _service.LoginAsync("a@test.com", "pw");

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("locked out");
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsGenericFailure()
    {
        var user = new ApplicationUser { Email = "a@test.com", IsActive = true };
        _userManager.FindByEmailAsync("a@test.com").Returns(user);
        _signInManager.CheckPasswordSignInAsync(user, "wrong", true).Returns(SignInResult.Failed);

        var result = await _service.LoginAsync("a@test.com", "wrong");

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Invalid credentials.");
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokenAndRoles()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "a@test.com", IsActive = true };
        _userManager.FindByEmailAsync("a@test.com").Returns(user);
        _signInManager.CheckPasswordSignInAsync(user, "pw", true).Returns(SignInResult.Success);
        _userManager.GetRolesAsync(user).Returns((IList<string>)["Admin"]);

        var result = await _service.LoginAsync("a@test.com", "pw");

        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.UserId.Should().Be(user.Id);
        result.Email.Should().Be("a@test.com");
        result.Roles.Should().Contain("Admin");
        result.ExpiresAtUtc.Should().NotBeNull();

        _dbContext.RefreshTokens.Should().ContainSingle(t => t.UserId == user.Id && t.RevokedAtUtc == null);
    }

    [Fact]
    public async Task RegisterAsync_RoleDoesNotExist_ReturnsFailure()
    {
        _roleManager.RoleExistsAsync("Admin").Returns(false);

        var result = await _service.RegisterAsync("a@test.com", "pw", "Admin");

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("does not exist");
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ReturnsFailure()
    {
        _roleManager.RoleExistsAsync("Admin").Returns(true);
        _userManager.FindByEmailAsync("a@test.com").Returns(new ApplicationUser());

        var result = await _service.RegisterAsync("a@test.com", "pw", "Admin");

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task RegisterAsync_CreateFails_ReturnsJoinedErrorMessages()
    {
        _roleManager.RoleExistsAsync("Admin").Returns(true);
        _userManager.FindByEmailAsync("a@test.com").Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), "pw")
                    .Returns(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        var result = await _service.RegisterAsync("a@test.com", "pw", "Admin");

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Password too weak");
    }

    [Fact]
    public async Task RegisterAsync_AddToRoleFails_ReturnsJoinedErrorMessages()
    {
        _roleManager.RoleExistsAsync("Admin").Returns(true);
        _userManager.FindByEmailAsync("a@test.com").Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), "pw").Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), "Admin")
                    .Returns(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed" }));

        var result = await _service.RegisterAsync("a@test.com", "pw", "Admin");

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Role assignment failed");
    }

    [Fact]
    public async Task RegisterAsync_Success_ReturnsUserId()
    {
        _roleManager.RoleExistsAsync("Admin").Returns(true);
        _userManager.FindByEmailAsync("a@test.com").Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), "pw").Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), "Admin").Returns(IdentityResult.Success);

        var result = await _service.RegisterAsync("a@test.com", "pw", "Admin");

        result.Success.Should().BeTrue();
        result.UserId.Should().NotBeNull();
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ProducesTokenWithExpectedClaims()
    {
        var userId = Guid.NewGuid();

        var token = await _service.GenerateJwtTokenAsync(userId, "a@test.com", ["Admin", "User"]);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Issuer.Should().Be("test-issuer");
        jwt.Audiences.Should().Contain("test-audience");
        jwt.Subject.Should().Be(userId.ToString());
        jwt.Claims.Should().Contain(c => c.Type == "email" && c.Value == "a@test.com");
        jwt.Claims.Where(c => c.Type == JwtSettings.RoleClaimType).Select(c => c.Value)
           .Should().BeEquivalentTo(["Admin", "User"]);
    }

    [Fact]
    public async Task SignInWithCookieAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        _userManager.FindByIdAsync(userId.ToString()).Returns((ApplicationUser?)null);

        var act = async () => await _service.SignInWithCookieAsync(userId, isPersistent: true);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task SignInWithCookieAsync_UserFound_SignsInViaSignInManager()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);

        await _service.SignInWithCookieAsync(user.Id, isPersistent: true);

        await _signInManager.Received().SignInAsync(user, true);
    }

    [Fact]
    public async Task SignOutCookieAsync_CallsSignInManagerSignOut()
    {
        await _service.SignOutCookieAsync();

        await _signInManager.Received().SignOutAsync();
    }

    [Fact]
    public async Task RefreshTokenAsync_UnknownToken_ReturnsGenericFailure()
    {
        var result = await _service.RefreshTokenAsync("does-not-exist");

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Invalid refresh token.");
    }

    [Fact]
    public async Task RefreshTokenAsync_ExpiredToken_ReturnsGenericFailure()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "a@test.com", IsActive = true };
        var rawToken = await SeedRefreshTokenAsync(user.Id, expiresAtUtc: DateTime.UtcNow.AddMinutes(-1));

        var result = await _service.RefreshTokenAsync(rawToken);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Invalid refresh token.");
    }

    [Fact]
    public async Task RefreshTokenAsync_UserNoLongerActive_ReturnsGenericFailure()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "a@test.com", IsActive = false };
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        var rawToken = await SeedRefreshTokenAsync(user.Id);

        var result = await _service.RefreshTokenAsync(rawToken);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Invalid refresh token.");
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_RotatesAndReturnsNewTokenPair()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "a@test.com", IsActive = true };
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _userManager.GetRolesAsync(user).Returns((IList<string>)["Admin"]);
        var rawToken = await SeedRefreshTokenAsync(user.Id);

        var result = await _service.RefreshTokenAsync(rawToken);

        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBe(rawToken);
        result.UserId.Should().Be(user.Id);
        result.Roles.Should().Contain("Admin");

        var oldEntity = _dbContext.RefreshTokens.Single(t => t.TokenHash == HashForTest(rawToken));
        oldEntity.RevokedAtUtc.Should().NotBeNull();
        oldEntity.ReplacedByTokenHash.Should().Be(HashForTest(result.RefreshToken!));

        _dbContext.RefreshTokens.Should().ContainSingle(t => t.TokenHash == HashForTest(result.RefreshToken!)
                                                              && t.RevokedAtUtc == null);
    }

    [Fact]
    public async Task RefreshTokenAsync_AlreadyRotatedToken_IsRejectedAndRevokesAllActiveTokensForUser()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "a@test.com", IsActive = true };
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _userManager.GetRolesAsync(user).Returns((IList<string>)[]);

        var firstRawToken = await SeedRefreshTokenAsync(user.Id);
        var secondRawToken = await SeedRefreshTokenAsync(user.Id);

        // Legitimately rotate the first token once (as a real client would).
        var rotated = await _service.RefreshTokenAsync(firstRawToken);
        rotated.Success.Should().BeTrue();

        // An attacker (or the original client after losing a race) replays the
        // now-rotated token: this must fail and burn every other active token,
        // including the unrelated second token and the newly-rotated one.
        var reuseResult = await _service.RefreshTokenAsync(firstRawToken);

        reuseResult.Success.Should().BeFalse();
        reuseResult.Error.Should().Be("Invalid refresh token.");
        _dbContext.RefreshTokens.Where(t => t.UserId == user.Id)
                                .Should().OnlyContain(t => t.RevokedAtUtc != null);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_UnknownToken_DoesNotThrow()
    {
        var act = async () => await _service.RevokeRefreshTokenAsync("does-not-exist");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_ActiveToken_MarksItRevoked()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        var rawToken = await SeedRefreshTokenAsync(user.Id);

        await _service.RevokeRefreshTokenAsync(rawToken);

        _dbContext.RefreshTokens.Single(t => t.TokenHash == HashForTest(rawToken))
                  .RevokedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_RevokedToken_IsIdempotent()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        var rawToken = await SeedRefreshTokenAsync(user.Id);
        await _service.RevokeRefreshTokenAsync(rawToken);
        var firstRevocation = _dbContext.RefreshTokens.Single(t => t.TokenHash == HashForTest(rawToken)).RevokedAtUtc;

        await _service.RevokeRefreshTokenAsync(rawToken);

        _dbContext.RefreshTokens.Single(t => t.TokenHash == HashForTest(rawToken))
                  .RevokedAtUtc.Should().Be(firstRevocation);
    }

    [Fact]
    public async Task GetUsersByFilterAsync_ReturnsUsersWithRolesSortedByEmailByDefault()
    {
        var adminRole = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" };
        var userA = new ApplicationUser { Id = Guid.NewGuid(), Email = "b@test.com", IsActive = true, CreatedAtUtc = DateTime.UtcNow };
        var userB = new ApplicationUser { Id = Guid.NewGuid(), Email = "a@test.com", IsActive = false, CreatedAtUtc = DateTime.UtcNow };
        _dbContext.Users.AddRange(userA, userB);
        _dbContext.Roles.Add(adminRole);
        _dbContext.UserRoles.Add(new IdentityUserRole<Guid> { UserId = userA.Id, RoleId = adminRole.Id });
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetUsersByFilterAsync(new RequestParameters());

        result.Should().HaveCount(2);
        result[0].Email.Should().Be("a@test.com");
        result[0].Roles.Should().BeEmpty();
        result[1].Email.Should().Be("b@test.com");
        result[1].Roles.Should().ContainSingle().Which.Should().Be("Admin");
        result[1].IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetUsersByFilterAsync_FiltersAndPaginates()
    {
        for (var i = 0; i < 3; i++)
            _dbContext.Users.Add(new ApplicationUser { Id = Guid.NewGuid(), Email = $"active{i}@test.com", IsActive = true });
        _dbContext.Users.Add(new ApplicationUser { Id = Guid.NewGuid(), Email = "inactive@test.com", IsActive = false });
        await _dbContext.SaveChangesAsync();

        var parameters = new RequestParameters
        {
            PageNumber = 1,
            PageSize = 2,
            Filters = [new FilterDescriptor(nameof(ApplicationUser.IsActive), FilterOperator.Equals, "true")]
        };

        var result = await _service.GetUsersByFilterAsync(parameters);

        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.IsActive);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_UnknownUser_ThrowsEntityNotFound()
    {
        var act = async () => await _service.UpdateUserRoleAsync(Guid.NewGuid(), "Admin");

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ReplacesExistingRolesWithTheNewOne()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _userManager.GetRolesAsync(user).Returns((IList<string>)["User"]);
        _userManager.RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>()).Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(user, "Admin").Returns(IdentityResult.Success);

        await _service.UpdateUserRoleAsync(user.Id, "Admin");

        await _userManager.Received().RemoveFromRolesAsync(user, Arg.Is<IEnumerable<string>>(r => r.Contains("User")));
        await _userManager.Received().AddToRoleAsync(user, "Admin");
    }

    [Fact]
    public async Task UpdateUserRoleAsync_AlreadyHasOnlyThatRole_DoesNothing()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _userManager.GetRolesAsync(user).Returns((IList<string>)["Admin"]);

        await _service.UpdateUserRoleAsync(user.Id, "Admin");

        await _userManager.DidNotReceive().RemoveFromRolesAsync(user, Arg.Any<IEnumerable<string>>());
        await _userManager.DidNotReceive().AddToRoleAsync(user, Arg.Any<string>());
    }

    [Fact]
    public async Task SetUserActiveAsync_UnknownUser_ThrowsEntityNotFound()
    {
        var act = async () => await _service.SetUserActiveAsync(Guid.NewGuid(), false);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task SetUserActiveAsync_TogglesFlagAndPersists()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), IsActive = true };
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _userManager.UpdateAsync(user).Returns(IdentityResult.Success);

        await _service.SetUserActiveAsync(user.Id, false);

        user.IsActive.Should().BeFalse();
        await _userManager.Received().UpdateAsync(user);
    }

    [Fact]
    public async Task DeleteUserAsync_UnknownUser_ThrowsEntityNotFound()
    {
        var act = async () => await _service.DeleteUserAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task DeleteUserAsync_KnownUser_CallsUserManagerDelete()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _userManager.DeleteAsync(user).Returns(IdentityResult.Success);

        await _service.DeleteUserAsync(user.Id);

        await _userManager.Received().DeleteAsync(user);
    }

    async Task<string> SeedRefreshTokenAsync(Guid userId, DateTime? expiresAtUtc = null)
    {
        var rawToken = Guid.NewGuid().ToString("N");

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            TokenHash = HashForTest(rawToken),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = expiresAtUtc ?? DateTime.UtcNow.AddDays(14)
        });
        await _dbContext.SaveChangesAsync();

        return rawToken;
    }

    static string HashForTest(string rawToken)
        => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));
}
