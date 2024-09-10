namespace Remote.Shell.Interrupt.Storehouse.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : ICommand<TResponse>
{
  readonly IEnumerable<IValidator<TRequest>> _validators = validators
    ?? throw new ArgumentNullException(nameof(validators));

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    var context = new ValidationContext<TRequest>(request);
    var errorsDictionary = _validators.Select(x => x.ValidateAsync(context,
                                                                   cancellationToken).Result)
                                      .SelectMany(x => x.Errors)
                                      .Where(x => x != null)
                                      .GroupBy(x => x.PropertyName,
                                               x => x.ErrorMessage,
                                               (PropertyName, ErrorMessage) => new
                                               {
                                                 Key = PropertyName,
                                                 Values = ErrorMessage
                                               })
                                      .ToDictionary(x => x.Key, x => x.Values);

    if (errorsDictionary.Count != 0)
      throw new ValidationException(errorsDictionary);

    return await next();
  }
}
