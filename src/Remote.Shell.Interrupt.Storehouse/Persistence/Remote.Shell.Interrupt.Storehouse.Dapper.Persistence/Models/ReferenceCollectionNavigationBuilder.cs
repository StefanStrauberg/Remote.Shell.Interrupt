namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

public class ReferenceCollectionNavigationBuilder<TEntity, TRelated>(EntityConfiguration config, Relationship relationship)
  where TEntity : class
  where TRelated : class
{
  readonly EntityConfiguration _config = config;
  readonly Relationship _relationship = relationship;

  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> HasForeignKey(
    Expression<Func<TEntity, object?>> foreignKeyExpression)
  {
    var member = ReferenceCollectionNavigationBuilder<TEntity, TRelated>.GetMemberName(foreignKeyExpression);
    _relationship.ForeignKey = member;
    return this;
  }

  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    _relationship.IsRequired = isRequired;
    return this;
  }

  static string GetMemberName(Expression<Func<TEntity, object?>> expression)
  {
    var body = expression.Body;
    
    // Обрабатываем преобразование nullable типов
    if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
      body = unary.Operand;

    if (body is MemberExpression memberExpr)
      return memberExpr.Member.Name;

    throw new ArgumentException("Invalid foreign key expression");
  }
}
