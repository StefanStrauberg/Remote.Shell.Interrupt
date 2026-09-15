using System.Net;
using System.Net.Http.Json;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Login;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;
using Tests.Integration.Fixtures;

namespace Tests.Integration;

/// <summary>
/// Nothing in the unit test suite ever ran a real EF Core migration or the identity seeder
/// against an actual PostgreSQL server - InMemory/SQLite substitutes don't validate the
/// Npgsql-specific SQL migrations generate. These tests boot the real host against a fresh,
/// empty Postgres container the same way a first deploy would, and check the two things
/// Program.cs does before it starts serving traffic actually took effect.
/// </summary>
[Collection("Integration")]
public class DatabaseMigrationAndSeedingTests(ApiFactory apiFactory)
{
  readonly ApiFactory _apiFactory = apiFactory;

  [Fact]
  public async Task Startup_FreshDatabase_AppliesMigrationsAndBecomesReady()
  {
    var client = _apiFactory.CreateClient();

    var response = await client.GetAsync("/health/ready");

    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task Startup_FreshDatabase_SeedsAdminRoleAndAccount()
  {
    var client = _apiFactory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/v1/Auth/Login",
      new LoginCommand(ApiFactory.AdminEmail, ApiFactory.AdminPassword));

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadFromJsonAsync<AuthenticationResult>();
    result!.Success.Should().BeTrue();
    result.Roles.Should().Contain("Admin");
  }

  /// <summary>
  /// Regression coverage for the race SyncDatabaseAsync's Postgres advisory lock exists to
  /// prevent: several replicas of this API starting at once, all calling MigrateAsync against
  /// the same schema concurrently. The fixture's own startup already applied every migration,
  /// so this isn't the "fresh empty database" case - it's the equally realistic "already at the
  /// target version, several replicas still boot together on a redeploy" case. Without the
  /// lock serializing them, concurrent calls are still likely to succeed against an
  /// already-migrated schema (there's nothing left to apply), so the meaningful assertion is
  /// that every call completes without throwing - a deadlock or provider-level conflict would
  /// surface here as a faulted task.
  /// </summary>
  [Fact]
  public async Task SyncDatabaseAsync_CalledConcurrently_AllCallsCompleteWithoutError()
  {
    var tasks = Enumerable.Range(0, 5)
      .Select(_ => _apiFactory.Services.SyncDatabaseAsync())
      .ToArray();

    var act = () => Task.WhenAll(tasks);

    await act.Should().NotThrowAsync();
  }
}
