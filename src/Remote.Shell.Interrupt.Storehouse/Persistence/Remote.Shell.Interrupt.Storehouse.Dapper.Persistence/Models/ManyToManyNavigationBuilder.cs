namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ManyToManyNavigationBuilder<TEntity, TRelated>(ManyToManyRelationship relationship,
                                                              IRelationshipValidatorFactory relationshipValidatorFactory)
  where TEntity : class
  where TRelated : class
{
  public ManyToManyNavigationBuilder<TEntity, TRelated> UsingJoinEntity<TJoin>() 
    where TJoin : class
  {
    relationship.JoinEntity = typeof(TJoin);
    return this;
  }

  public ManyToManyNavigationBuilder<TEntity, TRelated> HasForeignKeys(Expression<Func<TEntity, object>> principalForeignKeyExpression,
                                                                       Expression<Func<TRelated, object>> dependentForeignKeyExpression)
  {
    relationship.PrincipalForeignKey = ExpressionHelper.GetMemberName(principalForeignKeyExpression);
    relationship.DependentForeignKey = ExpressionHelper.GetMemberName(dependentForeignKeyExpression);
    relationshipValidatorFactory.GetValidator(RelationshipType.ManyToMany).Validate(relationship);
    return this;
  }

  public ManyToManyNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    relationship.IsRequired = isRequired;
    return this;
  }
}
