namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

/// <summary>
/// Represents a traffic plan entity with identification and description properties.
/// </summary>
public class TfPlan : BaseEntity
{
  /// <summary>
  /// Gets or sets the unique identifier of the traffic plan.
  /// </summary>
  public int IdTfPlan { get; set; }

  /// <summary>
  /// Gets or sets the name of the traffic plan.
  /// </summary>
  public string NameTfPlan { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the description of the traffic plan.
  /// </summary>
  public string? DescTfPlan { get; set; } = string.Empty;
}