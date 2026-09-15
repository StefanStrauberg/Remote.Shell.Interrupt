using System.Net;
using System.Net.Http.Json;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Login;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;
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
}
