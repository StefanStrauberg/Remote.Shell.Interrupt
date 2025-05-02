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
    if (validators is null || !validators.Any())
        return await next();

    var context = new ValidationContext<TRequest>(request);

    var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

    var errors = validationResults.SelectMany(result => result.Errors)
                                  .Where(error => error != null)
                                  .ToList();

    if (errors.Count != 0)
    {
      // Группируем ошибки по имени свойства. 
      var errorsDictionary = errors.GroupBy(error => error.PropertyName, 
                                            error => error.ErrorMessage)
                                   .ToDictionary(group => group.Key, 
                                                 group => group.ToArray());
      
      throw new ValidationException(errorsDictionary);
    }

    return await next();
  }
}
