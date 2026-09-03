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
    logger.LogInformation("[START] Handling request: {RequestName} with data: {Request}", requestName, request);

    var stopwatch = Stopwatch.StartNew();

    try
    {
      var response = await next();
      stopwatch.Stop();
      var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

      if (elapsedSeconds > 3)
        logger.LogWarning("[PERFORMANCE] Request {RequestName} took {ElapsedSeconds:F2} seconds", requestName, elapsedSeconds);
      else
        logger.LogInformation("[END] Finished handling request: {RequestName} in {ElapsedSeconds:F2} seconds", requestName, elapsedSeconds);

      return response;
    }
    catch (Exception ex)
    {
      stopwatch.Stop();
      var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
      logger.LogError("[ERROR] Request {RequestName} failed after {ElapsedSeconds:F2} seconds: {ErrorMessage}", requestName, elapsedSeconds, ex.Message);
      throw;
    }
  }
}
