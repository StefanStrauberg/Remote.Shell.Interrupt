using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Remote.Shell.Interrupt.Storehouse.API.HealthChecks;

/// <summary>
/// Writes a health report as JSON, naming each individual check (e.g. "postgresql",
/// "mysql-billing") and its status, so a caller can tell which dependency is down
/// instead of just "unhealthy".
/// </summary>
public static class HealthCheckResponseWriter
{
  public static Task WriteAsync(HttpContext context, HealthReport report)
  {
    context.Response.ContentType = "application/json";

    var payload = new
    {
      status = report.Status.ToString(),
      totalDurationMs = report.TotalDuration.TotalMilliseconds,
      checks = report.Entries.Select(entry => new
      {
        name = entry.Key,
        status = entry.Value.Status.ToString(),
        description = entry.Value.Description,
        durationMs = entry.Value.Duration.TotalMilliseconds,
        error = entry.Value.Exception?.Message
      })
    };

    return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
  }
}
