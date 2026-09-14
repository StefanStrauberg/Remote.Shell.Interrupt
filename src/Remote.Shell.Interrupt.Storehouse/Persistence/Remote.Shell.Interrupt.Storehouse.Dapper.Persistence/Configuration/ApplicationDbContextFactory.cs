using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;

/// <summary>
/// Lets EF Core design-time tooling (<c>dotnet ef migrations add</c>, <c>dotnet ef
/// database update</c>) construct <see cref="ApplicationDbContext"/> directly against
/// this project, instead of building the full API host. Building the host would run
/// startup validation that requires runtime secrets (e.g. JwtSettings:Key) that are
/// deliberately absent from source control, so design-time operations would fail
/// before ever reaching EF Core.
///
/// The connection string here only needs to be well-formed, not reachable: `migrations
/// add` never opens a connection. `database update` does, so pass a real one via
/// the ConnectionStrings__DefaultConnection environment variable when using it directly
/// (normal application startup does not go through this factory at all — see
/// <see cref="DatabaseInitializationExtensions.SyncDatabaseAsync"/>).
/// </summary>
internal sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
            npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
