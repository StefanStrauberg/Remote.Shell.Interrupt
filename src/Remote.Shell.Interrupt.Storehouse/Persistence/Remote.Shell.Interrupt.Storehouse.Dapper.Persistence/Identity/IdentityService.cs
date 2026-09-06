using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

/// <summary>
/// ASP.NET Core Identity implementation of <see cref="IIdentityService"/>.
/// Uses <see cref="UserManager{TUser}"/>/<see cref="RoleManager{TRole}"/> for
/// account and role management, <see cref="SignInManager{TUser}"/> for cookie
/// sessions and lockout handling, and JsonWebTokenHandler for JWT issuance.
/// </summary>
internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<JwtSettings> jwtOptions)
    : IIdentityService
{
    readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<AuthenticationResult> LoginAsync(string email,
                                                       string password,
                                                       CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return AuthenticationResult.Failed("Invalid credentials.");

        if (!user.IsActive)
            return AuthenticationResult.Failed("This account has been deactivated.");

        var signInResult = await signInManager.CheckPasswordSignInAsync(user,
                                                                        password,
                                                                        lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
            return AuthenticationResult.Failed("The account is locked out. Try again later.");

        if (!signInResult.Succeeded)
            return AuthenticationResult.Failed("Invalid credentials.");

        var roles = await userManager.GetRolesAsync(user);
        var token = await GenerateJwtTokenAsync(user.Id, user.Email!, roles);

        return new AuthenticationResult
        {
            Success = true,
            Token = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            UserId = user.Id,
            Email = user.Email,
            Roles = [.. roles]
        };
    }

    public async Task<RegistrationResult> RegisterAsync(string email,
                                                        string password,
                                                        string role,
                                                        CancellationToken cancellationToken = default)
    {
        if (!await roleManager.RoleExistsAsync(role))
            return RegistrationResult.Failed($"Role '{role}' does not exist.");

        if (await userManager.FindByEmailAsync(email) is not null)
            return RegistrationResult.Failed($"A user with email '{email}' already exists.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
            return RegistrationResult.Failed(string.Join("; ",
                createResult.Errors.Select(error => error.Description)));

        var addToRoleResult = await userManager.AddToRoleAsync(user, role);

        if (!addToRoleResult.Succeeded)
            return RegistrationResult.Failed(string.Join("; ",
                addToRoleResult.Errors.Select(error => error.Description)));

        return new RegistrationResult { Success = true, UserId = user.Id };
    }

    public Task<string> GenerateJwtTokenAsync(Guid userId,
                                              string email,
                                              IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(JwtSettings.RoleClaimType, role)));

        var identity = new ClaimsIdentity(claims, authenticationType: "Bearer");
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                SecurityAlgorithms.HmacSha256),
            Subject = identity
        };

        return Task.FromResult(new JsonWebTokenHandler().CreateToken(descriptor));
    }

    public async Task SignInWithCookieAsync(Guid userId,
                                            bool isPersistent,
                                            CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
                   ?? throw new InvalidOperationException($"User '{userId}' was not found.");

        await signInManager.SignInAsync(user, isPersistent);
    }

    public async Task SignOutCookieAsync(CancellationToken cancellationToken = default)
        => await signInManager.SignOutAsync();
}
