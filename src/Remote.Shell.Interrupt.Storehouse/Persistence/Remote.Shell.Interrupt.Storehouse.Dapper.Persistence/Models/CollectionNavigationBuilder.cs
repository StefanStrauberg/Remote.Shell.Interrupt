namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

public class CollectionNavigationBuilder<TEntity, TRelated>(EntityConfiguration config,
                                                            Relationship relationship)
{
  readonly EntityConfiguration _config = config;
  readonly Relationship _relationship = relationship;
  
  public CollectionNavigationBuilder<TEntity, TRelated> WithOne()
  {
    _relationship.RelationshipType = RelationshipType.OneToMany;
    return this;
  }

  public CollectionNavigationBuilder<TEntity, TRelated> HasForeignKey(
      Expression<Func<TRelated, object>> foreignKeyExpression)
  {
    if (_relationship.RelationshipType != RelationshipType.OneToMany)
        throw new InvalidOperationException("HasForeignKey can only be used with OneToMany relationships");
    
    _ = foreignKeyExpression.Body switch
    {
      MemberExpression m => m,
      UnaryExpression u when u.Operand is MemberExpression m => m,
      _ => throw new ArgumentException("Invalid foreign key expression")
    };

    var member = GetMemberName(foreignKeyExpression);
    _relationship.ForeignKey = member;
    return this;
  }

  public CollectionNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    _relationship.IsRequired = isRequired;
    return this;
  }

  static string GetMemberName<T>(Expression<Func<T, object>> expression)
    => expression.Body switch
    {
        MemberExpression m => m.Member.Name,
        UnaryExpression u when u.Operand is MemberExpression m => m.Member.Name,
        NewExpression n when n.Members != null && n.Members.Count == 1 => n.Members[0].Name,
        _ => throw new InvalidOperationException("Could not determine member name from expression")
    };
}
