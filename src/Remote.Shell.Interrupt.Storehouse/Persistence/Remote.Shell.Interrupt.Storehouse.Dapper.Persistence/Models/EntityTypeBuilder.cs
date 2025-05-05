namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class EntityTypeBuilder<TEntity>(EntityConfiguration config) 
  where TEntity : class
{
  readonly EntityConfiguration _config = config;

  public CollectionNavigationBuilder<TEntity, TRelated> HasMany<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> navigationExpression) 
    where TRelated : class
  {
    ArgumentNullException.ThrowIfNull(navigationExpression);

    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);

    // Проверка на дублирование
    if (_config.Relationships.Any(r => r.NavigationProperty == memberName))
      throw new InvalidOperationException($"Relationship '{memberName}' already exists.");
    
    var relationship = new Relationship
    {
        NavigationProperty = memberName,
        PrincipalEntity = typeof(TEntity),
        DependentEntity = typeof(TRelated),
        RelationshipType = RelationshipType.OneToMany
    };

    _config.Relationships.Add(relationship);
    _config.Validate();
    
    return new CollectionNavigationBuilder<TEntity, TRelated>(_config, relationship);
  }

  public EntityTypeBuilder<TEntity> HasKey(Expression<Func<TEntity, object>> keyExpression)
  {
    _config.PrimaryKey = ExpressionHelper.GetMemberName(keyExpression);
    return this;
  }

  public EntityTypeBuilder<TEntity> ToTable(string tableName)
  {
    _config.TableName = tableName;
    return this;
  }

  public ReferenceNavigationBuilder<TEntity, TRelated> HasOne<TRelated>(Expression<Func<TEntity, TRelated?>> navigationExpression) 
    where TRelated : class
  {
    ArgumentNullException.ThrowIfNull(navigationExpression);

    var memberName = ExpressionHelper.GetMemberName(navigationExpression.Body);

    // Проверка на дублирование
    if (_config.Relationships.Any(r => r.NavigationProperty == memberName))
      throw new InvalidOperationException($"Relationship '{memberName}' already exists.");
    
    var relationship = new Relationship 
    { 
      NavigationProperty = memberName,
      PrincipalEntity = typeof(TRelated),
      DependentEntity = typeof(TEntity),
      RelationshipType = RelationshipType.OneToOne
    };

    _config.Relationships.Add(relationship);
    _config.Validate();
    
    return new ReferenceNavigationBuilder<TEntity, TRelated>(_config, relationship);
  }
}