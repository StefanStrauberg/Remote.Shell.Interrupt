namespace Remote.Shell.Interrupt.BuildingBlocks.Exceptions;

public sealed class ValidationException(IReadOnlyDictionary<string, IEnumerable<string>> errorsDictionary) : ApplicationException("Validation Failure", "One or more validation errors occurred")
{
  public IReadOnlyDictionary<string, IEnumerable<string>> ErrorsDictionary { get; } = errorsDictionary;
}
