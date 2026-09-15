using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.QueryFilterParser;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Users;
using Remote.Shell.Interrupt.Storehouse.Application.Exceptions;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Request;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Response;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

/// <summary>
/// ASP.NET Core Identity implementation of <see cref="IIdentityService"/>.
/// Uses <see cref="UserManager{TUser}"/>/<see cref="RoleManager{TRole}"/> for
/// account and role management, <see cref="SignInManager{TUser}"/> for cookie
/// sessions and lockout handling, and JsonWebTokenHandler for JWT issuance.
/// Refresh tokens are persisted (hashed) via <see cref="ApplicationDbContext"/>.
/// </summary>
internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext dbContext,
    IQueryFilterParser queryFilterParser,
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
        var refreshToken = await IssueRefreshTokenAsync(user.Id, cancellationToken);

        return new AuthenticationResult
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
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
            // Accounts are created exclusively by an Administrator (this method requires
            // the Admin role at the API layer) rather than via self-service sign-up, so
            // there is no untrusted party to verify an email address against and no
            // confirmation email to send. EmailConfirmed is set true by design here, not
            // left over from an unfinished flow. (RequireConfirmedEmail/-Account are also
            // left at their default of false, so this flag does not gate sign-in anyway.)
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

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken,
                                                               CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var existing = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (existing is null)
            return AuthenticationResult.Failed("Invalid refresh token.");

        if (existing.RevokedAtUtc is not null)
        {
            // The token was already rotated or revoked, yet it is being presented
            // again: someone else may hold a copy of it. Treat the whole session
            // as compromised and revoke every other active token for this user.
            await RevokeAllActiveTokensAsync(existing.UserId, cancellationToken);
            return AuthenticationResult.Failed("Invalid refresh token.");
        }

        if (existing.ExpiresAtUtc <= DateTime.UtcNow)
            return AuthenticationResult.Failed("Invalid refresh token.");

        var user = await userManager.FindByIdAsync(existing.UserId.ToString());

        if (user is null || !user.IsActive)
            return AuthenticationResult.Failed("Invalid refresh token.");

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = await GenerateJwtTokenAsync(user.Id, user.Email!, roles);
        var newRefreshToken = await IssueRefreshTokenAsync(user.Id, cancellationToken, existing);

        return new AuthenticationResult
        {
            Success = true,
            Token = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            UserId = user.Id,
            Email = user.Email,
            Roles = [.. roles]
        };
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken,
                                              CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var existing = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (existing is null || existing.RevokedAtUtc is not null)
            return;

        existing.RevokedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task RevokeAllRefreshTokensAsync(Guid userId,
                                            CancellationToken cancellationToken = default)
        => RevokeAllActiveTokensAsync(userId, cancellationToken);

    /// <summary>
    /// Mints a new refresh token for <paramref name="userId"/> and persists its
    /// hash. When <paramref name="tokenBeingRotated"/> is supplied (the refresh
    /// flow), that token is revoked in the same save so the exchange is atomic:
    /// a caller can never observe both tokens active at once.
    /// </summary>
    async Task<string> IssueRefreshTokenAsync(Guid userId,
                                              CancellationToken cancellationToken,
                                              RefreshToken? tokenBeingRotated = null)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var tokenHash = HashToken(rawToken);

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        });

        if (tokenBeingRotated is not null)
        {
            tokenBeingRotated.RevokedAtUtc = DateTime.UtcNow;
            tokenBeingRotated.ReplacedByTokenHash = tokenHash;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return rawToken;
    }

    async Task RevokeAllActiveTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var activeTokens = await dbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        foreach (var token in activeTokens)
            token.RevokedAtUtc = now;

        if (activeTokens.Count > 0)
            await dbContext.SaveChangesAsync(cancellationToken);
    }

    static string HashToken(string rawToken)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

    public async Task<PagedList<UserDTO>> GetUsersByFilterAsync(RequestParameters requestParameters,
                                                                CancellationToken cancellationToken = default)
    {
        // Queries dbContext.Users directly rather than userManager.Users: the latter
        // throws NotSupportedException unless the configured IUserStore also implements
        // IQueryableUserStore<TUser>, which the real EF Core store does but a test double
        // typically does not. The DbSet works identically in both.
        IQueryable<ApplicationUser> query = dbContext.Users.AsNoTracking();

        var filterExpr = queryFilterParser.ParseFilters<ApplicationUser>(requestParameters.Filters);
        if (filterExpr is not null)
            query = query.Where(filterExpr);

        var totalCount = await query.CountAsync(cancellationToken);

        var orderByExpr = queryFilterParser.ParseOrderBy<ApplicationUser>(requestParameters.OrderBy);
        query = orderByExpr is not null
            ? (requestParameters.OrderByDescending ? query.OrderByDescending(orderByExpr) : query.OrderBy(orderByExpr))
            : query.OrderBy(u => u.Email);

        var pagination = new PaginationContext(requestParameters.PageNumber ?? 1,
                                               requestParameters.PageSize ?? Math.Max(totalCount, 1));

        if (requestParameters.IsPaginated)
            query = query.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize);

        var users = await query.ToListAsync(cancellationToken);
        var userIds = users.Select(u => u.Id).ToList();

        var rolesByUserId = await (from userRole in dbContext.UserRoles
                                   join role in dbContext.Roles on userRole.RoleId equals role.Id
                                   where userIds.Contains(userRole.UserId)
                                   select new { userRole.UserId, role.Name })
                                  .ToListAsync(cancellationToken);

        var rolesLookup = rolesByUserId.GroupBy(x => x.UserId)
                                       .ToDictionary(g => g.Key, g => g.Select(x => x.Name!).ToList());

        var dtos = users.Select(user => new UserDTO
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAtUtc,
            Roles = rolesLookup.TryGetValue(user.Id, out var roles) ? roles : []
        });

        return PagedList<UserDTO>.Create(dtos, totalCount, pagination);
    }

    public async Task UpdateUserRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await FindUserOrThrowAsync(userId);

        var currentRoles = await userManager.GetRolesAsync(user);

        if (currentRoles.Contains(role) && currentRoles.Count == 1)
            return;

        if (currentRoles.Count > 0)
            await userManager.RemoveFromRolesAsync(user, currentRoles);

        await userManager.AddToRoleAsync(user, role);
    }

    public async Task SetUserActiveAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await FindUserOrThrowAsync(userId);

        if (user.IsActive == isActive)
            return;

        user.IsActive = isActive;
        await userManager.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await FindUserOrThrowAsync(userId);

        await userManager.DeleteAsync(user);
    }

    public async Task UpdateUserProfileAsync(Guid userId, string email, string? fullName,
                                             CancellationToken cancellationToken = default)
    {
        var user = await FindUserOrThrowAsync(userId);

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await userManager.FindByEmailAsync(email);

            if (existing is not null && existing.Id != userId)
                throw new BadRequestException($"A user with email '{email}' already exists.");

            await userManager.SetEmailAsync(user, email);
            await userManager.SetUserNameAsync(user, email);
        }

        user.FullName = fullName;
        await userManager.UpdateAsync(user);
    }

    async Task<ApplicationUser> FindUserOrThrowAsync(Guid userId)
        => await userManager.FindByIdAsync(userId.ToString())
           ?? throw new EntityNotFoundException(typeof(ApplicationUser), nameof(ApplicationUser.Id));
}
