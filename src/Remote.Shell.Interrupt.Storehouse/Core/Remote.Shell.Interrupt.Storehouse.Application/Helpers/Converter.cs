namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

/// <summary>
/// Provides conversion utilities for common data types.
/// </summary>
public static class Converter
{
  /// <summary>
  /// Converts an enumerable of integers into a comma-separated string.
  /// </summary>
  /// <param name="array">The sequence of integers to convert.</param>
  /// <returns>
  /// A string containing the integers separated by commas. 
  /// Returns an empty string if <paramref name="array"/> is null or empty.
  /// </returns>
  /// <remarks>
  /// Useful for logging, serialization, or display purposes. 
  /// Handles null gracefully by returning an empty string.
  /// </remarks>
  public static string ArrayToString(IEnumerable<int> array)
    => array?.Any() is true ? string.Join(", ", array) : string.Empty;
}
