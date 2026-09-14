using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Remote.Shell.Interrupt.Storehouse.API.HealthChecks;

namespace Tests.Api;

public class HealthCheckResponseWriterTests
{
    static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    static async Task<JsonDocument> WriteAndParseAsync(HttpContext context, HealthReport report)
    {
        await HealthCheckResponseWriter.WriteAsync(context, report);

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var json = await reader.ReadToEndAsync();
        return JsonDocument.Parse(json);
    }

    [Fact]
    public async Task WriteAsync_SetsJsonContentType()
    {
        var context = CreateContext();
        var report = new HealthReport(new Dictionary<string, HealthReportEntry>(), TimeSpan.Zero);

        await HealthCheckResponseWriter.WriteAsync(context, report);

        context.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task WriteAsync_AllHealthy_ReportsOverallStatusAndEachCheckByName()
    {
        var context = CreateContext();
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["postgresql"] = new HealthReportEntry(HealthStatus.Healthy, "PostgreSQL connection established.",
                TimeSpan.FromMilliseconds(5), exception: null, data: null),
            ["mysql-billing"] = new HealthReportEntry(HealthStatus.Healthy, "MySQL billing database connection established.",
                TimeSpan.FromMilliseconds(8), exception: null, data: null)
        };
        var report = new HealthReport(entries, TimeSpan.FromMilliseconds(13));

        using var doc = await WriteAndParseAsync(context, report);
        var root = doc.RootElement;

        root.GetProperty("status").GetString().Should().Be("Healthy");
        root.GetProperty("totalDurationMs").GetDouble().Should().Be(13);

        var checks = root.GetProperty("checks").EnumerateArray().ToList();
        checks.Should().HaveCount(2);
        checks.Should().Contain(c => c.GetProperty("name").GetString() == "postgresql"
                                   && c.GetProperty("status").GetString() == "Healthy");
        checks.Should().Contain(c => c.GetProperty("name").GetString() == "mysql-billing"
                                   && c.GetProperty("status").GetString() == "Healthy");
    }

    [Fact]
    public async Task WriteAsync_OneCheckUnhealthy_IncludesItsExceptionMessage()
    {
        var context = CreateContext();
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["mysql-billing"] = new HealthReportEntry(HealthStatus.Unhealthy, "MySQL billing database health check threw an exception.",
                TimeSpan.FromMilliseconds(3), exception: new InvalidOperationException("connection refused"), data: null)
        };
        var report = new HealthReport(entries, TimeSpan.FromMilliseconds(3));

        using var doc = await WriteAndParseAsync(context, report);
        var root = doc.RootElement;

        root.GetProperty("status").GetString().Should().Be("Unhealthy");
        var check = root.GetProperty("checks").EnumerateArray().Single();
        check.GetProperty("name").GetString().Should().Be("mysql-billing");
        check.GetProperty("status").GetString().Should().Be("Unhealthy");
        check.GetProperty("error").GetString().Should().Be("connection refused");
    }
}
