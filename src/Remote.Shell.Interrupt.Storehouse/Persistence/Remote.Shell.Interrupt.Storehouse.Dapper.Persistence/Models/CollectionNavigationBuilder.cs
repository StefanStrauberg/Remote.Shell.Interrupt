namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class CollectionNavigationBuilder<TEntity, TRelated>(OneToManyRelationship relationship,
                                                              IRelationshipValidatorFactory relationshipValidatorFactory)
  where TEntity : class
  where TRelated : class
{ 
  public CollectionNavigationBuilder<TEntity, TRelated> WithOne(Expression<Func<TRelated, TEntity>>? navigationExpression = null)
  {
    if (navigationExpression != null)
      relationship.InverseNavigationProperty = ExpressionHelper.GetMemberName(navigationExpression);
    return this;
  }

  public CollectionNavigationBuilder<TEntity, TRelated> HasForeignKey(Expression<Func<TRelated, object>> foreignKeyExpression)
  {   
    relationship.ForeignKey = ExpressionHelper.GetMemberName(foreignKeyExpression);
    relationshipValidatorFactory.GetValidator(RelationshipType.OneToMany).Validate(relationship);
    return this;
  }

  public CollectionNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    relationship.IsRequired = isRequired;
    return this;
  }
}
