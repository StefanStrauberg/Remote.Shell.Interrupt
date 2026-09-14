namespace Remote.Shell.Interrupt.Storehouse.API.Middleware;

/// <summary>
/// Assigns a correlation ID to each request - reusing an inbound <c>X-Correlation-ID</c> header
/// when the caller already supplies one, generating a new one otherwise - and pushes it onto
/// Serilog's ambient log context for the lifetime of the request. Every log line written while
/// handling the request (the request-logging summary, MediatR's LoggingBehavior, repository
/// logs, etc.) is enriched with it, so they can be tied back together when investigating a
/// production incident. The value is also echoed back on the response so a caller can quote
/// it when reporting an issue.
/// </summary>
public class CorrelationIdMiddleware : IMiddleware
{
  public const string HeaderName = "X-Correlation-ID";

  async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
  {
    var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();

    if (string.IsNullOrWhiteSpace(correlationId))
      correlationId = Guid.NewGuid().ToString();

    context.Response.OnStarting(() =>
    {
      context.Response.Headers[HeaderName] = correlationId;
      return Task.CompletedTask;
    });

    using (LogContext.PushProperty("CorrelationId", correlationId))
      await next(context);
  }
}
