namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models.Relationships;

/// <summary>
/// Represents a many-to-many relationship between two entities.
/// </summary>
internal class ManyToManyRelationship : Relationship
{
  /// <summary>
  /// Gets or sets the join entity that facilitates the many-to-many relationship.
  /// </summary>
  public Type JoinEntity { get; set; } = null!;

  /// <summary>
  /// Gets or sets the foreign key in the principal entity.
  /// </summary>
  public string? PrincipalForeignKey { get; set; }

  /// <summary>
  /// Gets or sets the foreign key in the dependent entity.
  /// </summary>
  public string? DependentForeignKey { get; set; }
}
