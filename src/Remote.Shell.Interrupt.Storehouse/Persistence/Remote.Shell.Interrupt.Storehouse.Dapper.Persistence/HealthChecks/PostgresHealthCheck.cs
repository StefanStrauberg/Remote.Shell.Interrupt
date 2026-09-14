namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.HealthChecks;

/// <summary>
/// Verifies connectivity to the PostgreSQL database this application owns
/// (<see cref="ApplicationDbContext"/>), without writing anything.
/// </summary>
internal sealed class PostgresHealthCheck(ApplicationDbContext dbContext) : IHealthCheck
{
  public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
  {
    try
    {
      return await dbContext.Database.CanConnectAsync(cancellationToken)
        ? HealthCheckResult.Healthy("PostgreSQL connection established.")
        : HealthCheckResult.Unhealthy("PostgreSQL connection could not be established.");
    }
    catch (Exception ex)
    {
      return HealthCheckResult.Unhealthy("PostgreSQL health check threw an exception.", ex);
    }
  }
}
