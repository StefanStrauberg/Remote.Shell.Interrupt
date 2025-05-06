namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ReferenceNavigationBuilder<TEntity, TRelated>(EntityConfiguration config, OneToOneRelationship relationship)
  where TEntity : class
  where TRelated : class
{
  public ReferenceNavigationBuilder<TEntity, TRelated> HasForeignKey(Expression<Func<TEntity, object>> foreignKeyExpression)
  {
    var member = ((MemberExpression)foreignKeyExpression.Body).Member.Name;
    relationship.ForeignKey = member;
    return this;
  }

  // 2. Метод WithMany должен возвращать новый билдер для продолжения цепочки
  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> WithMany()
  {
    relationship.RelationshipType = RelationshipType.OneToMany;
    config.Relationships.Add(relationship);
    config.Validate();
    return new ReferenceCollectionNavigationBuilder<TEntity, TRelated>(relationship);
  }

  public ReferenceNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    relationship.IsRequired = isRequired;
    return this;
  }
}
