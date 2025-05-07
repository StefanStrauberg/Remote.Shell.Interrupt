namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class ModelBuilder(IRelationshipValidatorFactory relationshipValidatorFactory)
{
  public Dictionary<Type, EntityConfiguration> Configurations { get; } = [];

  public EntityTypeBuilder<TEntity> Entity<TEntity>()
    where TEntity : class
  {
    var type = typeof(TEntity);
    if (!Configurations.TryGetValue(type, out var config))
    {
      config = new EntityConfiguration(type);
      Configurations.Add(type, config);
    }
    return new EntityTypeBuilder<TEntity>(config, relationshipValidatorFactory);
  }

  public ModelBuilder Entity<TEntity>(Action<EntityTypeBuilder<TEntity>> buildAction) 
    where TEntity : class
  {
    if (buildAction != null)
    {
      var builder = Entity<TEntity>();
      buildAction(builder);
      return this;
    }
    throw new ArgumentNullException(nameof(buildAction));
  }
}
