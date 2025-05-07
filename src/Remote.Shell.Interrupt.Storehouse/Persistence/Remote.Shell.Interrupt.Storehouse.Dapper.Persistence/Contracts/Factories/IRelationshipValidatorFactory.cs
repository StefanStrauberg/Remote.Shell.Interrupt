namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Contracts.Factories;

internal interface IRelationshipValidatorFactory
{
  IRelationshipValidator GetValidator(RelationshipType type);
}
