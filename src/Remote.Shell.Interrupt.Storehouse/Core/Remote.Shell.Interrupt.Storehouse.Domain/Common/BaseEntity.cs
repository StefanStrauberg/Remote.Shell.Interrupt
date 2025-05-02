namespace Remote.Shell.Interrupt.Storehouse.Domain.Common;

/// <summary>
/// Base entity class providing common properties for all domain entities.
/// </summary>
public abstract class BaseEntity
{
  /// <summary>
  /// Gets or sets the unique identifier of the entity.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the entity was created.
  /// </summary>
  public DateTime CreatedAt { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the entity was last updated.
  /// </summary>
  public DateTime? UpdatedAt { get; set; }
}
