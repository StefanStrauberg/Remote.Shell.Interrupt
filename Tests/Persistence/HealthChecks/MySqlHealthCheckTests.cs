using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Context;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.HealthChecks;

namespace Tests.Persistence.HealthChecks;

/// <summary>
/// Stands SQLite in for the remote billing MySQL database, the same way
/// RemoteBillingRepositoryTests does - a live connection is enough to exercise the
/// "SELECT 1" round-trip without needing the real, externally-owned database.
/// </summary>
public class MySqlHealthCheckTests : IDisposable
{
    readonly SqliteConnection _connection = new("Data Source=:memory:");

    public MySqlHealthCheckTests() => _connection.Open();

    public void Dispose() => _connection.Dispose();

    /// <summary>
    /// Hand-rolled instead of an NSubstitute mock: <see cref="IMySqlConnectionFactory"/>
    /// is internal, and NSubstitute's Castle proxy generator needs
    /// [InternalsVisibleTo("DynamicProxyGenAssembly2")] to implement an internal
    /// interface, which this assembly does not (and should not) grant.
    /// </summary>
    sealed class FakeMySqlConnectionFactory(IDbConnection connection) : IMySqlConnectionFactory
    {
        public Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
            => Task.FromResult(connection);
    }

    sealed class ThrowingMySqlConnectionFactory : IMySqlConnectionFactory
    {
        public Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
            => throw new InvalidOperationException("connection refused");
    }

    [Fact]
    public async Task CheckHealthAsync_ReachableDatabase_ReturnsHealthy()
    {
        var check = new MySqlHealthCheck(new FakeMySqlConnectionFactory(_connection));

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_ConnectionFactoryThrows_ReturnsUnhealthyWithException()
    {
        var check = new MySqlHealthCheck(new ThrowingMySqlConnectionFactory());

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Exception.Should().BeOfType<InvalidOperationException>();
    }
}
