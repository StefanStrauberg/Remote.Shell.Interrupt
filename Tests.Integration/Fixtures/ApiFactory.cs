using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Remote.Shell.Interrupt.Storehouse.API;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;
using Testcontainers.MySql;
using Testcontainers.PostgreSql;

namespace Tests.Integration.Fixtures;

/// <summary>
/// Starts one ephemeral PostgreSQL container and one ephemeral MySQL container via
/// Testcontainers/Docker - entirely separate from any Postgres/MySQL instance already
/// running on the machine, disposable and torn down after the run - then boots the real API
/// pipeline (the exact same <see cref="ServiceRegistration.AddApplicationServices"/> /
/// <see cref="ServiceRegistration.ConfigurePipeline"/> extension methods <c>Program.cs</c>
/// calls in production: migrations, identity seeding, the full middleware pipeline included)
/// against them, using <see cref="TestServer"/> directly.
/// </summary>
/// <remarks>
/// Deliberately does not use <c>WebApplicationFactory&lt;Program&gt;</c>: its minimal-hosting
/// support drives the real host by reflecting into <c>Program.Main</c> and intercepting
/// <c>Build()</c>/<c>Run()</c> through a diagnostic-listener/exception-based handshake
/// (<c>HostAbortedException</c>) - a mechanism with a confirmed, still-open upstream
/// reliability bug (dotnet/aspnetcore#58442, "Quarantine WebApplicationFactory": the same
/// "Cannot access a disposed object. Object name: 'IServiceProvider'" failure this project hit
/// while evaluating that approach). Building the host directly here - the same call sequence
/// Program.cs uses, just invoked from test code instead of reflected into - reaches identical
/// application behavior without going through that fragile handshake.
/// </remarks>
public sealed class ApiFactory : IAsyncLifetime
{
  public const string AdminEmail = "admin@integration.test";
  public const string AdminPassword = "Integration#Test-Password-1";

  readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
    .WithImage("postgres:16-alpine")
    .WithDatabase("remote_shell_interrupt_it")
    .WithUsername("postgres")
    .WithPassword("postgres")
    .Build();

  readonly MySqlContainer _mysql = new MySqlBuilder()
    .WithImage("mysql:8.0")
    .WithDatabase("cod2")
    .WithUsername("billing")
    .WithPassword("billing")
    .Build();

  WebApplication? _app;
  int _nextFakeClientId;

  public string PostgresConnectionString => _postgres.GetConnectionString();
  public string MySqlConnectionString => _mysql.GetConnectionString();
  public IServiceProvider Services => _app!.Services;

  /// <summary>
  /// Overrides are applied as environment variables: this is the same override mechanism the
  /// app's own README documents for production (<c>ConnectionStrings__DefaultConnection</c>
  /// etc.), read by <c>WebApplication.CreateBuilder()</c>'s default configuration chain -
  /// exercising it here doubles as a check that the documented override path actually works.
  /// </summary>
  public async Task InitializeAsync()
  {
    await Task.WhenAll(_postgres.StartAsync(), _mysql.StartAsync());

    Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", PostgresConnectionString);
    Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection2", MySqlConnectionString);
    Environment.SetEnvironmentVariable("JwtSettings__Key", "integration-test-signing-key-at-least-32-chars-long");
    Environment.SetEnvironmentVariable("JwtSettings__Issuer", "RemoteShellInterrupt.API");
    Environment.SetEnvironmentVariable("JwtSettings__Audience", "RemoteShellInterrupt.Client");
    Environment.SetEnvironmentVariable("IdentitySeed__AdminEmail", AdminEmail);
    Environment.SetEnvironmentVariable("IdentitySeed__AdminPassword", AdminPassword);
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

    // AddControllers() discovers controllers from IWebHostEnvironment.ApplicationName's
    // assembly by default - which, unless told otherwise, is whatever process actually
    // launched (the test runner), not the API project. Point it at the API assembly
    // explicitly, or no controller (Auth, Gates, ...) is ever found and every route 404s.
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
      ApplicationName = typeof(ServiceRegistration).Assembly.GetName().Name
    });
    builder.WebHost.UseTestServer();
    builder.AddApplicationServices();

    _app = builder.Build();

    // AuthRateLimitPolicy partitions by Connection.RemoteIpAddress, which TestServer's
    // in-memory transport never sets - every request would otherwise share one "unknown"
    // bucket and different tests' login calls would rate-limit each other after 5 total,
    // regardless of which test made them. Let CreateClient's X-Test-Client-Ip header (a
    // distinct fake IP per HttpClient) stand in for it, so each test gets its own budget,
    // the same way distinct real clients would in production.
    _app.Use(async (context, next) =>
    {
      if (context.Request.Headers.TryGetValue("X-Test-Client-Ip", out var testIp)
          && IPAddress.TryParse(testIp.ToString(), out var ip))
        context.Connection.RemoteIpAddress = ip;

      await next();
    });

    _app.ConfigurePipeline();

    // Same startup sequence as Program.cs: apply migrations, then seed roles/the admin account.
    await _app.Services.SyncDatabaseAsync();

    using (var scope = _app.Services.CreateScope())
      await IdentitySeeder.SeedIdentityAsync(scope.ServiceProvider);

    await _app.StartAsync();
  }

  /// <summary>
  /// Each call gets an independent fake client IP (see the middleware registered in
  /// <see cref="InitializeAsync"/>), so tests running against this one shared host don't
  /// exhaust each other's auth rate-limit budget.
  /// </summary>
  public HttpClient CreateClient()
  {
    var client = _app!.GetTestClient();
    var id = Interlocked.Increment(ref _nextFakeClientId);
    client.DefaultRequestHeaders.Add("X-Test-Client-Ip", $"10.{(id >> 16) & 0xFF}.{(id >> 8) & 0xFF}.{id & 0xFF}");
    return client;
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    if (_app is not null)
    {
      await _app.StopAsync();
      await _app.DisposeAsync();
    }

    await Task.WhenAll(_postgres.DisposeAsync().AsTask(), _mysql.DisposeAsync().AsTask());
  }

  /// <summary>
  /// Resolves a service from a fresh DI scope, disposing the scope once <paramref name="use"/>
  /// completes. For reaching internal, DI-only services (e.g. <c>IMySqlConnectionFactory</c>)
  /// that black-box HTTP calls can't exercise directly.
  /// </summary>
  public async Task<TResult> WithScopedServiceAsync<TService, TResult>(Func<TService, Task<TResult>> use)
    where TService : notnull
  {
    using var scope = Services.CreateScope();
    var service = scope.ServiceProvider.GetRequiredService<TService>();
    return await use(service);
  }
}

[CollectionDefinition("Integration")]
public sealed class IntegrationCollection : ICollectionFixture<ApiFactory>;
