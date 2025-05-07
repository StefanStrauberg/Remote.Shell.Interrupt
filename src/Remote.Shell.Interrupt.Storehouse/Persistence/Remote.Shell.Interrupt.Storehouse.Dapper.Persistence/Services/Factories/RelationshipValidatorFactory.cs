namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Services.Factories;

internal class RelationshipValidatorFactory(IServiceProvider serviceProvider) : IRelationshipValidatorFactory
{
  readonly IServiceProvider _serviceProvider = serviceProvider;

  public IRelationshipValidator GetValidator(RelationshipType type) =>
    type switch
    {
      RelationshipType.OneToOne => _serviceProvider.GetRequiredService<IOneToOneRelationshipValidator>(),
      RelationshipType.OneToMany => _serviceProvider.GetRequiredService<IOneToManyRelationshipValidator>(),
      RelationshipType.ManyToMany => _serviceProvider.GetRequiredService<IManyToManyRelationshipValidator>(),
      _ => throw new NotSupportedException($"Validator for {type} not supported")
    };
}