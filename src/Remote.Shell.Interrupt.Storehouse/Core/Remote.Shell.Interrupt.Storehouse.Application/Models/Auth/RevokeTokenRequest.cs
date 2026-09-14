namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;

/// <summary>
/// Request payload for revoking a refresh token (JWT-flow equivalent of logout).
/// </summary>
public sealed record RevokeTokenRequest(string RefreshToken)
{
    // Prevents accidental credential leakage through ToString-based logging.
    public override string ToString()
        => $"{nameof(RevokeTokenRequest)} {{ {nameof(RefreshToken)} = *** }}";
}
