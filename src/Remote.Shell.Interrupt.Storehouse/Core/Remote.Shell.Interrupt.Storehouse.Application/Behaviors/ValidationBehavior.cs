namespace Remote.Shell.Interrupt.Storehouse.Application.Behaviors;

/// <summary>
/// Implements a logging behavior in the request pipeline, capturing execution time and request details.
/// </summary>
/// <typeparam name="TRequest">The type of request being processed.</typeparam>
/// <typeparam name="TResponse">The type of response returned after handling the request.</typeparam>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : ICommand<TResponse>
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
    var context = new ValidationContext<TRequest>(request);
    var errorsDictionary = validators.Select(x => x.ValidateAsync(context,
                                                                  cancellationToken)
                                                   .Result)
                                     .SelectMany(x => x.Errors)
                                     .Where(x => x != null)
                                     .GroupBy(x => x.PropertyName,
                                              x => x.ErrorMessage,
                                              (PropertyName, ErrorMessage) => new
                                              {
                                                Key = PropertyName,
                                                Values = ErrorMessage
                                              })
                                     .ToDictionary(x => x.Key,
                                                   x => x.Values.ToArray());

    if (errorsDictionary.Count != 0)
      throw new ValidationException(errorsDictionary);

    return await next();
  }
}
