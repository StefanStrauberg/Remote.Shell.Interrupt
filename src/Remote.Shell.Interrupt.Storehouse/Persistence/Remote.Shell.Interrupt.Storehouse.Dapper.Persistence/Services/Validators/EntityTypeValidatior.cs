namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Services.Validators;

/// <summary>
/// Provides validation methods for ensuring entity configurations are correctly defined.
/// </summary>
internal static class EntityTypeValidatior
{
  /// <summary>
  /// Validates the primary key within the entity configuration.
  /// </summary>
  /// <param name="config">The entity configuration to validate.</param>
  /// <exception cref="InvalidOperationException">Thrown if the primary key is not found in the entity properties.</exception>
  public static void ValidatePrimaryKey(EntityConfiguration config)
  {
    if (!config.Properties.Contains(config.PrimaryKey))
      throw new InvalidOperationException($"Invalid primary key '{config.PrimaryKey}' for {config.EntityType.Name}");
  }

  /// <summary>
  /// Validates the relationships defined in the entity configuration.
  /// </summary>
  /// <param name="config">The entity configuration to validate.</param>
  public static void ValidateRelationships(EntityConfiguration config)
  {
    foreach (var rel in config.Relationships)
    {
      ValidateNavigationProperty(config, rel);
      ValidateRelationshipSpecifics(rel);
    }
  }

  /// <summary>
  /// Validates that the navigation property exists in the entity.
  /// </summary>
  /// <param name="config">The entity configuration to validate.</param>
  /// <param name="rel">The relationship being validated.</param>
  /// <exception cref="InvalidOperationException">Thrown if the navigation property is not found in the entity.</exception>
  private static void ValidateNavigationProperty(EntityConfiguration config,
                                                 Relationship rel)
  {
    if (!config.Properties.Contains(rel.NavigationProperty))
      throw new InvalidOperationException($"Navigation property '{rel.NavigationProperty}' not found in {config.EntityType.Name}");
  }


  /// <summary>
  /// Validates specific constraints for different relationship types.
  /// </summary>
  /// <param name="rel">The relationship to validate.</param>
  /// <exception cref="InvalidOperationException">Thrown if required attributes for a relationship type are missing.</exception>
  static void ValidateRelationshipSpecifics(Relationship rel)
  {
    switch (rel)
    {
      case OneToOneRelationship oneToOne:
        if (string.IsNullOrEmpty(oneToOne.ForeignKey))
            throw new InvalidOperationException($"ForeignKey required for {rel.NavigationProperty}");
        break;

      case OneToManyRelationship oneToMany:
        if (string.IsNullOrEmpty(oneToMany.ForeignKey))
            throw new InvalidOperationException($"ForeignKey required for {rel.NavigationProperty}");
        break;
      
      case ManyToManyRelationship manyToMany:
        ValidateManyToManyRelationship(manyToMany);
        break;
    }
  }

  /// <summary>
  /// Validates constraints specific to many-to-many relationships.
  /// </summary>
  /// <param name="rel">The many-to-many relationship to validate.</param>
  /// <exception cref="InvalidOperationException">Thrown if required attributes are missing.</exception>
  static void ValidateManyToManyRelationship(ManyToManyRelationship rel)
  {
    if (rel.JoinEntity == null)
      throw new InvalidOperationException($"JoinEntity required for {rel.NavigationProperty}");
    
    if (string.IsNullOrEmpty(rel.PrincipalForeignKey) || string.IsNullOrEmpty(rel.DependentForeignKey))
      throw new InvalidOperationException($"Both foreign keys required for {rel.NavigationProperty}");
  }
}
