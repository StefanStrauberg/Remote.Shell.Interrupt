using System.Data.Common;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.HealthChecks;

/// <summary>
/// Verifies connectivity to the remote billing MySQL database via the same read-only
/// connection factory the RemBillRep repositories use (<see cref="IMySqlConnectionFactory"/>),
/// so the check is bound by the same "SET SESSION TRANSACTION READ ONLY" guarantee - it
/// only ever runs a "SELECT 1", never anything that could touch data on a database this
/// application does not own.
/// </summary>
internal sealed class MySqlHealthCheck(IMySqlConnectionFactory connectionFactory) : IHealthCheck
{
  public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
  {
    try
    {
      var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

      if (connection is not DbConnection dbConnection)
        return HealthCheckResult.Unhealthy("MySQL connection factory did not return a DbConnection.");

      using var command = dbConnection.CreateCommand();
      command.CommandText = "SELECT 1";
      await command.ExecuteScalarAsync(cancellationToken);

      return HealthCheckResult.Healthy("MySQL billing database connection established.");
    }
    catch (Exception ex)
    {
      return HealthCheckResult.Unhealthy("MySQL billing database health check threw an exception.", ex);
    }
  }
}
