namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ReferenceNavigationBuilder<TEntity, TRelated>(EntityConfiguration config, Relationship relationship)
  where TEntity : class
  where TRelated : class
{
  readonly EntityConfiguration _config = config;
  readonly Relationship _relationship = relationship;

  public ReferenceNavigationBuilder<TEntity, TRelated> HasForeignKey(Expression<Func<TEntity, object>> foreignKeyExpression)
  {
    var member = ((MemberExpression)foreignKeyExpression.Body).Member.Name;
    _relationship.ForeignKey = member;
    return this;
  }

  // 2. Метод WithMany должен возвращать новый билдер для продолжения цепочки
  public ReferenceCollectionNavigationBuilder<TEntity, TRelated> WithMany()
  {
    _relationship.RelationshipType = RelationshipType.OneToMany;
    _config.Relationships.Add(_relationship);
    _config.Validate();
    return new ReferenceCollectionNavigationBuilder<TEntity, TRelated>(_config, _relationship);
  }

  public ReferenceNavigationBuilder<TEntity, TRelated> IsRequired(bool isRequired = true)
  {
    _relationship.IsRequired = isRequired;
    return this;
  }
}
