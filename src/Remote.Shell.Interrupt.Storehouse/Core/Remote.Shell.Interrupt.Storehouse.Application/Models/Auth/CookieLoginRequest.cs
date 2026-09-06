namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;

/// <summary>
/// Request payload for browser (cookie-based) sign-in.
/// </summary>
public sealed record CookieLoginRequest(string Email, string Password, bool IsPersistent = false)
{
    // Prevents accidental credential leakage through ToString-based logging.
    public override string ToString()
        => $"{nameof(CookieLoginRequest)} {{ {nameof(Email)} = {Email}, {nameof(Password)} = *** }}";
}
