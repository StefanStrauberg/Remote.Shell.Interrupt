namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ReferenceNavigationBuilder<TEntity, TRelated>(EntityConfiguration config,
                                                             IRelationshipValidatorFactory relationshipValidatorFactory,
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

    relationshipValidatorFactory.GetValidator(RelationshipType.OneToOne).Validate(relationship);
    
    return this;
  }

  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> WithMany(Expression<Func<TRelated, IEnumerable<TEntity>>>? navigationExpression = null)
  {
    string navProp = navigationExpression != null ? ExpressionHelper.GetMemberName(navigationExpression) 
                                                  : string.Empty;
    var fk =(relationship as OneToOneRelationship)?.ForeignKey ?? string.Empty;
    var isRequired = relationship.IsRequired;
    
    if (!config.Relationships.Any(r => r.NavigationProperty == relationship.NavigationProperty))
      throw new InvalidOperationException($"Relationship '{relationship.NavigationProperty}' already exists.");

    var newRelationship = ReplaceRelationship<OneToManyRelationship>(relationship,
                                                                     relationship.NavigationProperty,
                                                                     RelationshipType.OneToMany,
                                                                     x => 
                                                                     {
                                                                      x.InverseNavigationProperty = navProp;
                                                                      x.ForeignKey = fk;
                                                                      x.IsRequired = isRequired;
                                                                      x.PrincipalEntity = relationship.PrincipalEntity;
                                                                      x.DependentEntity = relationship.DependentEntity;
                                                                     });

    return new ReferenceCollectionNavigationBuilder<TEntity, TRelated>(newRelationship,
                                                                       relationshipValidatorFactory);
  }

  public ReferenceNavigationBuilder<TEntity, TRelated> WithOne(Expression<Func<TRelated, TEntity>>? inverseNavigationExpression = null)
  {
    var inverseNavProp = inverseNavigationExpression != null ? ExpressionHelper.GetMemberName(inverseNavigationExpression) 
                                                             : string.Empty;
    var fk =(relationship as OneToOneRelationship)?.ForeignKey ?? string.Empty;
    var isRequired = relationship.IsRequired;

    // Check uniq
    if (config.Relationships.Any(r => r.NavigationProperty == relationship.NavigationProperty))
      throw new InvalidOperationException($"Relationship '{relationship.NavigationProperty}' already exists.");

    var newRelationship = ReplaceRelationship<OneToOneRelationship>(relationship, 
                                                                    relationship.NavigationProperty, 
                                                                    RelationshipType.OneToOne, 
                                                                    x =>
                                                                    {
                                                                      x.InverseNavigationProperty = inverseNavProp;
                                                                      x.ForeignKey = fk;
                                                                      x.IsRequired = isRequired;
                                                                    });

    return new ReferenceNavigationBuilder<TEntity, TRelated>(config,
                                                             relationshipValidatorFactory,
                                                             newRelationship);
  }

  public ReferenceNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    relationship.IsRequired = isRequired;
    return this;
  }

  TRelationship ReplaceRelationship<TRelationship>(Relationship oldRelationship,
                                                   string navigationProperty,
                                                   RelationshipType relationshipType,
                                                   Action<TRelationship> configure) 
    where TRelationship : Relationship, new()
  {
    config.Relationships.Remove(oldRelationship);

    var relationship = RelationshipFactory<TEntity, TRelated>.Create<TRelationship>(navigationProperty,
                                                                                    relationshipType,
                                                                                    configure); 
    // Adding configuration
    config.Relationships.Add(relationship);

    return relationship;
  }
}
