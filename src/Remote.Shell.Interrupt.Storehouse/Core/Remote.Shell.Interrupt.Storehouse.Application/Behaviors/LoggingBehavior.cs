namespace Remote.Shell.Interrupt.Storehouse.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(IAppLogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : notnull, IRequest<TResponse>
  where TResponse : notnull
{
  readonly IAppLogger<LoggingBehavior<TRequest, TResponse>> _logger = logger
    ?? throw new ArgumentNullException(nameof(logger));

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    _logger.LogInformation("[START] Handle request={Request} Response={Response} RequestData={RequestData}",
                           typeof(TRequest).Name,
                           typeof(TResponse).Name,
                           request);

    var timer = new Stopwatch();
    timer.Start();
    var response = await next();
    timer.Stop();
    var timeTaken = timer.Elapsed;

    if (timeTaken.Seconds > 3)
      _logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken}",
                         typeof(TRequest).Name,
                         timeTaken.Seconds);
    else
      _logger.LogInformation("[END] Handle {Request} with {Response}",
                             typeof(TRequest).Name,
                             typeof(TResponse).Name);

    return response;
  }
}
