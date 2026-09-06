using Microsoft.AspNetCore.Identity;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

/// <summary>
/// ASP.NET Core Identity user for the application.
///
/// Lives in the Persistence layer (not Domain) on purpose: inheriting
/// <see cref="IdentityUser{TKey}"/> requires a Microsoft.AspNetCore.Identity
/// reference, which the Core (Domain/Application) layers must not carry.
/// All other layers interact with users exclusively through
/// Application.Contracts.Identity.IIdentityService and ICurrentUserService.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Display name of the account holder (optional, human-readable).
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// UTC timestamp of account creation, maintained by <see cref="IdentityService"/>.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Soft-disable switch: inactive accounts fail login even with valid credentials.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
