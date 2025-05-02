namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

/// <summary>
/// Represents an exception that occurs when validation errors are detected.
/// </summary>
/// <param name="errorsDictionary">A dictionary containing validation errors with associated fields.</param>
public sealed class ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary)
  : ApplicationException("Validation Failure", "One or more validation errors occurred")
{
  /// <summary>
  /// Gets the dictionary of validation errors.
  /// </summary>
  public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get => errorsDictionary; }
}
