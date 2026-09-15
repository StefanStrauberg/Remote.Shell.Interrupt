namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

/// <summary>
/// Placeholder secrets baked into docker-compose.yml and .env.example purely so that
/// <c>docker compose up</c> and a bare <c>dotnet run</c> work out of the box for local
/// development, with no manual secret provisioning step. They are intentionally public
/// (documented in the README, committed to the repo) and must never be trusted outside
/// Development: a host started with <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.IsDevelopment"/>
/// false but still carrying one of these values is signing JWTs with, or seeding an
/// administrator account protected by, a secret anyone can read from source control.
/// </summary>
/// <remarks>
/// Startup code checks these explicitly (rather than only enforcing e.g. a minimum JWT key
/// length) and refuses to start outside Development when they're still in effect - see
/// <c>ServiceRegistration.AddAuthenticationAndAuthorization</c> and <c>IdentitySeeder.SeedIdentityAsync</c>.
/// </remarks>
public static class InsecureDefaults
{
  /// <summary>
  /// The default of <c>JwtSettings__Key</c> / <c>JWT_KEY</c> in docker-compose.yml and .env.example.
  /// </summary>
  public const string DevJwtKey = "dev-only-signing-key-change-me-0123456789-abcdefgh";

  /// <summary>
  /// The default of <c>IdentitySeed__AdminPassword</c> / <c>ADMIN_PASSWORD</c> in docker-compose.yml and .env.example.
  /// </summary>
  public const string DevAdminPassword = "Admin#Dev-Only-2025";
}
