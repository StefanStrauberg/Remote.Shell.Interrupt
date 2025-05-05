namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ReferenceCollectionNavigationBuilder<TEntity, TRelated>(EntityConfiguration config, Relationship relationship)
  where TEntity : class
  where TRelated : class
{
  readonly EntityConfiguration _config = config;
  readonly Relationship _relationship = relationship;

  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> HasForeignKey(
    Expression<Func<TEntity, object?>> foreignKeyExpression)
  {
    var member = ExpressionHelper.GetMemberName(foreignKeyExpression!);
    _relationship.ForeignKey = member;
    return this;
  }

  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    _relationship.IsRequired = isRequired;
    return this;
  }
}
