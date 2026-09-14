namespace Remote.Shell.Interrupt.Storehouse.API.Entities;

/// <summary>
/// Contains default configuration values used throughout the application.
/// </summary>
public static class DefaultEntities
{
  /// <summary>
  /// The default CORS policy name used for cross-origin configuration.
  /// </summary>
  public static string CorsPolicyName { get; } = "CorsPolicy";

  /// <summary>
  /// The name of the response header that exposes pagination metadata.
  /// </summary>
  public static string ExposedHeaders { get; } = "X-Pagination";

  /// <summary>
  /// Rate-limiting policy name applied to credential-checking auth endpoints
  /// (login, cookie login) to slow down brute-force/credential-stuffing attempts.
  /// Must be a compile-time constant: referenced from the <see cref="Microsoft.AspNetCore.RateLimiting.EnableRateLimitingAttribute"/>.
  /// </summary>
  public const string AuthRateLimitPolicy = "AuthRateLimit";

  /// <summary>
  /// The file path pattern for logging output, used by logging providers such as Serilog.
  /// </summary>
  public static string LoggingTo { get; } = "logs/log-.txt";
}
