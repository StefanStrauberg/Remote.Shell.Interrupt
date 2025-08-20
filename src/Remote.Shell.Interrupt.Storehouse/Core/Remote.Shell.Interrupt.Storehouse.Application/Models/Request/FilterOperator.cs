namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Request;

/// <summary>
/// Defines the set of operators for filtering data.
/// </summary>
public enum FilterOperator
{
  /// <summary>
  /// Checks if the values are equal.
  /// </summary>
  Equals,

  /// <summary>
  /// Checks if the values are not equal.
  /// </summary>
  NotEquals,

  /// <summary>
  /// Checks if the value is greater than the comparison value.
  /// </summary>
  GraterThan,

  /// <summary>
  /// Checks if the value is less than the comparison value.
  /// </summary>
  LessThan,

  /// <summary>
  /// Checks if the value contains the comparison value (typically for strings).
  /// </summary>
  Contains,

  /// <summary>
  /// Checks if the value is present within a predefined set.
  /// </summary>
  In
}