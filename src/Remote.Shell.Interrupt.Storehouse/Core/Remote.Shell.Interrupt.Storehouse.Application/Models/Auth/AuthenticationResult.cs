namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;

/// <summary>
/// Outcome of a credential check performed by <see cref="IIdentityService.LoginAsync"/>.
/// Never exposes why credentials failed beyond a generic message, and carries the
/// issued JWT only on success.
/// </summary>
public sealed record AuthenticationResult
{
    public bool Success { get; init; }

    public string? Token { get; init; }

    public DateTime? ExpiresAtUtc { get; init; }

    public Guid? UserId { get; init; }

    public string? Email { get; init; }

    public IReadOnlyList<string> Roles { get; init; } = [];

    public string? Error { get; init; }

    public static AuthenticationResult Failed(string error)
        => new() { Success = false, Error = error };
}
