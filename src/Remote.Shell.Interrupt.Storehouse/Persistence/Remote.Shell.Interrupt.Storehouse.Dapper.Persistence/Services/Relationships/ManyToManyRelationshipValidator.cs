namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Services.Relationships;

internal class ManyToManyRelationshipValidator : IManyToManyRelationshipValidator
{
  public void Validate(Relationship relationship)
  {
    var manyToMany = relationship as ManyToManyRelationship
      ?? throw new NullReferenceException($"Relationship can't be null");

    if (manyToMany.JoinEntity == null)
      throw new InvalidOperationException($"JoinEntity required for {relationship.NavigationProperty}");
    
    if (string.IsNullOrEmpty(manyToMany.PrincipalForeignKey) || 
        string.IsNullOrEmpty(manyToMany.DependentForeignKey))
      throw new InvalidOperationException($"Both foreign keys required for {manyToMany.NavigationProperty}");
  }
}
