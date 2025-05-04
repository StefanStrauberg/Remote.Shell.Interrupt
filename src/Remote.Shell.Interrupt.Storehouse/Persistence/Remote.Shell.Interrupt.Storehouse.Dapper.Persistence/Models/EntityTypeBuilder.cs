namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

public class EntityTypeBuilder<TEntity>(EntityConfiguration config) 
  where TEntity : class
{
  readonly EntityConfiguration _config = config;

  public CollectionNavigationBuilder<TEntity, TRelated> HasMany<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> navigationExpression) 
    where TRelated : class
  {
    var memberInfo = ((MemberExpression)navigationExpression.Body).Member;
    
    var relationship = new Relationship
    {
        NavigationProperty = memberInfo.Name,
        PrincipalEntity = typeof(TEntity),
        DependentEntity = typeof(TRelated),
        RelationshipType = RelationshipType.OneToMany
    };

    _config.Relationships.Add(relationship);
    
    return new CollectionNavigationBuilder<TEntity, TRelated>(_config, relationship);
  }

  public EntityTypeBuilder<TEntity> HasKey(Expression<Func<TEntity, object>> keyExpression)
  {
    var memberName = GetMemberName(keyExpression);
    _config.PrimaryKey = memberName;
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
    var member = ((MemberExpression)navigationExpression.Body).Member.Name;
    
    var relationship = new Relationship 
    { 
      NavigationProperty = member,
      PrincipalEntity = typeof(TRelated),
      DependentEntity = typeof(TEntity)
    };
    
    return new ReferenceNavigationBuilder<TEntity, TRelated>(_config, relationship);
  }

  static string GetMemberName(Expression<Func<TEntity, object>> expression)
  {
    var body = expression.Body;

    // Обработка оператора преобразования для value types
    if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
      body = unary.Operand;

    if (body is MemberExpression memberExpression)
      return memberExpression.Member.Name;

    throw new ArgumentException("Invalid key expression");
  }
}