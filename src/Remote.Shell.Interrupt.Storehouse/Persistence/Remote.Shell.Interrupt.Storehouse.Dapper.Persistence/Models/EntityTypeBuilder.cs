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

    // Проверка на дублирование
    if (config.Relationships.Any(r => r.NavigationProperty == memberName))
      throw new InvalidOperationException($"Relationship '{memberName}' already exists.");
    
    var relationship = CreateOneToOneRelationship(navigationExpression);

    config.Relationships.Add(relationship);
    config.Validate();
    
    return new ReferenceNavigationBuilder<TEntity, TRelated>(config, relationship);
  }

  public CollectionNavigationBuilder<TEntity, TRelated> HasMany<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> navigationExpression) 
    where TRelated : class
  {
    ArgumentNullException.ThrowIfNull(navigationExpression);

    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);

    // Проверка на дублирование
    if (config.Relationships.Any(r => r.NavigationProperty == memberName))
      throw new InvalidOperationException($"Relationship '{memberName}' already exists.");
    
    var relationship = CreateOneToManyRelationship(navigationExpression);

    config.Relationships.Add(relationship);
    config.Validate();
    
    return new CollectionNavigationBuilder<TEntity, TRelated>(config, relationship);
  }

  static OneToOneRelationship CreateOneToOneRelationship<TRelated>(Expression<Func<TEntity, TRelated?>> navigationExpression)
  {
    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);
    return new OneToOneRelationship 
    { 
      NavigationProperty = memberName,
      PrincipalEntity = typeof(TRelated),
      DependentEntity = typeof(TEntity),
      RelationshipType = RelationshipType.OneToOne
    };
  }

  static OneToManyRelationship CreateOneToManyRelationship<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> navigationExpression)
  {
    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);
    return new OneToManyRelationship
    {
      NavigationProperty = memberName,
      PrincipalEntity = typeof(TEntity),
      DependentEntity = typeof(TRelated),
      RelationshipType = RelationshipType.OneToMany
    };
  }
}