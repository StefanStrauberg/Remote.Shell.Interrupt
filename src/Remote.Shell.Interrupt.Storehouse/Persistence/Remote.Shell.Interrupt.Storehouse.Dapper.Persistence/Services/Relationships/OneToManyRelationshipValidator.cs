namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Services.Relationships;

internal class OneToManyRelationshipValidator : IOneToManyRelationshipValidator
{
  public void Validate(Relationship relationship)
  {
    var oneToMany = relationship as OneToManyRelationship
      ?? throw new NullReferenceException($"Relationship can't be null");

    if (string.IsNullOrEmpty(oneToMany.ForeignKey))
      throw new InvalidOperationException($"ForeignKey required for {oneToMany.NavigationProperty}");
  }
}
