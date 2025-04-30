namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Request;

/// <summary>
/// Represents a descriptor for filtering data.
/// </summary>
public class FilterDescriptor
{
  /// <summary>
  /// Gets or sets the path to the property being filtered.
  /// </summary>
  /// <remarks>
  /// Example: "NetworkDevice.Name" to filter by a network device's name.
  /// </remarks>
  public string PropertyPath { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the operator used for filtering.
  /// </summary>
  /// <remarks>
  /// The operator could be equality, less than, greater than, etc.
  /// </remarks>
  public FilterOperator Operator { get; set; }

  /// <summary>
  /// Gets or sets the value to be filtered against.
  /// </summary>
  /// <remarks>
  /// The value is compared to the specified property using the given operator.
  /// </remarks>
  public string Value { get; set; } = string.Empty;
}
