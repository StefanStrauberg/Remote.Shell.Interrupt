using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

namespace Tests.Persistence;

public class IdentityServiceTests
{
    readonly UserManager<ApplicationUser> _userManager;
    readonly RoleManager<IdentityRole<Guid>> _roleManager;
    readonly SignInManager<ApplicationUser> _signInManager;
    readonly JwtSettings _jwtSettings = new()
    {
        Issuer = "test-issuer",
        Audience = "test-audience",
        Key = "0123456789abcdef0123456789abcdef",
        ExpiryMinutes = 60,
        CookieExpiryDays = 7
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

        _service = new IdentityService(_userManager, _roleManager, _signInManager, Options.Create(_jwtSettings));
    }

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
        result.UserId.Should().Be(user.Id);
        result.Email.Should().Be("a@test.com");
        result.Roles.Should().Contain("Admin");
        result.ExpiresAtUtc.Should().NotBeNull();
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
}
