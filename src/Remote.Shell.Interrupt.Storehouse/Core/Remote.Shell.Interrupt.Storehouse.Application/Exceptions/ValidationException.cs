namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public sealed class ValidationException(IReadOnlyDictionary<string, IEnumerable<string>> errorsDictionary)
  : ApplicationException("Validation Failure", "One or more validation errors occurred")
{
  public IReadOnlyDictionary<string, IEnumerable<string>> ErrorsDictionary { get => errorsDictionary; }
}
