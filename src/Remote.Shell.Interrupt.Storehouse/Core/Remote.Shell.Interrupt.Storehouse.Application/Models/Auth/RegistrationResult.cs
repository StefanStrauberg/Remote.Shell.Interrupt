namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;

/// <summary>
/// Outcome of a user registration performed by <see cref="IIdentityService.RegisterAsync"/>.
/// </summary>
public sealed record RegistrationResult
{
    public bool Success { get; init; }

    public Guid? UserId { get; init; }

    public string? Error { get; init; }

    public static RegistrationResult Failed(string error)
        => new() { Success = false, Error = error };
}
