namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

/// <summary>
/// Strongly typed JWT issuance/validation settings, bound from the
/// "JwtSettings" configuration section and shared by the token issuer
/// (Persistence) and the bearer validator (API).
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>Claim type used for role claims inside issued JWTs.</summary>
    public const string RoleClaimType = "role";

    /// <summary>
    /// Identifies the token issuer; must match the bearer validation settings.
    /// </summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// Identifies the accepted audience; must match the bearer validation settings.
    /// </summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Symmetric signing key. Minimum length 32 characters; supply through
    /// user-secrets or the JwtSettings__Key environment variable, never in source.
    /// </summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// JWT access-token lifetime in minutes.
    /// </summary>
    public int ExpiryMinutes { get; init; } = 60;

    /// <summary>
    /// Authentication-cookie lifetime in days (browser sessions).
    /// </summary>
    public int CookieExpiryDays { get; init; } = 7;
}
