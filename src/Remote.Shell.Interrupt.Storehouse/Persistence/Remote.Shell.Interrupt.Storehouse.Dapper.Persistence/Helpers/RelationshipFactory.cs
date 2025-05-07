namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

/// <summary>
/// Provides a factory for creating entity relationships.
/// </summary>
/// <typeparam name="TEntity">The type of the principal entity.</typeparam>
/// <typeparam name="TRelated">The type of the related entity.</typeparam>
internal static class RelationshipFactory<TEntity, TRelated>
{
  /// <summary>
  /// Creates and configures a relationship between two entities.
  /// </summary>
  /// <typeparam name="TRelationship">The specific relationship type being created.</typeparam>
  /// <param name="memberName">The navigation property name defining the relationship.</param>
  /// <param name="type">The relationship type (OneToOne, OneToMany, ManyToMany).</param>
  /// <param name="setup">An optional delegate for additional relationship configuration.</param>
  /// <returns>The configured relationship instance.</returns>
  /// <exception cref="InvalidOperationException">Thrown if the relationship already exists.</exception>
  public static TRelationship Create<TRelationship>(string memberName,
                                                    RelationshipType type,
                                                    Action<TRelationship>? setup = null) 
    where TRelationship : Relationship, new()
  {
    // Create and config relationship
    var relationship = new TRelationship
    {
      NavigationProperty = memberName,
      PrincipalEntity = typeof(TEntity), // TEntity и TRelated могут быть generic-параметрами
      DependentEntity = typeof(TRelated),
      RelationshipType = type
    };

    // Additional configuration
    setup?.Invoke(relationship);

    return relationship;
  }
}
