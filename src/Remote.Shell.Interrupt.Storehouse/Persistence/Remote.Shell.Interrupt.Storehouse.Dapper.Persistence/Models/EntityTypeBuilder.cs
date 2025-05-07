namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

/// <summary>
/// Provides a fluent API for configuring entity mappings, including primary keys, table names, and relationships.
/// </summary>
/// <typeparam name="TEntity">The type of entity being configured.</typeparam>
internal class EntityTypeBuilder<TEntity>(EntityConfiguration config,
                                          IRelationshipValidatorFactory relationshipValidatorFactory) 
  where TEntity : class
{
  /// <summary>
  /// Configures the primary key for the entity.
  /// </summary>
  /// <param name="keyExpression">An expression defining the key property.</param>
  /// <returns>The current <see cref="EntityTypeBuilder{TEntity}"/> instance.</returns>
  public EntityTypeBuilder<TEntity> HasKey(Expression<Func<TEntity, object>> keyExpression)
  {
    config.PrimaryKey = ExpressionHelper.GetMemberName(keyExpression);
    return this;
  }

  /// <summary>
  /// Configures the table name for the entity.
  /// </summary>
  /// <param name="tableName">The name of the database table.</param>
  /// <returns>The current <see cref="EntityTypeBuilder{TEntity}"/> instance.</returns>
  public EntityTypeBuilder<TEntity> ToTable(string tableName)
  {
    config.TableName = tableName;
    return this;
  }

  /// <summary>
  /// Configures a one-to-one relationship between the entity and a related entity.
  /// </summary>
  /// <typeparam name="TRelated">The type of the related entity.</typeparam>
  /// <param name="navigationExpression">An expression specifying the navigation property.</param>
  /// <returns>A <see cref="ReferenceNavigationBuilder{TEntity, TRelated}"/> for further configuration.</returns>
  public ReferenceNavigationBuilder<TEntity, TRelated> HasOne<TRelated>(Expression<Func<TEntity, TRelated?>> navigationExpression) 
    where TRelated : class
  {
    ArgumentNullException.ThrowIfNull(navigationExpression);

    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);

    var relationshipType = RelationshipType.OneToOne;

    // Check uniq
    if (config.Relationships.Any(r => r.NavigationProperty == memberName))
      throw new InvalidOperationException($"Relationship '{memberName}' already exists.");

    var relationship = RelationshipFactory<TEntity, TRelated>.Create<OneToOneRelationship>(memberName,
                                                                                           relationshipType);

    // Adding configuration
    config.Relationships.Add(relationship);

    relationshipValidatorFactory.GetValidator(relationshipType).Validate(relationship);

    return new ReferenceNavigationBuilder<TEntity, TRelated>(config,
                                                             relationshipValidatorFactory,
                                                             relationship);
  }

  /// <summary>
  /// Configures a one-to-many relationship between the entity and a collection of related entities.
  /// </summary>
  /// <typeparam name="TRelated">The type of the related entities.</typeparam>
  /// <param name="navigationExpression">An expression specifying the navigation property.</param>
  /// <returns>A <see cref="CollectionNavigationBuilder{TEntity, TRelated}"/> for further configuration.</returns>
  public CollectionNavigationBuilder<TEntity, TRelated> HasMany<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> navigationExpression) 
    where TRelated : class
  {
    ArgumentNullException.ThrowIfNull(navigationExpression);

    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);
    
    // Check uniq
    if (config.Relationships.Any(r => r.NavigationProperty == memberName))
      throw new InvalidOperationException($"Relationship '{memberName}' already exists.");

    var relationshipType = RelationshipType.OneToMany;

    var relationship = RelationshipFactory<TEntity, TRelated>.Create<OneToManyRelationship>(memberName,
                                                                                            relationshipType);

    // Adding configuration
    config.Relationships.Add(relationship);

    relationshipValidatorFactory.GetValidator(relationshipType).Validate(relationship);
    
    return new CollectionNavigationBuilder<TEntity, TRelated>(relationship);
  }

  /// <summary>
  /// Configures a many-to-many relationship between the entity and a collection of related entities.
  /// </summary>
  /// <typeparam name="TRelated">The type of the related entities.</typeparam>
  /// <param name="navigationExpression">An expression specifying the navigation property.</param>
  /// <returns>A <see cref="ManyToManyNavigationBuilder{TEntity, TRelated}"/> for further configuration.</returns>
  public ManyToManyNavigationBuilder<TEntity, TRelated> HasManyToMany<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> navigationExpression)
    where TRelated : class
  {
    ArgumentNullException.ThrowIfNull(navigationExpression);

    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);

    // Check uniq
    if (config.Relationships.Any(r => r.NavigationProperty == memberName))
      throw new InvalidOperationException($"Relationship '{memberName}' already exists.");

    var relationshipType = RelationshipType.ManyToMany;

    var relationship = RelationshipFactory<TEntity, TRelated>.Create<ManyToManyRelationship>(memberName,
                                                                                             relationshipType);

    // Adding configuration
    config.Relationships.Add(relationship);

    relationshipValidatorFactory.GetValidator(relationshipType).Validate(relationship);

    return new ManyToManyNavigationBuilder<TEntity, TRelated>(relationship);
  }
}
