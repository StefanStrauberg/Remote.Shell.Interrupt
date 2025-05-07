namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models.Relationships;

/// <summary>
/// Represents a one-to-many relationship between two entities.
/// </summary>
internal class OneToManyRelationship : Relationship
{
  /// <summary>
  /// Gets or sets the foreign key property in the dependent entity.
  /// </summary>
  public string? ForeignKey { get; set; }

  /// <summary>
  /// Gets or sets the inverse navigation property in the principal entity.
  /// </summary>
  public string? InverseNavigationProperty { get; set; }
}
