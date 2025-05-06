namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ReferenceNavigationBuilder<TEntity, TRelated>(EntityConfiguration config,
                                                             Relationship relationship)
  where TEntity : class
  where TRelated : class
{
  public ReferenceNavigationBuilder<TEntity, TRelated> HasForeignKey(Expression<Func<TEntity, object>> foreignKeyExpression)
  {
    var member = ExpressionHelper.GetMemberName(foreignKeyExpression);
    
    if (relationship is OneToOneRelationship oneToOne)
      oneToOne.ForeignKey = member;
    else
      throw new InvalidOperationException("Foreign key can only be set on one-to-one relationships using this method.");
    
    return this;
  }

  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> WithMany(Expression<Func<TRelated, IEnumerable<TEntity>>>? navigationExpression = null)
  {
    config.Relationships.Remove(relationship);

    string navProp = navigationExpression != null ? ExpressionHelper.GetMemberName(navigationExpression) 
                                                  : string.Empty;
    var oneToMany = new OneToManyRelationship
    {
        NavigationProperty = relationship.NavigationProperty,
        InverseNavigationProperty = navProp,
        PrincipalEntity = typeof(TEntity),
        DependentEntity = typeof(TRelated),
        ForeignKey = (relationship as OneToOneRelationship)?.ForeignKey ?? string.Empty,
        IsRequired = relationship.IsRequired,
        RelationshipType = RelationshipType.OneToMany
    };

    config.Relationships.Add(oneToMany);
    return new ReferenceCollectionNavigationBuilder<TEntity, TRelated>(oneToMany);
  }

  public ReferenceNavigationBuilder<TEntity, TRelated> WithOne(Expression<Func<TRelated, TEntity>>? inverseNavigationExpression = null)
  {
    config.Relationships.Remove(relationship);

    var inverseNavProp = inverseNavigationExpression != null ? ExpressionHelper.GetMemberName(inverseNavigationExpression) 
                                                             : string.Empty;

    var oneToOne = new OneToOneRelationship
    {
        NavigationProperty = relationship.NavigationProperty,
        InverseNavigationProperty = inverseNavProp,
        PrincipalEntity = typeof(TEntity),
        DependentEntity = typeof(TRelated),
        ForeignKey = (relationship as OneToOneRelationship)?.ForeignKey ?? string.Empty,
        IsRequired = relationship.IsRequired,
        RelationshipType = RelationshipType.OneToOne
    };
    
    config.Relationships.Add(oneToOne);
    return new ReferenceNavigationBuilder<TEntity, TRelated>(config, oneToOne);
  }

  public ReferenceNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    relationship.IsRequired = isRequired;
    return this;
  }
}
