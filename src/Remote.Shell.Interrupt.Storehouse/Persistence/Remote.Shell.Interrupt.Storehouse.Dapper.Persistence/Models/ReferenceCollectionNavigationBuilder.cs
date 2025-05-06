namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ReferenceCollectionNavigationBuilder<TEntity, TRelated>(OneToManyRelationship relationship)
  where TEntity : class
  where TRelated : class
{
  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> HasForeignKey(Expression<Func<TEntity, object?>> foreignKeyExpression)
  {
    var member = ExpressionHelper.GetMemberName(foreignKeyExpression!);
    relationship.ForeignKey = member;
    return this;
  }

  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    relationship.IsRequired = isRequired;
    return this;
  }
}
