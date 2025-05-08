namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ManyToManyNavigationBuilder<TEntity, TRelated>(EntityConfiguration config,
                                                              IRelationshipValidatorFactory relationshipValidatorFactory,
                                                              ManyToManyRelationship relationship)
  where TEntity : class
  where TRelated : class
{
  public ManyToManyNavigationBuilder<TEntity, TRelated> WithMany(Expression<Func<TRelated, IEnumerable<TEntity>>> navigationExpression)
  {
    var inverseNav = ExpressionHelper.GetMemberName(navigationExpression.Body);
    
    if (config.Relationships.Any(r => r.NavigationProperty == inverseNav))
      throw new InvalidOperationException($"Relationship '{inverseNav}' already exists.");
    
    relationship.InverseNavigationProperty = inverseNav;
    return this;
  }

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
