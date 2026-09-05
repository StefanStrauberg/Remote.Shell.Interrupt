namespace Remote.Shell.Interrupt.Storehouse.Application.Behaviors;

/// <summary>
/// Implements a validation behavior in the request pipeline, validating the request against
/// registered FluentValidation validators before passing it to the next handler.
/// </summary>
/// <typeparam name="TRequest">The type of request being processed.</typeparam>
/// <typeparam name="TResponse">The type of response returned after handling the request.</typeparam>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : ICommand<TResponse>
{
  /// <summary>
  /// Validates the request against all registered validators.
  /// If validation fails, throws a <see cref="ValidationException"/> with grouped errors.
  /// If validation passes (or no validators are registered), delegates to the next handler.
  /// </summary>
  /// <param name="request">The incoming request instance.</param>
  /// <param name="next">The delegate responsible for processing the request.</param>
  /// <param name="cancellationToken">A token for canceling the operation if needed.</param>
  /// <returns>The response generated after processing the request.</returns>
  /// <exception cref="ValidationException">Thrown when one or more validators report errors.</exception>
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (!validators.Any())
        return await next();

    var context = new ValidationContext<TRequest>(request);

    var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

    var errors = validationResults.SelectMany(result => result.Errors)
                                  .Where(error => error != null)
                                  .ToList();

    if (errors.Count != 0)
    {
      // Group errors by property name.
      var errorsDictionary = errors.GroupBy(error => error.PropertyName, 
                                            error => error.ErrorMessage)
                                   .ToDictionary(group => group.Key, 
                                                 group => group.ToArray());
      
      throw new ValidationException(errorsDictionary);
    }

    return await next();
  }
}
