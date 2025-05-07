namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models.Relationships;

/// <summary>
/// Represents a general entity relationship, defining properties common to all relationship types.
/// </summary>
internal class Relationship
{
  /// <summary>
  /// Gets or sets the principal entity type in the relationship.
  /// </summary>
  public Type PrincipalEntity { get; set; } = null!;

  /// <summary>
  /// Gets or sets the dependent entity type in the relationship.
  /// </summary>
  public Type DependentEntity { get; set; } = null!;

  /// <summary>
  /// Gets or sets the navigation property name associated with the relationship.
  /// </summary>
  public string NavigationProperty { get; set; } = string.Empty;

  /// <summary>
  /// Indicates whether the relationship is required.
  /// </summary>
  public bool IsRequired { get; set; } = false;

  /// <summary>
  /// Gets or sets the type of relationship (OneToOne, OneToMany, ManyToMany).
  /// </summary>
  public RelationshipType RelationshipType { get; set; }
}
