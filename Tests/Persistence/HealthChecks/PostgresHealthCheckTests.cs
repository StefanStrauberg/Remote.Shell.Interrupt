using Microsoft.Extensions.Diagnostics.HealthChecks;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.HealthChecks;
using Tests.Persistence;

namespace Tests.Persistence.HealthChecks;

public class PostgresHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_ReachableDatabase_ReturnsHealthy()
    {
        using var dbContext = TestDbContextFactory.CreateContext();
        var check = new PostgresHealthCheck(dbContext);

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_DisposedContext_ReturnsUnhealthyWithException()
    {
        var dbContext = TestDbContextFactory.CreateContext();
        dbContext.Dispose();
        var check = new PostgresHealthCheck(dbContext);

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Exception.Should().NotBeNull();
    }
}
