namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Services.Relationships;

internal class OneToOneRelationshipValidator : IOneToOneRelationshipValidator
{
  public void Validate(Relationship relationship)
  {
    var oneToOne = relationship as OneToOneRelationship
      ?? throw new NullReferenceException($"Relationship can't be null");

    if (!string.IsNullOrEmpty(oneToOne.InverseNavigationProperty)
        && string.IsNullOrEmpty(oneToOne.ForeignKey))
    {
      throw new InvalidOperationException(
          $"ForeignKey required for relationship '{relationship.NavigationProperty}' when inverse navigation is set.");
    }
  }
}