namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ReferenceCollectionNavigationBuilder<TEntity, TRelated>(OneToManyRelationship relationship,
                                                                       IRelationshipValidatorFactory relationshipValidatorFactory)
  where TEntity : class
  where TRelated : class
{
  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> HasForeignKey(Expression<Func<TEntity, object?>> foreignKeyExpression)
  {
    relationship.ForeignKey = ExpressionHelper.GetMemberName(foreignKeyExpression!);
    relationshipValidatorFactory.GetValidator(RelationshipType.OneToMany).Validate(relationship);
    return this;
  }

  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    relationship.IsRequired = isRequired;
    return this;
  }
}
