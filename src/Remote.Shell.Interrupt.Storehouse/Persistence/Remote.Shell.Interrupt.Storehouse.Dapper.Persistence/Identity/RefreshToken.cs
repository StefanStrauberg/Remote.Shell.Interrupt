namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

/// <summary>
/// A rotatable, revocable refresh token issued alongside a JWT access token.
///
/// Lives in the Persistence layer for the same reason as <see cref="ApplicationUser"/>:
/// it is infrastructure for the JWT issuance flow, not a core domain concept.
///
/// Only the SHA-256 hash of the token is stored, never the raw value, so a
/// database leak alone cannot be replayed as a live credential (the same
/// principle as password hashing).
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    /// <summary>SHA-256 hash (hex) of the opaque token value handed to the client.</summary>
    public string TokenHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>
    /// Set when the token is rotated (used to mint a replacement) or explicitly
    /// revoked (logout, reuse-detected compromise). Null means still active.
    /// </summary>
    public DateTime? RevokedAtUtc { get; set; }

    /// <summary>
    /// Hash of the token that replaced this one via rotation, if any. Presenting
    /// a token that already carries this value is a reuse signal: the token
    /// has already been consumed once, so it is treated as compromised.
    /// </summary>
    public string? ReplacedByTokenHash { get; set; }

    public bool IsActive
        => RevokedAtUtc is null && DateTime.UtcNow < ExpiresAtUtc;
}
