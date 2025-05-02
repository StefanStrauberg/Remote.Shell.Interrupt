namespace Remote.Shell.Interrupt.Storehouse.Application.Behaviors;

/// <summary>
/// Implements a logging behavior in the request pipeline, capturing execution time and request details.
/// </summary>
/// <typeparam name="TRequest">The type of request being processed.</typeparam>
/// <typeparam name="TResponse">The type of response returned after handling the request.</typeparam>
public class LoggingBehavior<TRequest, TResponse>(IAppLogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : notnull, IRequest<TResponse>
  where TResponse : notnull
{
  /// <summary>
  /// Logs request details, measures execution time, and records performance warnings if needed.
  /// </summary>
  /// <param name="request">The incoming request instance.</param>
  /// <param name="next">The delegate responsible for processing the request.</param>
  /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
  /// <returns>The response generated after processing the request.</returns>
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    var requestName = typeof(TRequest).Name;
    logger.LogInformation($"[START] Handling request: {requestName} with data: {request}");

    var stopwatch = Stopwatch.StartNew();
    
    try
    {
      var response = await next();
      return response;
    }
    finally
    {
      stopwatch.Stop();
      var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

      if (elapsedSeconds > 3)
        logger.LogWarning($"[PERFORMANCE] Request {requestName} took {elapsedSeconds:F2} seconds");
      else
        logger.LogInformation($"[END] Finished handling request: {requestName} in {elapsedSeconds:F2} seconds");
    }
  }
}
