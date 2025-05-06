namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class EntityTypeBuilder<TEntity>(EntityConfiguration config) 
  where TEntity : class
{
  public EntityTypeBuilder<TEntity> HasKey(Expression<Func<TEntity, object>> keyExpression)
  {
    config.PrimaryKey = ExpressionHelper.GetMemberName(keyExpression);
    return this;
  }

  public EntityTypeBuilder<TEntity> ToTable(string tableName)
  {
    config.TableName = tableName;
    return this;
  }

  public ReferenceNavigationBuilder<TEntity, TRelated> HasOne<TRelated>(Expression<Func<TEntity, TRelated?>> navigationExpression) 
    where TRelated : class
  {
    ArgumentNullException.ThrowIfNull(navigationExpression);

    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);

    ValidateRelationshipUniqueness(memberName);
    
    var relationship = new OneToOneRelationship
    {
      NavigationProperty = memberName,
      PrincipalEntity = typeof(TEntity),
      DependentEntity = typeof(TRelated),
      RelationshipType = RelationshipType.OneToOne
    };

    config.Relationships.Add(relationship);
    return new ReferenceNavigationBuilder<TEntity, TRelated>(config, relationship);
  }

  public CollectionNavigationBuilder<TEntity, TRelated> HasMany<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> navigationExpression) 
    where TRelated : class
  {
    ArgumentNullException.ThrowIfNull(navigationExpression);

    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);

    ValidateRelationshipUniqueness(memberName);
    
    var relationship = new OneToManyRelationship
    {
      NavigationProperty = memberName,
      PrincipalEntity = typeof(TEntity),
      DependentEntity = typeof(TRelated),
      RelationshipType = RelationshipType.OneToMany
    };

    config.Relationships.Add(relationship);
    return new CollectionNavigationBuilder<TEntity, TRelated>(relationship);
  }

  public ManyToManyNavigationBuilder<TEntity, TRelated> HasManyToMany<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> navigationExpression)
    where TRelated : class
  {
    ArgumentNullException.ThrowIfNull(navigationExpression);

    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);

    ValidateRelationshipUniqueness(memberName);

    var relationship = new ManyToManyRelationship
    {
      NavigationProperty = memberName,
      PrincipalEntity = typeof(TEntity),
      DependentEntity = typeof(TRelated),
      RelationshipType = RelationshipType.ManyToMany
    };

    config.Relationships.Add(relationship);
    return new ManyToManyNavigationBuilder<TEntity, TRelated>(relationship);
  }

  void ValidateRelationshipUniqueness(string memberName)
  {
    if (config.Relationships.Any(r => r.NavigationProperty == memberName))
      throw new InvalidOperationException($"Relationship '{memberName}' already exists.");
  }
}