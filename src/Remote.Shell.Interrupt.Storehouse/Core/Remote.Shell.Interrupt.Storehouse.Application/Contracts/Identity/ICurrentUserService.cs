namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;

/// <summary>
/// Provides the identity of the caller resolved from the authenticated
/// claims principal. Supports both JWT ("sub"/"email"/"role") and Identity
/// cookie ("nameidentifier"/"email"/ClaimTypes.Role) claim shapes, so CQRS
/// handlers are agnostic to the active authentication scheme.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Email { get; }

    IReadOnlyList<string> Roles { get; }

    bool IsAuthenticated { get; }
}
