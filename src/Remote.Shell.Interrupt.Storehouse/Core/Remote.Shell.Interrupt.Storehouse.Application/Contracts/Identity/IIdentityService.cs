using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;

namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;

/// <summary>
/// Abstraction over the ASP.NET Core Identity infrastructure.
/// Implemented in the Persistence layer; keeps Core layers free of
/// Microsoft.AspNetCore.Identity references.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Validates credentials and issues a signed JWT access token.
    /// </summary>
    Task<AuthenticationResult> LoginAsync(string email,
                                          string password,
                                          CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user and assigns one of the supported roles ("Admin" or "User").
    /// </summary>
    Task<RegistrationResult> RegisterAsync(string email,
                                           string password,
                                           string role,
                                           CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a signed JWT containing the user id ("sub"), email and role claims.
    /// Implemented with JsonWebTokenHandler (not the deprecated JwtSecurityTokenHandler).
    /// </summary>
    Task<string> GenerateJwtTokenAsync(Guid userId,
                                       string email,
                                       IEnumerable<string> roles);

    /// <summary>
    /// Signs the user into the cookie authentication scheme (browser sessions).
    /// </summary>
    Task SignInWithCookieAsync(Guid userId,
                               bool isPersistent,
                               CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs the user out of the cookie authentication scheme.
    /// </summary>
    Task SignOutCookieAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Exchanges a valid, unexpired, unrevoked refresh token for a new access
    /// token and a new refresh token (rotation): the presented token is revoked
    /// as part of the exchange so it cannot be used again. Presenting a token
    /// that has already been rotated or revoked is treated as a compromise
    /// signal and revokes every other active refresh token for that user.
    /// </summary>
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken,
                                                 CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a refresh token immediately (JWT-flow equivalent of logout),
    /// without waiting for its natural expiry. A no-op if the token does not
    /// exist or is already revoked.
    /// </summary>
    Task RevokeRefreshTokenAsync(string refreshToken,
                                 CancellationToken cancellationToken = default);
}
